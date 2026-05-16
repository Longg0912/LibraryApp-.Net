using LibraryApp.BLL.Interfaces;
using LibraryApp.BLL.Services;
using LibraryApp.DAL.Interfaces;
using LibraryApp.DAL.Repositories;

namespace LibraryApp.UI.Common;

/// <summary>
/// Service Locator đơn giản — khởi tạo một lần các repository và service
/// rồi cung cấp cho các Form sử dụng.
/// </summary>
/// <remarks>
/// Trong dự án nhỏ này, dùng Service Locator là đủ. Khi quy mô lớn hơn,
/// nên chuyển sang <c>Microsoft.Extensions.DependencyInjection</c> để hỗ trợ
/// scope và lifetime quản lý tự động.
/// </remarks>
public static class ServiceLocator
{
    // Repositories
    private static readonly ICategoryRepository _categoryRepo = new CategoryRepository();
    private static readonly IBookRepository _bookRepo = new BookRepository();
    private static readonly IReaderRepository _readerRepo = new ReaderRepository();
    private static readonly IUserRepository _userRepo = new UserRepository();
    private static readonly IBorrowReceiptRepository _borrowRepo = new BorrowReceiptRepository();

    /// <summary>Dịch vụ xác thực (login/logout/đổi mật khẩu).</summary>
    public static IAuthService Auth { get; } = new AuthService(_userRepo);

    /// <summary>Dịch vụ quản lý danh mục.</summary>
    public static ICategoryService Categories { get; } = new CategoryService(_categoryRepo);

    /// <summary>Dịch vụ quản lý sách.</summary>
    public static IBookService Books { get; } = new BookService(_bookRepo, _categoryRepo);

    /// <summary>Dịch vụ quản lý độc giả.</summary>
    public static IReaderService Readers { get; } = new ReaderService(_readerRepo);

    /// <summary>Dịch vụ mượn/trả sách.</summary>
    public static IBorrowService Borrow { get; } = new BorrowService(_borrowRepo, _readerRepo, _bookRepo);

    /// <summary>Dịch vụ báo cáo / thống kê.</summary>
    public static IReportService Reports { get; } = new ReportService();

    /// <summary>Dịch vụ quản lý người dùng (Admin only).</summary>
    public static IUserService Users { get; } = new UserService();

    /// <summary>Repo Role để load danh sách vai trò cho login.</summary>
    internal static IUserRepository UserRepo => _userRepo;
}
