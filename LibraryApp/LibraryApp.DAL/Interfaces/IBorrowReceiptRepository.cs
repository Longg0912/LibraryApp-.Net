using System.Data;
using LibraryApp.Models;
using LibraryApp.Models.Enums;

namespace LibraryApp.DAL.Interfaces;

/// <summary>
/// Repository cho entity <see cref="BorrowReceipt"/>. Việc lập phiếu mượn và
/// ghi nhận trả sách được thực hiện qua stored procedure
/// (<c>sp_Borrow_Create</c>, <c>sp_Return_Create</c>) để đảm bảo transaction
/// và chống race condition — xem các method <c>CreateBorrow</c> dưới đây.
/// </summary>
public interface IBorrowReceiptRepository : IRepository<BorrowReceipt, int>
{
    /// <summary>
    /// Tìm kiếm phiếu mượn theo nhiều tiêu chí.
    /// </summary>
    List<BorrowReceipt> Search(string? receiptCode, int? readerId, BorrowStatus? status,
        DateOnly? fromDate, DateOnly? toDate);

    /// <summary>Trả DataTable cho DataGridView.</summary>
    DataTable SearchAsDataTable(string? receiptCode, int? readerId, BorrowStatus? status,
        DateOnly? fromDate, DateOnly? toDate);

    /// <summary>
    /// Lấy phiếu mượn kèm toàn bộ chi tiết (Details), độc giả, thủ thư.
    /// Dùng khi mở form xem chi tiết hoặc lập phiếu trả.
    /// </summary>
    BorrowReceipt? GetByIdWithDetails(int borrowId);

    /// <summary>
    /// Lấy danh sách phiếu đang hoạt động (chưa trả hết) của một độc giả.
    /// </summary>
    List<BorrowReceipt> GetActiveByReader(int readerId);

    /// <summary>
    /// Gọi stored procedure <c>sp_Borrow_Create</c> để lập phiếu mượn mới.
    /// Toàn bộ logic kiểm tra tồn kho, trừ <c>AvailableQty</c>, audit log
    /// được thực hiện trong stored procedure (an toàn race condition).
    /// </summary>
    /// <param name="receipt">Đầu phiếu (chưa có <c>BorrowId</c>).</param>
    /// <param name="items">Các dòng chi tiết, ít nhất 1 dòng.</param>
    /// <returns>ID của phiếu vừa tạo.</returns>
    int CreateBorrow(BorrowReceipt receipt, IEnumerable<BorrowReceiptDetail> items);

    /// <summary>
    /// Gọi stored procedure <c>sp_Return_Create</c> để ghi nhận trả sách.
    /// </summary>
    /// <param name="returnReceipt">Đầu phiếu trả.</param>
    /// <param name="items">Các dòng chi tiết trả (đã chỉ rõ <c>BorrowDetailId</c>).</param>
    /// <returns>ID phiếu trả vừa tạo.</returns>
    int CreateReturn(ReturnReceipt returnReceipt, IEnumerable<ReturnReceiptDetail> items);
}
