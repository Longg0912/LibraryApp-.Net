namespace LibraryApp.Models;

/// <summary>
/// Phiếu trả sách. Một phiếu mượn có thể có nhiều phiếu trả (trả theo từng đợt),
/// mỗi phiếu trả chứa nhiều dòng chi tiết <see cref="ReturnReceiptDetail"/>.
/// Mapping với bảng <c>dbo.ReturnReceipts</c>.
/// </summary>
public sealed class ReturnReceipt : BaseAuditableEntity
{
    /// <summary>
    /// Khoá chính. Mapping với cột <c>ReturnId</c>.
    /// </summary>
    public int ReturnId { get; set; }

    /// <summary>
    /// Mã phiếu trả duy nhất (ví dụ: <c>PT2026001</c>).
    /// </summary>
    public required string ReturnCode { get; set; }

    /// <summary>
    /// Khoá ngoại tới phiếu mượn gốc mà phiếu trả này thuộc về.
    /// </summary>
    public int BorrowId { get; set; }

    /// <summary>Khoá ngoại tới thủ thư tiếp nhận trả sách.</summary>
    public int UserId { get; set; }

    /// <summary>Ngày trả sách.</summary>
    public DateOnly ReturnDate { get; set; }

    /// <summary>
    /// Tổng tiền phạt phát sinh trong đợt trả này (cộng từ các dòng chi tiết).
    /// </summary>
    public decimal TotalFine { get; set; }

    /// <summary>Ghi chú tự do của thủ thư.</summary>
    public string? Note { get; set; }

    // ---------- Navigation properties ----------

    /// <summary>Phiếu mượn gốc (nạp khi JOIN).</summary>
    public BorrowReceipt? BorrowReceipt { get; set; }

    /// <summary>Thủ thư đã tiếp nhận trả sách.</summary>
    public User? Librarian { get; set; }

    /// <summary>
    /// Danh sách chi tiết các bản sách đã trả trong phiếu này.
    /// </summary>
    public List<ReturnReceiptDetail> Details { get; set; } = [];

    // ---------- Computed property ----------

    /// <summary>
    /// Tổng số bản sách đã trả trong phiếu này.
    /// </summary>
    public int TotalReturnedBooks => Details.Sum(d => d.Quantity);
}