namespace LibraryApp.Models;

/// <summary>
/// Vai trò người dùng trong hệ thống (Admin, Librarian, Viewer...).
/// Mỗi <see cref="User"/> thuộc đúng một <see cref="Role"/>.
/// Mapping với bảng <c>dbo.Roles</c>.
/// </summary>
public sealed class Role : BaseAuditableEntity
{
    /// <summary>
    /// Khoá chính của vai trò. Mapping với cột <c>RoleId</c>.
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Mã vai trò ngắn gọn, viết hoa, duy nhất (ví dụ: <c>ADMIN</c>, <c>LIBRARIAN</c>).
    /// Đây là khoá nghiệp vụ – dùng để check trong code thay vì hard-code <see cref="RoleId"/>.
    /// </summary>
    public required string RoleCode { get; set; }

    /// <summary>
    /// Tên vai trò hiển thị cho người dùng (ví dụ: "Quản trị viên", "Thủ thư").
    /// </summary>
    public required string RoleName { get; set; }

    /// <summary>
    /// Mô tả chi tiết về phạm vi và trách nhiệm của vai trò.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Vai trò có đang được kích hoạt hay không. Khi <c>false</c>, không thể
    /// gán cho user mới (nhưng các user hiện tại vẫn giữ vai trò cũ).
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Phục vụ binding với <c>ComboBox</c> trong WinForms (hiển thị tên vai trò).
    /// </summary>
    public override string ToString() => RoleName;
}