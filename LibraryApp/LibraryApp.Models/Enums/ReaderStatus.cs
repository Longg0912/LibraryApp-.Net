namespace LibraryApp.Models.Enums;

/// <summary>
/// Trạng thái thẻ độc giả. Mapping với cột <c>Readers.Status</c>.
/// </summary>
public enum ReaderStatus
{
    /// <summary>Thẻ còn hiệu lực, độc giả có thể mượn sách bình thường.</summary>
    Active,

    /// <summary>Thẻ đang bị khoá (do vi phạm, nợ phạt, v.v.).</summary>
    Locked,

    /// <summary>Thẻ đã hết hạn sử dụng.</summary>
    Expired
}