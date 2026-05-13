using LibraryApp.Models.Enums;

namespace LibraryApp.Models;

/// <summary>
/// Phiếu mượn sách – đầu phiếu. Một phiếu mượn gắn với đúng một độc giả,
/// được lập bởi một thủ thư, và chứa nhiều dòng chi tiết <see cref="BorrowReceiptDetail"/>.
/// Mapping với bảng <c>dbo.BorrowReceipts</c>.
/// </summary>
public sealed class BorrowReceipt : BaseAuditableEntity
{
    /// <summary>
    /// Khoá chính. Mapping với cột <c>BorrowId</c>.
    /// </summary>
    public int BorrowId { get; set; }

    /// <summary>
    /// Mã phiếu mượn duy nhất, in trên biên nhận giấy (ví dụ: <c>PM2026001</c>).
    /// </summary>
    public required string ReceiptCode { get; set; }

    /// <summary>Khoá ngoại tới độc giả mượn sách.</summary>
    public int ReaderId { get; set; }

    /// <summary>Khoá ngoại tới thủ thư đã lập phiếu.</summary>
    public int UserId { get; set; }

    /// <summary>
    /// Ngày lập phiếu (cũng là ngày bắt đầu tính hạn trả).
    /// </summary>
    public DateOnly BorrowDate { get; set; }

    /// <summary>
    /// Ngày phải trả sách. Khi hôm nay vượt qua mốc này mà còn sách chưa trả,
    /// phiếu sẽ bị chuyển sang <see cref="BorrowStatus.Overdue"/>.
    /// </summary>
    public DateOnly DueDate { get; set; }

    /// <summary>
    /// Trạng thái phiếu mượn theo vòng đời nghiệp vụ.
    /// </summary>
    public BorrowStatus Status { get; set; } = BorrowStatus.Borrowing;

    /// <summary>
    /// Tổng tiền phạt đã phát sinh trên phiếu (VNĐ). Cộng dồn qua từng lần trả.
    /// </summary>
    public decimal TotalFine { get; set; }

    /// <summary>
    /// Số lần đã gia hạn phiếu mượn. Không vượt quá giới hạn cấu hình
    /// (<c>SystemSettings.MaxRenewCount</c>).
    /// </summary>
    public int RenewCount { get; set; }

    /// <summary>Ghi chú tự do của thủ thư.</summary>
    public string? Note { get; set; }

    // ---------- Navigation properties ----------

    /// <summary>Đối tượng độc giả tương ứng (nạp khi JOIN).</summary>
    public Reader? Reader { get; set; }

    /// <summary>Đối tượng thủ thư đã lập phiếu.</summary>
    public User? Librarian { get; set; }

    /// <summary>
    /// Danh sách các dòng chi tiết của phiếu. Được khởi tạo rỗng để tránh
    /// <c>NullReferenceException</c> khi thao tác trên UI.
    /// </summary>
    public List<BorrowReceiptDetail> Details { get; set; } = [];

    /// <summary>
    /// Các phiếu trả gắn với phiếu mượn này. Một phiếu mượn có thể được trả
    /// thành nhiều đợt nên đây là quan hệ 1–N.
    /// </summary>
    public List<ReturnReceipt> Returns { get; set; } = [];

    // ---------- Computed properties ----------

    /// <summary>
    /// Số ngày còn lại đến hạn trả (âm nếu đã quá hạn).
    /// </summary>
    public int DaysUntilDue =>
        DueDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber;

    /// <summary>
    /// Cho biết phiếu đã quá hạn tại thời điểm hiện tại hay chưa.
    /// </summary>
    public bool IsOverdue =>
        Status is BorrowStatus.Borrowing or BorrowStatus.PartiallyReturned or BorrowStatus.Overdue
        && DueDate < DateOnly.FromDateTime(DateTime.Today);

    /// <summary>
    /// Tổng số sách trên phiếu (tính theo từng bản copy).
    /// </summary>
    public int TotalBooks => Details.Sum(d => d.Quantity);

    /// <summary>
    /// Số sách còn nợ chưa trả.
    /// </summary>
    public int NotReturnedQty => Details.Sum(d => d.Quantity - d.ReturnedQty);
}