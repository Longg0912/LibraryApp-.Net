using LibraryApp.BLL.Dtos;
using LibraryApp.Models;

namespace LibraryApp.BLL.Interfaces;

/// <summary>
/// Dịch vụ xác thực và quản lý phiên đăng nhập.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Đăng nhập với username/password. Tự kiểm tra khoá tài khoản, hash mật khẩu,
    /// cập nhật <c>LastLoginAt</c>.
    /// </summary>
    LoginResult Login(string username, string password, string? ipAddress = null);

    /// <summary>
    /// Đổi mật khẩu cho user hiện tại. Mật khẩu cũ phải đúng và mật khẩu mới
    /// phải đạt yêu cầu phức tạp.
    /// </summary>
    void ChangePassword(int userId, string oldPassword, string newPassword);

    /// <summary>
    /// Đăng xuất — hiện tại chỉ ghi log; nếu sau này có session token thì xoá ở đây.
    /// </summary>
    void Logout(int userId);

    /// <summary>
    /// Lấy thông tin user hiện đang đăng nhập (cho thanh trạng thái form chính).
    /// </summary>
    User? GetCurrentUser();

    /// <summary>
    /// Lưu thông tin user vào ngữ cảnh "current user" của ứng dụng,
    /// thường gọi sau khi Login thành công.
    /// </summary>
    void SetCurrentUser(User user);
}
