namespace LibraryApp.BLL.Common;

/// <summary>
/// Kết quả của một thao tác validate. Chứa danh sách các lỗi gắn với từng field
/// để tầng UI có thể highlight đúng vị trí trên form (qua <c>ErrorProvider</c>).
/// </summary>
/// <remarks>
/// Có 2 cách dùng validate trong BLL:
/// <list type="number">
/// <item>Ném <c>BusinessException</c> ngay khi phát hiện lỗi đầu tiên — đơn giản nhưng
/// chỉ báo được 1 lỗi/lần.</item>
/// <item>Gom tất cả lỗi vào <see cref="ValidationResult"/> rồi trả về — UX tốt hơn
/// vì người dùng thấy hết lỗi cùng lúc.</item>
/// </list>
/// Trong dự án này, service dùng cách (1) cho các lỗi đơn lẻ và cách (2) cho form
/// thêm/sửa có nhiều field. Cả hai đều có sẵn ở đây.
/// </remarks>
public sealed class ValidationResult
{
    private readonly List<ValidationError> _errors = [];

    /// <summary>Danh sách lỗi đã thu thập (read-only).</summary>
    public IReadOnlyList<ValidationError> Errors => _errors;

    /// <summary>Validate có thành công không (không có lỗi nào).</summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>Số lượng lỗi đã thu thập.</summary>
    public int ErrorCount => _errors.Count;

    /// <summary>Thêm một lỗi vào kết quả.</summary>
    public void AddError(string fieldName, string message)
    {
        _errors.Add(new ValidationError(fieldName, message));
    }

    /// <summary>
    /// Thêm một lỗi không gắn với field cụ thể (lỗi tổng quát của form).
    /// </summary>
    public void AddError(string message)
    {
        _errors.Add(new ValidationError(string.Empty, message));
    }

    /// <summary>
    /// Nếu có lỗi, ném <see cref="BusinessException"/> với thông báo gộp.
    /// Tiện gọi ở cuối phương thức validate khi muốn dừng flow.
    /// </summary>
    public void ThrowIfInvalid()
    {
        if (IsValid) return;

        var message = string.Join(Environment.NewLine,
            _errors.Select(e => string.IsNullOrEmpty(e.FieldName)
                ? $"• {e.Message}"
                : $"• {e.FieldName}: {e.Message}"));

        throw new BusinessException(message);
    }
}

/// <summary>
/// Một lỗi validate gắn với một field cụ thể.
/// </summary>
/// <param name="FieldName">Tên field bị lỗi (rỗng nếu là lỗi chung).</param>
/// <param name="Message">Thông báo lỗi hiển thị cho người dùng.</param>
public sealed record ValidationError(string FieldName, string Message);
