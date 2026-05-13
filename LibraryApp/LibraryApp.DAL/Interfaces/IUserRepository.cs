using LibraryApp.Models;

namespace LibraryApp.DAL.Interfaces;

/// <summary>
/// Repository cho entity <see cref="User"/>. Thao tác đăng nhập, khoá tài khoản
/// thường được thực hiện qua stored procedure trong tầng cao hơn (AuthService).
/// </summary>
public interface IUserRepository : IRepository<User, int>
{
    /// <summary>Tìm user theo username (dùng cho đăng nhập).</summary>
    User? GetByUsername(string username);

    /// <summary>Tìm kiếm user theo từ khoá (username / họ tên).</summary>
    List<User> Search(string? keyword, int? roleId, bool? isActive);

    /// <summary>Kiểm tra username đã tồn tại hay chưa.</summary>
    bool ExistsByUsername(string username);

    /// <summary>
    /// Cập nhật mật khẩu (đã hash) của user. Cập nhật cả <c>PasswordChangedAt</c>.
    /// </summary>
    bool UpdatePassword(int userId, string newPasswordHash);
}
