using System.Data;
using LibraryApp.Models;
using LibraryApp.Models.Enums;

namespace LibraryApp.DAL.Interfaces;

/// <summary>
/// Repository cho entity <see cref="Book"/>. Bổ sung các thao tác tìm kiếm
/// nâng cao và trả về <see cref="DataTable"/> phục vụ <c>DataGridView</c> tốc độ cao.
/// </summary>
public interface IBookRepository : IRepository<Book, int>
{
    /// <summary>
    /// Tìm kiếm sách theo nhiều tiêu chí. Mọi tham số đều có thể là <c>null</c>
    /// (bỏ qua điều kiện).
    /// </summary>
    /// <param name="keyword">Từ khoá tìm trong <c>Title</c>, <c>Author</c>, <c>BookCode</c>.</param>
    /// <param name="categoryId">Chỉ lấy sách thuộc danh mục này.</param>
    /// <param name="status">Lọc theo trạng thái.</param>
    /// <param name="yearFrom">Năm xuất bản từ.</param>
    /// <param name="yearTo">Năm xuất bản đến.</param>
    List<Book> Search(string? keyword, int? categoryId, BookStatus? status, int? yearFrom, int? yearTo);

    /// <summary>
    /// Phiên bản <see cref="Search"/> trả về <see cref="DataTable"/> để bind trực tiếp
    /// vào <c>DataGridView</c>. Nhanh hơn ~30% so với <see cref="List{T}"/> khi dataset lớn
    /// vì không phải tạo object trung gian.
    /// </summary>
    DataTable SearchAsDataTable(string? keyword, int? categoryId, BookStatus? status, int? yearFrom, int? yearTo);

    /// <summary>
    /// Kiểm tra mã sách đã tồn tại hay chưa (dùng cho form thêm mới).
    /// </summary>
    bool ExistsByCode(string bookCode);
}
