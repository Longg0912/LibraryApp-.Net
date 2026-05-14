using LibraryApp.Models;

namespace LibraryApp.UI.Common;

/// <summary>
/// Lưu trữ ngữ cảnh ứng dụng (user hiện đang đăng nhập, vai trò...).
/// Static class cho đơn giản — WinForms một process chỉ có một user đăng nhập.
/// </summary>
/// <remarks>
/// Khác với <c>AuthService.GetCurrentUser()</c> ở chỗ:
/// <c>AuthService</c> giữ user cho mục đích nghiệp vụ (login/logout);
/// <see cref="CurrentSession"/> này tiện cho UI đọc nhanh
/// (hiển thị tên trên statusbar, kiểm tra quyền để ẩn/hiện menu...).
/// </remarks>
public static class CurrentSession
{
    /// <summary>
    /// User đang đăng nhập, <c>null</c> nếu chưa login.
    /// </summary>
    public static User? CurrentUser { get; private set; }

    /// <summary>
    /// Mã vai trò của user hiện tại
    /// (ví dụ: <c>ADMIN</c>, <c>LIBRARIAN</c>).
    /// </summary>
    public static string? CurrentRoleCode { get; private set; }

    /// <summary>
    /// Cho biết user hiện tại có phải Admin hay không.
    /// </summary>
    public static bool IsAdmin =>
        string.Equals(CurrentRoleCode,
            "ADMIN",
            StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Cho biết user hiện tại có phải Thủ thư hay không.
    /// </summary>
    public static bool IsLibrarian =>
        string.Equals(CurrentRoleCode,
            "LIBRARIAN",
            StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Cho biết đã có user đăng nhập hay chưa.
    /// </summary>
    public static bool IsAuthenticated =>
        CurrentUser is not null;

    /// <summary>
    /// Đặt user đã đăng nhập vào ngữ cảnh.
    /// Gọi sau khi <c>AuthService.Login</c> thành công.
    /// </summary>
    /// <param name="user">
    /// User đăng nhập thành công.
    /// </param>
    /// <param name="roleCode">
    /// Mã vai trò của user.
    /// </param>
    public static void SignIn(User user, string roleCode)
    {
        ArgumentNullException.ThrowIfNull(user);

        CurrentUser = user;
        CurrentRoleCode = roleCode;
    }

    /// <summary>
    /// Xoá ngữ cảnh đăng nhập
    /// (khi logout hoặc đóng form chính).
    /// </summary>
    public static void SignOut()
    {
        CurrentUser = null;
        CurrentRoleCode = null;
    }
}