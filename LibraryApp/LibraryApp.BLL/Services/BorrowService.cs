using LibraryApp.BLL.Common;
using LibraryApp.BLL.Interfaces;
using LibraryApp.BLL.Validation;

using LibraryApp.DAL.Common;
using LibraryApp.DAL.Interfaces;

using LibraryApp.Models;
using LibraryApp.Models.Enums;

using System.Data;

namespace LibraryApp.BLL.Services;
/// <summary>
/// Service nghiệp vụ mượn/trả sách. Đây là service phức tạp nhất vì phải:
/// validate độc giả + thẻ, validate tồn kho, sinh mã phiếu duy nhất,
/// tính tiền phạt quá hạn, và phối hợp với stored procedure để đảm bảo
/// transaction toàn vẹn ở phía SQL Server.
/// </summary>
/// <remarks>
/// Phần lớn logic chống race condition đã nằm trong stored procedure
/// (<c>sp_Borrow_Create</c> với <c>UPDLOCK + HOLDLOCK</c>); BLL chỉ chịu trách nhiệm
/// validate đầu vào ở mức "đẹp UI" và bao bọc các lỗi 5002x / 5003x từ SP
/// thành thông báo tiếng Việt thân thiện.
/// </remarks>
public sealed class BorrowService : IBorrowService
{
    private readonly IBorrowReceiptRepository _borrowRepo;
    private readonly IReaderRepository _readerRepo;
    private readonly IBookRepository _bookRepo;

    /// <summary>
    /// Tiền phạt mặc định mỗi ngày quá hạn (VNĐ). Trong production nên load
    /// từ bảng <c>SystemSettings</c>; ở đây hard-code cho bài tập lớn.
    /// </summary>
    private const decimal DefaultFinePerDay = 5000m;

    /// <summary>Số ngày mượn mặc định nếu thủ thư không nhập DueDate.</summary>
    public const int DefaultBorrowDays = 14;

    /// <summary>Khởi tạo với các repository được inject.</summary>
    public BorrowService(IBorrowReceiptRepository borrowRepo,
        IReaderRepository readerRepo, IBookRepository bookRepo)
    {
        _borrowRepo = borrowRepo ?? throw new ArgumentNullException(nameof(borrowRepo));
        _readerRepo = readerRepo ?? throw new ArgumentNullException(nameof(readerRepo));
        _bookRepo = bookRepo ?? throw new ArgumentNullException(nameof(bookRepo));
    }

