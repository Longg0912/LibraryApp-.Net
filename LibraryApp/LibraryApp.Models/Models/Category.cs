namespace LibraryApp.Models;

/// <summary>
/// Danh mục phân loại sách (CNTT, Kinh tế, Văn học...).
/// Một <see cref="Book"/> thuộc đúng một <see cref="Category"/>.
/// Mapping với bảng <c>dbo.Categories</c>.
/// </summary>
public sealed class Category : BaseAuditableEntity
{
    /// <summary>
    /// Khoá chính của danh mục. Mapping với cột <c>CategoryId</c>.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Mã danh mục ngắn gọn, duy nhất, dùng cho mã sách và báo cáo
    /// (ví dụ: <c>CNTT</c>, <c>KT</c>, <c>VH</c>).
    /// </summary>
    public required string CategoryCode { get; set; }

    /// <summary>
    /// Tên hiển thị của danh mục (ví dụ: "Công nghệ thông tin").
    /// </summary>
    public required string CategoryName { get; set; }

    /// <summary>
    /// Mô tả chi tiết về phạm vi danh mục.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Có cho phép gán sách mới vào danh mục này hay không. Khi <c>false</c>,
    /// danh mục coi như đã ngừng sử dụng nhưng dữ liệu cũ vẫn được giữ nguyên.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Hiển thị tên danh mục khi binding với <c>ComboBox</c> trên form sách.
    /// </summary>
    public override string ToString() => CategoryName;
}