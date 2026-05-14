namespace LibraryApp.BLL.Common;

/// <summary>
/// Exception cho các lỗi nghiệp vụ phát sinh trong tầng BLL.
/// <para>
/// Phân biệt rõ với <c>DalException</c> (lỗi kỹ thuật ở tầng DAL):
/// <see cref="BusinessException"/> là lỗi "đúng quy trình nhưng vi phạm quy tắc"
/// — ví dụ "thẻ độc giả đã hết hạn", "vượt số sách tối đa được mượn", "mật khẩu sai".
/// Tầng UI có thể hiển thị trực tiếp <see cref="Exception.Message"/> cho người dùng.
/// </para>
/// </summary>
public sealed class BusinessException : Exception
{
    /// <summary>
    /// Tên trường bị lỗi (nếu lỗi do validate đầu vào). Có thể <c>null</c> khi
    /// lỗi không gắn với trường cụ thể (ví dụ: vi phạm logic tổng thể).
    /// </summary>
    public string? FieldName { get; }

    /// <summary>Khởi tạo exception với thông báo.</summary>
    public BusinessException(string message) : base(message) { }

    /// <summary>Khởi tạo exception gắn với một field cụ thể.</summary>
    public BusinessException(string fieldName, string message) : base(message)
    {
        FieldName = fieldName;
    }

    /// <summary>Khởi tạo exception kèm inner exception.</summary>
    public BusinessException(string message, Exception innerException)
        : base(message, innerException) { }
}
