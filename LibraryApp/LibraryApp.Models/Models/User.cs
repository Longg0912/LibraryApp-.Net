namespace LibraryApp.Models;

/// <summary>
/// Tài khoản người dùng nội bộ của thư viện (quản trị viên, thủ thư...).
/// Mapping với bảng <c>dbo.Users</c>.
/// </summary>
/// <remarks>
/// Lưu ý bảo mật: thuộc tính <see cref="PasswordHash"/> luôn lưu giá trị đã hash
/// (khuyến nghị BCrypt) — KHÔNG bao giờ chứa mật khẩu thô. Lớp này không cung cấp
/// constructor nhận mật khẩu thô để tránh vô tình lưu plain text.
/// </remarks>
public sealed class User : BaseAuditableEntity
{
    /// <summary>
    /// Khoá chính của tài khoản. Mapping với cột <c>UserId</c>.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Tên đăng nhập, duy nhất trong toàn hệ thống. Không phân biệt hoa thường
    /// nhưng nên lưu dưới dạng chữ thường để nhất quán.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Mật khẩu đã được hash (mặc định bằng BCrypt). KHÔNG bao giờ là plain text.
    /// </summary>
    public required string PasswordHash { get; set; }

    /// <summary>
    /// Salt riêng nếu thuật toán hash tách salt khỏi hash. Với BCrypt, salt được
    /// nhúng sẵn trong chuỗi hash nên trường này thường để <c>null</c>.
    /// </summary>
    public string? PasswordSalt { get; set; }

    /// <summary>
    /// Họ và tên đầy đủ hiển thị trên giao diện (thanh trạng thái, log...).
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>Email liên hệ.</summary>
    public string? Email { get; set; }

    /// <summary>Số điện thoại liên hệ.</summary>
    public string? Phone { get; set; }

    /// <summary>Khoá ngoại tới <see cref="Role"/>.</summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Tài khoản có đang được phép đăng nhập hay không. Khi <c>false</c>,
    /// thủ tục <c>sp_User_Login</c> sẽ trả mã <c>4 (Inactive)</c>.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Số lần đăng nhập sai liên tiếp. Đạt ngưỡng cấu hình sẽ bị khoá tạm thời
    /// (xem <see cref="LockoutUntil"/>).
    /// </summary>
    public int FailedLoginCount { get; set; }

    /// <summary>
    /// Thời điểm hết khoá tài khoản (giờ UTC). <c>null</c> nếu không bị khoá.
    /// </summary>
    public DateTime? LockoutUntil { get; set; }

    /// <summary>Thời điểm đăng nhập gần nhất.</summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>Địa chỉ IP của lần đăng nhập gần nhất.</summary>
    public string? LastLoginIp { get; set; }

    /// <summary>Thời điểm đổi mật khẩu gần nhất.</summary>
    public DateTime? PasswordChangedAt { get; set; }

    /// <summary>
    /// Cờ buộc đổi mật khẩu khi đăng nhập lần tới (thường set khi admin reset password).
    /// </summary>
    public bool MustChangePassword { get; set; }

    // ---------- Navigation properties ----------

    /// <summary>
    /// Đối tượng vai trò tương ứng. Được nạp khi truy vấn JOIN.
    /// Không serialize xuống database — chỉ dùng để binding UI.
    /// </summary>
    public Role? Role { get; set; }

    /// <summary>
    /// Cho biết tài khoản có đang trong trạng thái khoá tại thời điểm hiện tại hay không.
    /// </summary>
    public bool IsCurrentlyLockedOut =>
        LockoutUntil.HasValue && LockoutUntil.Value > DateTime.UtcNow;

    /// <summary>
    /// Hiển thị họ tên kèm tên đăng nhập trên ComboBox/Label.
    /// </summary>
    public override string ToString() => $"{FullName} ({Username})";
}