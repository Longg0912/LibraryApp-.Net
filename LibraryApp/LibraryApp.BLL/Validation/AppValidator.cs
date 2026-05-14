using System.Text.RegularExpressions;
using LibraryApp.BLL.Common;

namespace LibraryApp.BLL.Validation;

/// <summary>
/// Các phương thức tiện ích validate dùng chung cho mọi service.
/// Tất cả method ném <see cref="BusinessException"/> nếu phát hiện lỗi —
/// gọi từ service chỉ cần 1 dòng, không phải viết <c>if</c> lặp đi lặp lại.
/// </summary>
public static class Validator
{
    private static readonly Regex EmailPattern = new(
        @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
        RegexOptions.Compiled);

    private static readonly Regex PhonePattern = new(
        @"^[0-9+]{8,15}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Đảm bảo chuỗi không null và không chỉ chứa khoảng trắng.
    /// </summary>
    public static void NotEmpty(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BusinessException(fieldName, $"{fieldName} không được để trống.");
    }

    /// <summary>
    /// Kiểm tra độ dài chuỗi sau khi đã trim. Tối thiểu <paramref name="min"/>,
    /// tối đa <paramref name="max"/> ký tự.
    /// </summary>
    public static void Length(string? value, string fieldName, int min, int max)
    {
        var trimmed = (value ?? string.Empty).Trim();
        if (trimmed.Length < min)
            throw new BusinessException(fieldName, $"{fieldName} phải có ít nhất {min} ký tự.");
        if (trimmed.Length > max)
            throw new BusinessException(fieldName, $"{fieldName} không được vượt quá {max} ký tự.");
    }

    /// <summary>
    /// Kiểm tra giới hạn trên cho độ dài (không yêu cầu tối thiểu).
    /// </summary>
    public static void MaxLength(string? value, string fieldName, int max)
    {
        if (value is not null && value.Length > max)
            throw new BusinessException(fieldName, $"{fieldName} không được vượt quá {max} ký tự.");
    }

    /// <summary>
    /// Số nguyên phải nằm trong khoảng <paramref name="min"/>..<paramref name="max"/>.
    /// </summary>
    public static void Range(int value, string fieldName, int min, int max)
    {
        if (value < min || value > max)
            throw new BusinessException(fieldName,
                $"{fieldName} phải nằm trong khoảng từ {min} đến {max}.");
    }

    /// <summary>Số nguyên phải lớn hơn hoặc bằng 0.</summary>
    public static void NonNegative(int value, string fieldName)
    {
        if (value < 0)
            throw new BusinessException(fieldName, $"{fieldName} không được âm.");
    }

    /// <summary>Decimal phải lớn hơn hoặc bằng 0.</summary>
    public static void NonNegative(decimal value, string fieldName)
    {
        if (value < 0)
            throw new BusinessException(fieldName, $"{fieldName} không được âm.");
    }

    /// <summary>Số nguyên phải lớn hơn 0.</summary>
    public static void Positive(int value, string fieldName)
    {
        if (value <= 0)
            throw new BusinessException(fieldName, $"{fieldName} phải lớn hơn 0.");
    }

    /// <summary>
    /// <paramref name="endDate"/> không được sớm hơn <paramref name="startDate"/>.
    /// </summary>
    public static void DateRange(DateOnly startDate, DateOnly endDate,
        string startFieldName, string endFieldName)
    {
        if (endDate < startDate)
            throw new BusinessException(endFieldName,
                $"{endFieldName} không được sớm hơn {startFieldName}.");
    }

    /// <summary>
    /// Email hợp lệ (nếu có nhập). Bỏ qua kiểm tra nếu <paramref name="value"/> null/rỗng.
    /// </summary>
    public static void Email(string? value, string fieldName = "Email")
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        if (!EmailPattern.IsMatch(value))
            throw new BusinessException(fieldName, $"{fieldName} không đúng định dạng.");
    }

    /// <summary>
    /// Số điện thoại hợp lệ (8-15 ký tự, chỉ chứa chữ số và dấu +).
    /// </summary>
    public static void Phone(string? value, string fieldName = "Số điện thoại")
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        if (!PhonePattern.IsMatch(value))
            throw new BusinessException(fieldName,
                $"{fieldName} chỉ được chứa chữ số và dấu +, dài 8-15 ký tự.");
    }

    /// <summary>
    /// Năm xuất bản hợp lệ (nullable).
    /// </summary>
    public static void PublishYear(int? year, string fieldName = "Năm xuất bản")
    {
        if (!year.HasValue) return;
        if (year.Value < 1500 || year.Value > DateTime.Today.Year + 1)
            throw new BusinessException(fieldName,
                $"{fieldName} phải nằm trong khoảng 1500 đến {DateTime.Today.Year + 1}.");
    }

    /// <summary>
    /// Mật khẩu phải đủ mạnh: tối thiểu 6 ký tự, có ít nhất 1 chữ và 1 số.
    /// </summary>
    public static void Password(string? value, string fieldName = "Mật khẩu")
    {
        NotEmpty(value, fieldName);
        if (value!.Length < 6)
            throw new BusinessException(fieldName, $"{fieldName} phải có ít nhất 6 ký tự.");
        if (!value.Any(char.IsLetter) || !value.Any(char.IsDigit))
            throw new BusinessException(fieldName,
                $"{fieldName} phải chứa cả chữ cái và chữ số.");
    }
}
