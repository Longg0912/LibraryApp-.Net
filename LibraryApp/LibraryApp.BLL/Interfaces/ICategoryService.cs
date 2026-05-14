using LibraryApp.Models;

namespace LibraryApp.BLL.Interfaces;

/// <summary>Service quản lý danh mục sách.</summary>
public interface ICategoryService
{
    /// <summary>Lấy toàn bộ danh mục (kể cả không kích hoạt).</summary>
    List<Category> GetAll();

    /// <summary>Lấy danh mục đang hoạt động — dùng cho ComboBox trên form sách.</summary>
    List<Category> GetActive();

    /// <summary>Lấy chi tiết một danh mục theo ID.</summary>
    Category? GetById(int id);

    /// <summary>Thêm danh mục mới. Tự validate và check trùng mã.</summary>
    int Create(Category category);

    /// <summary>Cập nhật danh mục.</summary>
    void Update(Category category);

    /// <summary>Xoá mềm danh mục. Ném <c>BusinessException</c> nếu còn sách tham chiếu.</summary>
    void Delete(int categoryId);
}
