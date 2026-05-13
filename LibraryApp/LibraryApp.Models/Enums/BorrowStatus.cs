namespace LibraryApp.Models.Enums;

/// <summary>
/// Trạng thái của một phiếu mượn theo vòng đời nghiệp vụ.
/// Mapping với cột <c>BorrowReceipts.Status</c>.
/// </summary>
public enum BorrowStatus
{
    /// <summary>Phiếu đang được mượn, chưa có sách nào được trả.</summary>
    Borrowing,

    /// <summary>Đã trả một phần số sách trên phiếu.</summary>
    PartiallyReturned,

    /// <summary>Đã trả toàn bộ sách. Phiếu đóng.</summary>
    Returned,

    /// <summary>Phiếu đã quá hạn trả mà vẫn còn sách chưa trả.</summary>
    Overdue,

    /// <summary>Phiếu bị huỷ trước khi giao sách (hiếm khi xảy ra).</summary>
    Cancelled
}