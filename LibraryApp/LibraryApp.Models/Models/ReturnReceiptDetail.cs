using LibraryApp.Models.Enums;

namespace LibraryApp.Models;

/// <summary>
/// Một dòng chi tiết phiếu trả. Mỗi dòng tham chiếu ngược lại một dòng chi tiết
/// phiếu mượn (<see cref="BorrowReceiptDetail"/>) để biết đã trả cho lần mượn nào.
/// Mapping với bảng <c>dbo.ReturnReceiptDetails</c>.
/// </summary>
public sealed class ReturnReceiptDetail
{
    /// <summary>
    /// Khoá chính. Mapping với cột <c>ReturnDetailId</c>.
    /// </summary>
    public int ReturnDetailId { get; set; }

    /// <summary>Khoá ngoại tới phiếu trả cha.</summary>
    public int ReturnId { get; set; }

    /// <summary>Khoá ngoại tới dòng chi tiết phiếu mượn được trả.</summary>
    public int BorrowDetailId { get; set; }

    /// <summary>Số bản copy trả ở dòng này (> 0).</summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Tình trạng sách tại thời điểm trả. Khi là <see cref="BookCondition.Lost"/>,
    /// tồn kho sẽ KHÔNG được hoàn lại trong stored procedure trả sách.
    /// </summary>
    public BookCondition Condition { get; set; } = BookCondition.Good;

    /// <summary>
    /// Tiền phạt phát sinh cho dòng này (VNĐ). Tính dựa trên số ngày quá hạn
    /// và/hoặc giá trị bồi thường khi mất/hỏng.
    /// </summary>
    public decimal Fine { get; set; }

    /// <summary>Ghi chú trên dòng.</summary>
    public string? Note { get; set; }

    // ---------- Navigation properties ----------

    /// <summary>Phiếu trả cha (nạp khi JOIN).</summary>
    public ReturnReceipt? ReturnReceipt { get; set; }

    /// <summary>Dòng chi tiết phiếu mượn được trả (nạp khi JOIN).</summary>
    public BorrowReceiptDetail? BorrowDetail { get; set; }
}