    /// <inheritdoc/>
    public DataTable Search(string? receiptCode, int? readerId, BorrowStatus? status,
        DateOnly? fromDate, DateOnly? toDate)
    {
        if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
            throw new BusinessException("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

        return _borrowRepo.SearchAsDataTable(receiptCode?.Trim(), readerId, status, fromDate, toDate);
    }

    /// <inheritdoc/>
    public BorrowReceipt? GetDetail(int borrowId)
    {
        Validator.Positive(borrowId, "ID phiếu mượn");
        return _borrowRepo.GetByIdWithDetails(borrowId);
    }

    /// <inheritdoc/>
    public List<BorrowReceipt> GetActiveByReader(int readerId)
    {
        Validator.Positive(readerId, "ID độc giả");
        return _borrowRepo.GetActiveByReader(readerId);
    }

    /// <inheritdoc/>
    public BorrowReceipt CreateBorrow(int readerId, int userId, DateOnly dueDate,
        IEnumerable<(int BookId, int Quantity)> items, string? note = null)
    {
        Validator.Positive(readerId, "ID độc giả");
        Validator.Positive(userId, "ID thủ thư");

        // -- 1. Validate độc giả còn hiệu lực --------------------------------
        var reader = _readerRepo.GetById(readerId)
            ?? throw new BusinessException("Không tìm thấy độc giả.");

        if (!reader.IsCardValid)
        {
            var reason = reader.Status switch
            {
                ReaderStatus.Locked => "Thẻ độc giả đang bị khoá.",
                ReaderStatus.Expired => "Thẻ độc giả đã hết hạn.",
                _ => $"Thẻ độc giả không hợp lệ (hết hạn ngày {reader.CardExpireDate:dd/MM/yyyy})."
            };
            throw new BusinessException(reason);
        }

        // -- 2. Validate ngày mượn / ngày trả ----------------------------------
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (dueDate <= today)
            throw new BusinessException("Ngày trả phải sau ngày hôm nay.");
        if ((dueDate.DayNumber - today.DayNumber) > 90)
            throw new BusinessException("Thời gian mượn tối đa 90 ngày.");

        // -- 3. Validate danh sách sách ----------------------------------------
        var itemList = items?.ToList() ?? [];
        if (itemList.Count == 0)
            throw new BusinessException("Phiếu mượn phải có ít nhất một quyển sách.");

        if (itemList.Any(i => i.Quantity <= 0))
            throw new BusinessException("Số lượng mượn của mỗi sách phải lớn hơn 0.");

        // Gộp các dòng trùng BookId (UI có thể đưa lên 2 dòng cho cùng 1 sách)
        var merged = itemList
            .GroupBy(i => i.BookId)
            .Select(g => (BookId: g.Key, Quantity: g.Sum(x => x.Quantity)))
            .ToList();

        // -- 4. Pre-check tồn kho (UX tốt hơn — báo trước khi gọi SP) -----------
        foreach (var (bookId, qty) in merged)
        {
            var book = _bookRepo.GetById(bookId)
                ?? throw new BusinessException($"Sách ID {bookId} không tồn tại.");

            if (book.Status != BookStatus.Available)
                throw new BusinessException($"Sách '{book.Title}' không thể cho mượn (trạng thái: {book.Status}).");
            if (book.AvailableQty < qty)
                throw new BusinessException(
                    $"Sách '{book.Title}' chỉ còn {book.AvailableQty} bản, không đủ cho {qty} bản yêu cầu.");
        }

        // -- 5. Gọi stored procedure -------------------------------------------
        var receipt = new BorrowReceipt
        {
            ReceiptCode = GenerateReceiptCode("PM"),
            ReaderId = readerId,
            UserId = userId,
            BorrowDate = today,
            DueDate = dueDate,
            Status = BorrowStatus.Borrowing,
            Note = note?.Trim()
        };

        var details = merged.Select(m => new BorrowReceiptDetail
        {
            BookId = m.BookId,
            Quantity = m.Quantity
        });

        try
        {
            _borrowRepo.CreateBorrow(receipt, details);
            return receipt;
        }
        catch (DalException ex) when (ex.SqlErrorNumber is 50020 or 50021 or 50022)
        {
            // SP đã trả về thông báo tiếng Việt rõ ràng — bọc lại thành BusinessException
            throw new BusinessException(ex.Message, ex);
        }
    }

    /// <inheritdoc/>
    public ReturnReceipt CreateReturn(int borrowId, int userId, DateOnly returnDate,
        IEnumerable<(int BorrowDetailId, int Quantity, BookCondition Condition, decimal? FineOverride)> items,
        string? note = null)
    {
        Validator.Positive(borrowId, "ID phiếu mượn");
        Validator.Positive(userId, "ID thủ thư");

        var receipt = _borrowRepo.GetByIdWithDetails(borrowId)
            ?? throw new BusinessException("Không tìm thấy phiếu mượn.");

        if (receipt.Status is BorrowStatus.Returned or BorrowStatus.Cancelled)
            throw new BusinessException("Phiếu này đã đóng, không thể ghi nhận trả thêm.");

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (returnDate > today)
            throw new BusinessException("Ngày trả không được sau ngày hôm nay.");
        if (returnDate < receipt.BorrowDate)
            throw new BusinessException("Ngày trả không được trước ngày mượn.");

        var itemList = items?.ToList() ?? [];
        if (itemList.Count == 0)
            throw new BusinessException("Phiếu trả phải có ít nhất một dòng.");

        // Validate từng dòng + tính phạt tự động nếu không override
        var details = new List<ReturnReceiptDetail>();
        foreach (var (detailId, qty, condition, fineOverride) in itemList)
        {
            Validator.Positive(qty, "Số lượng trả");

            var borrowDetail = receipt.Details.FirstOrDefault(d => d.BorrowDetailId == detailId)
                ?? throw new BusinessException($"Dòng chi tiết ID {detailId} không thuộc phiếu này.");

            var remaining = borrowDetail.Quantity - borrowDetail.ReturnedQty;
            if (qty > remaining)
                throw new BusinessException(
                    $"Số lượng trả ({qty}) vượt quá số còn nợ ({remaining}) của sách.");

            // Tự tính tiền phạt nếu không override
            decimal fine = fineOverride ?? CalculateLineFine(receipt, qty, condition, returnDate);

            details.Add(new ReturnReceiptDetail
            {
                BorrowDetailId = detailId,
                Quantity = qty,
                Condition = condition,
                Fine = fine
            });
        }

        var returnReceipt = new ReturnReceipt
        {
            ReturnCode = GenerateReceiptCode("PT"),
            BorrowId = borrowId,
            UserId = userId,
            ReturnDate = returnDate,
            TotalFine = details.Sum(d => d.Fine),
            Note = note?.Trim()
        };

        try
        {
            _borrowRepo.CreateReturn(returnReceipt, details);
            return returnReceipt;
        }
        catch (DalException ex) when (ex.SqlErrorNumber is 50030 or 50031)
        {
            throw new BusinessException(ex.Message, ex);
        }
    }

    /// <inheritdoc/>
    public decimal CalculateOverdueFine(BorrowReceipt receipt, int notReturnedQty, DateOnly asOfDate)
    {
        ArgumentNullException.ThrowIfNull(receipt);
        if (notReturnedQty <= 0) return 0m;
        if (asOfDate <= receipt.DueDate) return 0m;

        int daysOverdue = asOfDate.DayNumber - receipt.DueDate.DayNumber;
        return daysOverdue * DefaultFinePerDay * notReturnedQty;
    }

    // ----------------------------------------------------------------
    // Helpers nội bộ
    // ----------------------------------------------------------------

    /// <summary>
    /// Tính tiền phạt cho một dòng trả: kết hợp phạt quá hạn + phạt hỏng/mất.
    /// </summary>
    private decimal CalculateLineFine(BorrowReceipt receipt, int qty,
        BookCondition condition, DateOnly returnDate)
    {
        // 1. Phạt quá hạn
        decimal overdueFine = CalculateOverdueFine(receipt, qty, returnDate);

        // 2. Phạt hỏng/mất: lấy theo giá sách nếu có thông tin trong Details->Book
        decimal damageFine = 0m;
        if (condition is BookCondition.Damaged or BookCondition.Lost)
        {
            // Tìm BookId qua BorrowDetailId để lấy giá
            var borrowDetail = receipt.Details.FirstOrDefault();
            if (borrowDetail is not null)
            {
                // Lấy giá sách nếu Detail có gắn Book (do GetByIdWithDetails có thể chưa nạp Book)
                var book = _bookRepo.GetById(borrowDetail.BookId);
                if (book is not null)
                {
                    damageFine = condition switch
                    {
                        BookCondition.Lost => book.Price * qty,         // đền 100% giá
                        BookCondition.Damaged => book.Price * qty * 0.3m,  // phạt 30% giá
                        _ => 0m
                    };
                }
            }
        }

        return overdueFine + damageFine;
    }

    /// <summary>
    /// Sinh mã phiếu duy nhất theo prefix và timestamp.
    /// Format: <c>{prefix}{yyMMddHHmmss}{xx}</c> với xx là 2 chữ số random
    /// để giảm xác suất trùng khi sinh đồng thời ở 2 máy.
    /// </summary>
    private static string GenerateReceiptCode(string prefix)
    {
        var ts = DateTime.Now.ToString("yyMMddHHmmss");
        var random = Random.Shared.Next(0, 100).ToString("D2");
        return $"{prefix}{ts}{random}";
    }
}
