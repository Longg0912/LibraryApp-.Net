namespace LibraryApp.Models.Enums;

/// <summary>
/// Tình trạng một bản sao sách tại thời điểm độc giả trả lại.
/// Mapping với cột <c>ReturnReceiptDetails.Condition</c>.
/// </summary>
public enum BookCondition
{
    /// <summary>Sách còn tốt, không phát sinh chi phí phạt.</summary>
    Good,

    /// <summary>Sách bị hư hỏng nhẹ, có thể phát sinh phí phạt.</summary>
    Damaged,

    /// <summary>Sách bị mất, độc giả phải đền bù toàn bộ giá trị.</summary>
    Lost
}