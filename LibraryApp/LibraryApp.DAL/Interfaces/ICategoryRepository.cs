using LibraryApp.Models;

namespace LibraryApp.DAL.Interfaces;

/// <summary>
/// Repository cho entity <see cref="Category"/>.
/// </summary>
public interface ICategoryRepository : IRepository<Category, int>
{
    /// <summary>
    /// Lấy danh sách danh mục đang được kích hoạt (<c>IsActive = 1</c>),
    /// dùng để binding ComboBox trên form thêm/sửa sách.
    /// </summary>
    List<Category> GetActive();

    /// <summary>Kiểm tra mã danh mục đã tồn tại hay chưa.</summary>
    bool ExistsByCode(string categoryCode);
}
