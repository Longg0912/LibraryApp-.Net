using System.Data;

namespace LibraryApp.DAL.Common;

/// <summary>
/// Tập hợp extension method giúp đọc giá trị từ <see cref="IDataReader"/>
/// một cách an toàn với <c>DBNull</c> và đỡ phải viết lặp code mapping.
/// </summary>
/// <remarks>
/// Lý do tồn tại: <see cref="IDataReader.GetString(int)"/>, <see cref="IDataReader.GetInt32(int)"/>...
/// sẽ ném <see cref="InvalidCastException"/> nếu cột là <c>DBNull</c>. Các method ở đây
/// kiểm tra trước rồi trả về <c>null</c> hoặc giá trị mặc định, giúp mapper gọn và
/// không bị crash khi gặp cột nullable.
/// </remarks>
public static class DbExtensions
{
    /// <summary>
    /// Đọc cột kiểu chuỗi. Trả về <c>null</c> nếu cột là <c>DBNull</c>.
    /// </summary>
    public static string? GetStringOrNull(this IDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    /// <summary>
    /// Đọc cột kiểu chuỗi và bắt buộc có giá trị. Ném <see cref="InvalidOperationException"/>
    /// nếu cột bị <c>DBNull</c> — dùng cho các cột <c>NOT NULL</c> trong schema.
    /// </summary>
    public static string GetStringRequired(this IDataReader reader, string columnName)
    {
        return reader.GetStringOrNull(columnName)
            ?? throw new InvalidOperationException($"Cột '{columnName}' bắt buộc nhưng nhận được NULL.");
    }

    /// <summary>Đọc cột <c>INT NOT NULL</c>.</summary>
    public static int GetInt32(this IDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.GetInt32(ordinal);
    }

    /// <summary>Đọc cột <c>INT NULL</c>.</summary>
    public static int? GetInt32OrNull(this IDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
    }

    /// <summary>Đọc cột <c>DECIMAL NOT NULL</c>.</summary>
    public static decimal GetDecimal(this IDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.GetDecimal(ordinal);
    }

    /// <summary>Đọc cột <c>BIT NOT NULL</c>.</summary>
    public static bool GetBoolean(this IDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.GetBoolean(ordinal);
    }

    /// <summary>Đọc cột <c>DATETIME2 NOT NULL</c>.</summary>
    public static DateTime GetDateTime(this IDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.GetDateTime(ordinal);
    }

    /// <summary>Đọc cột <c>DATETIME2 NULL</c>.</summary>
    public static DateTime? GetDateTimeOrNull(this IDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
    }

    /// <summary>
    /// Đọc cột <c>DATE NOT NULL</c> dưới dạng <see cref="DateOnly"/>
    /// (kiểu khớp đúng với <c>DATE</c> trong SQL Server, không có giờ).
    /// </summary>
    public static DateOnly GetDateOnly(this IDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        return DateOnly.FromDateTime(reader.GetDateTime(ordinal));
    }

    /// <summary>Đọc cột <c>DATE NULL</c>.</summary>
    public static DateOnly? GetDateOnlyOrNull(this IDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : DateOnly.FromDateTime(reader.GetDateTime(ordinal));
    }

    /// <summary>Đọc cột <c>ROWVERSION</c> (8 byte).</summary>
    public static byte[]? GetRowVersion(this IDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return null;
        var buffer = new byte[8];
        reader.GetBytes(ordinal, 0, buffer, 0, 8);
        return buffer;
    }

    /// <summary>
    /// Đọc một cột Enum lưu dưới dạng chuỗi (ví dụ: cột <c>Status VARCHAR(20)</c>).
    /// Ném <see cref="InvalidOperationException"/> nếu giá trị không khớp tên thành viên enum.
    /// </summary>
    public static TEnum GetEnum<TEnum>(this IDataReader reader, string columnName) where TEnum : struct, Enum
    {
        var raw = reader.GetStringRequired(columnName);
        if (Enum.TryParse<TEnum>(raw, ignoreCase: true, out var value))
            return value;
        throw new InvalidOperationException($"Giá trị '{raw}' tại cột '{columnName}' không hợp lệ với enum {typeof(TEnum).Name}.");
    }

    /// <summary>Đọc enum lưu chuỗi, cho phép NULL.</summary>
    public static TEnum? GetEnumOrNull<TEnum>(this IDataReader reader, string columnName) where TEnum : struct, Enum
    {
        var raw = reader.GetStringOrNull(columnName);
        if (raw is null) return null;
        return Enum.TryParse<TEnum>(raw, ignoreCase: true, out var value) ? value : null;
    }

    /// <summary>
    /// Chuyển <c>null</c> thành <see cref="DBNull.Value"/> để truyền vào <see cref="SqlParameter"/>.
    /// </summary>
    public static object ToDbValue(this object? value) => value ?? DBNull.Value;
}
