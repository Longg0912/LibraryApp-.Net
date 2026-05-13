namespace LibraryApp.Models;

/// <summary>
/// Một dòng chi tiết trên phiếu mượn: cho biết một đầu sách cụ thể được mượn
/// với số lượng bao nhiêu, đã trả lại bao nhiêu.
/// Mapping với bảng <c>dbo.BorrowReceiptDetails</c>.
/// </summary>
/// <remarks>
/// Mỗi phiếu mượn có ràng buộc <c>UNIQUE(BorrowId, BookId)</c> nên một đầu sách
/// chỉ xuất hiện đúng một dòng. Nếu cần mượn nhiều bản, hãy tăng <see cref="Quantity"/>
/// thay vì tạo nhiều dòng.
/// </remarks>
public sealed class BorrowReceiptDetail
{
    /// <summary>
    /// Khoá chính. Mapping với cột <c>BorrowDetailId</c>.
    /// </summary>
    public int BorrowDetailId { get; set; }

    /// <summary>Khoá ngoại tới phiếu mượn cha.</summary>
    public int BorrowId { get; set; }

    /// <summary>Khoá ngoại tới đầu sách được mượn.</summary>
    public int BookId { get; set; }

    /// <summary>
    /// Số lượng bản copy được mượn ở dòng này (> 0).
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Số bản đã được trả lại. Luôn nằm trong khoảng <c>[0, Quantity]</c>.
    /// Khi <see cref="ReturnedQty"/> = <see cref="Quantity"/>, dòng coi như đã đóng.
    /// </summary>
    public int ReturnedQty { get; set; }

    /// <summary>Ghi chú trên dòng (ví dụ: tình trạng đặc biệt khi giao).</summary>
    public string? Note { get; set; }

    // ---------- Navigation properties ----------

    /// <summary>Phiếu mượn cha (nạp khi JOIN).</summary>
    public BorrowReceipt? BorrowReceipt { get; set; }

    /// <summary>Đối tượng sách được mượn (nạp khi JOIN).</summary>
    public Book? Book { get; set; }

    // ---------- Computed properties ----------

    /// <summary>
    /// Số bản còn chưa trả của dòng này.
    /// </summary>
    public int RemainingQty => Quantity - ReturnedQty;

    /// <summary>
    /// Cho biết dòng đã được trả hết hay chưa.
    /// </summary>
    public bool IsFullyReturned => ReturnedQty >= Quantity;
}