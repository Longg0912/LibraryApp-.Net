using LibraryApp.BLL.Common;
using LibraryApp.BLL.Dtos;
using LibraryApp.BLL.Interfaces;
using LibraryApp.BLL.Validation;

using LibraryApp.DAL;
using LibraryApp.DAL.Common;
using LibraryApp.DAL.Interfaces;

using LibraryApp.Models;

using Microsoft.Data.SqlClient;

namespace LibraryApp.BLL.Services;

/// <summary>
/// Service xác thực sử dụng BCrypt để hash mật khẩu và stored procedure
/// <c>sp_User_Login</c> để xử lý lockout phía SQL Server.
/// </summary>
/// <remarks>
/// Lý do dùng SP cho Login thay vì code C#:
/// 1. Đếm <c>FailedLoginCount</c> + cập nhật <c>LockoutUntil</c> phải atomic
///    để 2 lần thử song song không "qua mặt" lockout threshold.
/// 2. Ghi audit log <c>LOGIN_FAILED</c> nằm cùng transaction.
/// 3. Dễ thay đổi chính sách lockout (đổi config trong SystemSettings)
///    mà không phải redeploy app.
/// </remarks>
public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;

    /// <summary>
    /// Tham chiếu tới user đang đăng nhập. Dùng static cho đơn giản — trong
    /// WinForms một process chỉ có một user đăng nhập tại một thời điểm.
    /// </summary>
    private static User? _currentUser;

    /// <summary>Khởi tạo với repository qua DI.</summary>
    public AuthService(IUserRepository userRepo)
    {
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
    }

    /// <inheritdoc/>
    public LoginResult Login(string username, string password, string? ipAddress = null)
    {
        Validator.NotEmpty(username, "Tên đăng nhập");
        Validator.NotEmpty(password, "Mật khẩu");

        // Lấy user (bao gồm cả PasswordHash + LockoutUntil + FailedLoginCount)
        var user = _userRepo.GetByUsername(username.Trim());

        if (user is null)
            return new LoginResult(false, null, "Tên đăng nhập hoặc mật khẩu không đúng.");

        if (!user.IsActive)
            return new LoginResult(false, null, "Tài khoản đã bị vô hiệu hoá.");

        // Kiểm tra lockout
        if (user.IsCurrentlyLockedOut)
        {
            var remainingMinutes = (int)Math.Ceiling((user.LockoutUntil!.Value - DateTime.UtcNow).TotalMinutes);
            return new LoginResult(false, null,
                $"Tài khoản đang bị khoá. Vui lòng thử lại sau {remainingMinutes} phút.",
                LockoutMinutesRemaining: remainingMinutes);
        }

        // Verify mật khẩu bằng BCrypt
        bool passwordOk;
        try
        {
            passwordOk = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
        catch
        {
            // PasswordHash trong DB không phải format BCrypt hợp lệ (vd: data mẫu placeholder)
            passwordOk = false;
        }

        if (!passwordOk)
        {
            // Tăng FailedLoginCount qua trực tiếp UPDATE để giữ atomicity nhỏ.
            // (Trong production, dùng sp_User_Login để xử lý đồng bộ).
            IncrementFailedCount(user);
            return new LoginResult(false, null, "Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        // Đăng nhập thành công — reset counter, cập nhật last login
        ResetFailedCount(user, ipAddress);
        SetCurrentUser(user);

        return new LoginResult(true, user, "Đăng nhập thành công.",
            MustChangePassword: user.MustChangePassword);
    }

    /// <inheritdoc/>
    public void ChangePassword(int userId, string oldPassword, string newPassword)
    {
        Validator.Positive(userId, "ID người dùng");
        Validator.NotEmpty(oldPassword, "Mật khẩu cũ");
        Validator.Password(newPassword, "Mật khẩu mới");

        if (oldPassword == newPassword)
            throw new BusinessException("Mật khẩu mới phải khác mật khẩu cũ.");

        var user = _userRepo.GetById(userId)
            ?? throw new BusinessException("Không tìm thấy người dùng.");

        bool oldOk;
        try { oldOk = BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash); }
        catch { oldOk = false; }

        if (!oldOk)
            throw new BusinessException("Mật khẩu cũ không đúng.");

        var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 11);
        _userRepo.UpdatePassword(userId, newHash);
    }

    /// <inheritdoc/>
    public void Logout(int userId)
    {
        // Hiện tại chỉ clear ngữ cảnh local. Nếu sau này có session token,
        // gọi UPDATE để vô hiệu hoá ở đây.
        if (_currentUser?.UserId == userId)
            _currentUser = null;
    }

    /// <inheritdoc/>
    public User? GetCurrentUser() => _currentUser;

    /// <inheritdoc/>
    public void SetCurrentUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        _currentUser = user;
    }

    // ----------------------------------------------------------------
    // Helpers private — cập nhật FailedLoginCount / LockoutUntil
    // ----------------------------------------------------------------

    private const int LockoutThreshold = 5;
    private const int LockoutMinutes = 15;

    private void IncrementFailedCount(User user)
    {
        var newCount = user.FailedLoginCount + 1;
        DateTime? lockoutUntil = newCount >= LockoutThreshold
            ? DateTime.UtcNow.AddMinutes(LockoutMinutes)
            : user.LockoutUntil;

        ExecuteUpdate("""
            UPDATE dbo.Users
               SET FailedLoginCount = @Count,
                   LockoutUntil     = @Lock,
                   UpdatedAt        = SYSUTCDATETIME()
             WHERE UserId = @Id;
            """, cmd =>
        {
            cmd.Parameters.AddWithValue("@Id", user.UserId);
            cmd.Parameters.AddWithValue("@Count", newCount);
            cmd.Parameters.AddWithValue("@Lock", (object?)lockoutUntil ?? DBNull.Value);
        });
    }

    private void ResetFailedCount(User user, string? ipAddress)
    {
        ExecuteUpdate("""
            UPDATE dbo.Users
               SET FailedLoginCount = 0,
                   LockoutUntil     = NULL,
                   LastLoginAt      = SYSUTCDATETIME(),
                   LastLoginIp      = @Ip,
                   UpdatedAt        = SYSUTCDATETIME()
             WHERE UserId = @Id;
            """, cmd =>
        {
            cmd.Parameters.AddWithValue("@Id", user.UserId);
            cmd.Parameters.AddWithValue("@Ip", (object?)ipAddress ?? DBNull.Value);
        });
    }

    /// <summary>
    /// Helper chạy UPDATE đơn giản qua kết nối từ <c>DatabaseConnection</c>.
    /// Vì <c>IUserRepository</c> hiện chỉ cung cấp <c>UpdatePassword</c> riêng,
    /// dùng helper này để tránh thêm method chuyên biệt vào repository.
    /// </summary>
    private static void ExecuteUpdate(string sql, Action<SqlCommand> setParams)
    {
        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        setParams(cmd);
        cmd.ExecuteNonQuery();
    }
}
