using System.Data;
using LibraryApp.Models;

namespace LibraryApp.BLL.Interfaces;

/// <summary>
/// Service quản lý người dùng (nhân viên) — chỉ Admin được dùng.
/// Tách khỏi <see cref="IAuthService"/> (login/changepassword) để rõ phân chia trách nhiệm.
/// </summary>
public interface IUserService
{
    /// <summary>Tìm kiếm user theo từ khoá + vai trò + trạng thái.</summary>
    DataTable SearchAsDataTable(string? keyword, int? roleId, bool? isActive);

    /// <summary>Lấy chi tiết user theo ID.</summary>
    User? GetById(int userId);

    /// <summary>Lấy toàn bộ vai trò (cho ComboBox).</summary>
    List<Role> GetAllRoles();

    /// <summary>
    /// Tạo user mới với mật khẩu tạm <paramref name="tempPassword"/>.
    /// User sẽ phải đổi mật khẩu khi đăng nhập lần đầu.
    /// </summary>
    /// <returns>UserId vừa tạo.</returns>
    int Create(string username, string fullName, string? email, string? phone,
               int roleId, string tempPassword);

    /// <summary>Cập nhật thông tin user (không bao gồm username/password).</summary>
    void Update(int userId, string fullName, string? email, string? phone, int roleId);

    /// <summary>Khóa / mở khóa tài khoản (set IsActive).</summary>
    void SetActive(int userId, bool isActive);

    /// <summary>
    /// Reset mật khẩu về một giá trị tạm (do admin cấp). Sinh ngẫu nhiên + đặt
    /// <c>MustChangePassword = 1</c> để user phải đổi khi đăng nhập.
    /// </summary>
    /// <returns>Mật khẩu tạm để admin thông báo cho user.</returns>
    string ResetPassword(int userId);

    /// <summary>Xoá mềm user (set IsDeleted=1).</summary>
    void Delete(int userId);
}
