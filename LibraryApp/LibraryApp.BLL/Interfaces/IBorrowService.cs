using System.Data;
using LibraryApp.Models;
using LibraryApp.Models.Enums;

namespace LibraryApp.BLL.Interfaces;

/// <summary>Service quản lý nghiệp vụ mượn/trả sách.</summary>
public interface IBorrowService
{
    /// <summary>Tìm kiếm phiếu mượn theo các tiêu chí.</summary>
    DataTable Search(string? receiptCode, int? readerId, BorrowStatus? status,
        DateOnly? fromDate, DateOnly? toDate);

    /// <summary>Lấy chi tiết phiếu mượn kèm các dòng và thông tin liên quan.</summary>
    BorrowReceipt? GetDetail(int borrowId);

    /// <summary>Phiếu đang hoạt động của một độc giả (để gợi ý khi lập phiếu mới).</summary>
    List<BorrowReceipt> GetActiveByReader(int readerId);

    /// <summary>
    /// Lập phiếu mượn mới. Tự sinh mã phiếu, validate độc giả/tồn kho/giới hạn.
    /// Toàn bộ thao tác trừ tồn kho + insert detail nằm trong transaction phía SQL.
    /// </summary>
    /// <param name="readerId">ID độc giả.</param>
    /// <param name="userId">ID thủ thư lập phiếu.</param>
    /// <param name="dueDate">Ngày phải trả.</param>
    /// <param name="items">Danh sách (BookId, Quantity) cần mượn.</param>
    /// <param name="note">Ghi chú tuỳ chọn.</param>
    /// <returns>Phiếu mượn vừa tạo (đã có ID).</returns>
    BorrowReceipt CreateBorrow(int readerId, int userId, DateOnly dueDate,
        IEnumerable<(int BookId, int Quantity)> items, string? note = null);

    /// <summary>
    /// Ghi nhận trả sách. Tính tiền phạt quá hạn / hỏng / mất rồi gọi stored procedure.
    /// </summary>
    /// <param name="borrowId">Phiếu mượn gốc.</param>
    /// <param name="userId">Thủ thư tiếp nhận.</param>
    /// <param name="returnDate">Ngày trả.</param>
    /// <param name="items">
    /// Chi tiết trả: (BorrowDetailId, Quantity, Condition, FineOverride).
    /// FineOverride = null thì hệ thống tự tính theo công thức.
    /// </param>
    /// <param name="note">Ghi chú.</param>
    /// <returns>Phiếu trả vừa tạo.</returns>
    ReturnReceipt CreateReturn(int borrowId, int userId, DateOnly returnDate,
        IEnumerable<(int BorrowDetailId, int Quantity, BookCondition Condition, decimal? FineOverride)> items,
        string? note = null);

    /// <summary>
    /// Tính tiền phạt ước tính nếu trả ở thời điểm <paramref name="asOfDate"/>.
    /// Áp dụng cho UI: hiển thị số tiền dự kiến lúc thủ thư đang nhập phiếu trả.
    /// </summary>
    decimal CalculateOverdueFine(BorrowReceipt receipt, int notReturnedQty, DateOnly asOfDate);
}
