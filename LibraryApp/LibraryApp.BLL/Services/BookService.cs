using LibraryApp.BLL.Common;
using LibraryApp.BLL.Interfaces;
using LibraryApp.BLL.Validation;

using LibraryApp.DAL.Common;
using LibraryApp.DAL.Interfaces;

using LibraryApp.Models;
using LibraryApp.Models.Enums;

using System.Data;

namespace LibraryApp.BLL.Services;

/// <summary>
/// Service quản lý sách. Bao gồm validate đầu vào, check trùng mã, và bao bọc
/// các lỗi từ stored procedure (lỗi 5001x) thành <see cref="BusinessException"/>
/// có thông báo tiếng Việt thân thiện.
/// </summary>
public sealed class BookService : IBookService
{
    private readonly IBookRepository _bookRepo;
    private readonly ICategoryRepository _categoryRepo;

    /// <summary>
    /// Khởi tạo với 2 repository qua DI. <paramref name="categoryRepo"/> dùng
    /// để validate <c>CategoryId</c> trỏ tới danh mục thực sự tồn tại.
    /// </summary>
    public BookService(IBookRepository bookRepo, ICategoryRepository categoryRepo)
    {
        _bookRepo = bookRepo ?? throw new ArgumentNullException(nameof(bookRepo));
        _categoryRepo = categoryRepo ?? throw new ArgumentNullException(nameof(categoryRepo));
    }

    /// <inheritdoc/>
    public List<Book> GetAll() => _bookRepo.GetAll();

    /// <inheritdoc/>
    public Book? GetById(int id)
    {
        Validator.Positive(id, "ID sách");
        return _bookRepo.GetById(id);
    }

    /// <inheritdoc/>
    public List<Book> Search(string? keyword, int? categoryId, BookStatus? status,
        int? yearFrom, int? yearTo)
    {
        ValidateYearRange(yearFrom, yearTo);
        return _bookRepo.Search(keyword?.Trim(), categoryId, status, yearFrom, yearTo);
    }

    /// <inheritdoc/>
    public DataTable SearchAsDataTable(string? keyword, int? categoryId, BookStatus? status,
        int? yearFrom, int? yearTo)
    {
        ValidateYearRange(yearFrom, yearTo);
        return _bookRepo.SearchAsDataTable(keyword?.Trim(), categoryId, status, yearFrom, yearTo);
    }

    /// <inheritdoc/>
    public int Create(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);
        ValidateForCreate(book);

        // Check trùng mã
        if (_bookRepo.ExistsByCode(book.BookCode))
            throw new BusinessException(nameof(book.BookCode),
                $"Mã sách '{book.BookCode}' đã tồn tại.");

        // Check danh mục tồn tại
        if (_categoryRepo.GetById(book.CategoryId) is null)
            throw new BusinessException(nameof(book.CategoryId),
                "Danh mục không tồn tại hoặc đã bị xoá.");

        try
        {
            return _bookRepo.Insert(book);
        }
        catch (DalException ex) when (ex.SqlErrorNumber == 50010)
        {
            throw new BusinessException(ex.Message, ex);
        }
    }

    /// <inheritdoc/>
    public void Update(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);
        Validator.Positive(book.BookId, "ID sách");
        ValidateForUpdate(book);

        if (_categoryRepo.GetById(book.CategoryId) is null)
            throw new BusinessException(nameof(book.CategoryId),
                "Danh mục không tồn tại.");

        try
        {
            _bookRepo.Update(book);
        }
        catch (DalException ex) when (ex.SqlErrorNumber is 50011 or 50012 or 50013)
        {
            // 50011: không tìm thấy / 50012: bị người khác sửa / 50013: số lượng nhỏ hơn số đang mượn
            throw new BusinessException(ex.Message, ex);
        }
    }

    /// <inheritdoc/>
    public void Delete(int bookId)
    {
        Validator.Positive(bookId, "ID sách");

        try
        {
            _bookRepo.Delete(bookId);
        }
        catch (DalException ex) when (ex.SqlErrorNumber is 50014 or 50015)
        {
            throw new BusinessException(ex.Message, ex);
        }
    }

    // ----------------------------------------------------------------
    // Validate
    // ----------------------------------------------------------------

    private static void ValidateForCreate(Book b)
    {
        Validator.NotEmpty(b.BookCode, "Mã sách");
        Validator.Length(b.BookCode, "Mã sách", 2, 30);
        Validator.NotEmpty(b.Title, "Tên sách");
        Validator.Length(b.Title, "Tên sách", 1, 200);
        Validator.NotEmpty(b.Author, "Tác giả");
        Validator.Length(b.Author, "Tác giả", 1, 150);
        Validator.MaxLength(b.Publisher, "Nhà xuất bản", 150);
        Validator.PublishYear(b.PublishYear);
        Validator.Positive(b.CategoryId, "Danh mục");
        Validator.NonNegative(b.Quantity, "Số lượng");
        Validator.NonNegative(b.Price, "Giá");
    }

    private static void ValidateForUpdate(Book b)
    {
        Validator.NotEmpty(b.Title, "Tên sách");
        Validator.Length(b.Title, "Tên sách", 1, 200);
        Validator.NotEmpty(b.Author, "Tác giả");
        Validator.Length(b.Author, "Tác giả", 1, 150);
        Validator.MaxLength(b.Publisher, "Nhà xuất bản", 150);
        Validator.PublishYear(b.PublishYear);
        Validator.Positive(b.CategoryId, "Danh mục");
        Validator.NonNegative(b.Quantity, "Số lượng");
        Validator.NonNegative(b.Price, "Giá");
    }

    private static void ValidateYearRange(int? from, int? to)
    {
        if (from.HasValue) Validator.PublishYear(from, "Năm từ");
        if (to.HasValue) Validator.PublishYear(to, "Năm đến");
        if (from.HasValue && to.HasValue && from > to)
            throw new BusinessException("Năm bắt đầu không được lớn hơn năm kết thúc.");
    }
}
