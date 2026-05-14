using System.Data;
using LibraryApp.Models;
using LibraryApp.Models.Enums;

namespace LibraryApp.BLL.Interfaces;

/// <summary>Service quản lý sách.</summary>
public interface IBookService
{
    /// <summary>Lấy toàn bộ sách chưa bị xoá.</summary>
    List<Book> GetAll();

    /// <summary>Lấy chi tiết một sách theo ID.</summary>
    Book? GetById(int id);

    /// <summary>
    /// Tìm kiếm nâng cao theo nhiều tiêu chí.
    /// Mọi tham số có thể null (bỏ qua điều kiện).
    /// </summary>
    List<Book> Search(string? keyword, int? categoryId, BookStatus? status,
        int? yearFrom, int? yearTo);

    /// <summary>Tìm kiếm trả về <c>DataTable</c> để bind <c>DataGridView</c>.</summary>
    DataTable SearchAsDataTable(string? keyword, int? categoryId, BookStatus? status,
        int? yearFrom, int? yearTo);

    /// <summary>Thêm sách mới. Validate đầy đủ và check trùng mã.</summary>
    int Create(Book book);

    /// <summary>Cập nhật sách. Kiểm tra số lượng không nhỏ hơn số đang được mượn.</summary>
    void Update(Book book);

    /// <summary>Xoá mềm sách. Không cho xoá nếu đang có người mượn.</summary>
    void Delete(int bookId);
}
