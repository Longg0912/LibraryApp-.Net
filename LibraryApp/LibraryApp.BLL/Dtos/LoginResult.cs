using LibraryApp.Models;

namespace LibraryApp.BLL.Dtos;

/// <summary>
/// Kết quả của thao tác đăng nhập, trả về cho tầng UI để quyết định
/// hành vi tiếp theo (mở form chính, hiển thị lỗi, yêu cầu đổi mật khẩu...).
/// </summary>
/// <param name="Success">Đăng nhập thành công hay không.</param>
/// <param name="User">Thông tin user nếu thành công, <c>null</c> nếu thất bại.</param>
/// <param name="Message">Thông báo hiển thị cho người dùng.</param>
/// <param name="MustChangePassword">
/// <c>true</c> nếu user bắt buộc đổi mật khẩu khi đăng nhập (thường vì admin reset).
/// </param>
/// <param name="LockoutMinutesRemaining">
/// Số phút còn lại cho đến khi tài khoản hết khoá, có giá trị khi tài khoản đang bị khoá.
/// </param>
public sealed record LoginResult(
    bool Success,
    User? User,
    string Message,
    bool MustChangePassword = false,
    int? LockoutMinutesRemaining = null);
