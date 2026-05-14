using LibraryApp.BLL.Common;
using LibraryApp.BLL.Interfaces;
using LibraryApp.BLL.Validation;

using LibraryApp.DAL.Common;
using LibraryApp.DAL.Interfaces;

using LibraryApp.Models;

namespace LibraryApp.BLL.Services;

/// <summary>
/// Service quản lý danh mục sách. Đảm nhiệm validate đầu vào và bao bọc
/// các lỗi DAL thành lỗi nghiệp vụ thân thiện.
/// </summary>
public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    /// <summary>
    /// Khởi tạo service với repository được inject. Pattern này cho phép
    /// thay thế repository bằng mock khi unit test.
    /// </summary>
    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc/>
    public List<Category> GetAll() => _repository.GetAll();

    /// <inheritdoc/>
    public List<Category> GetActive() => _repository.GetActive();

    /// <inheritdoc/>
    public Category? GetById(int id)
    {
        Validator.Positive(id, "ID danh mục");
        return _repository.GetById(id);
    }

    /// <inheritdoc/>
    public int Create(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        ValidateForCreate(category);

        if (_repository.ExistsByCode(category.CategoryCode))
            throw new BusinessException(nameof(category.CategoryCode),
                $"Mã danh mục '{category.CategoryCode}' đã tồn tại.");

        return _repository.Insert(category);
    }

    /// <inheritdoc/>
    public void Update(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        Validator.Positive(category.CategoryId, "ID danh mục");
        ValidateForUpdate(category);

        if (!_repository.Update(category))
            throw new BusinessException("Không tìm thấy danh mục để cập nhật.");
    }

    /// <inheritdoc/>
    public void Delete(int categoryId)
    {
        Validator.Positive(categoryId, "ID danh mục");

        try
        {
            if (!_repository.Delete(categoryId))
                throw new BusinessException("Không tìm thấy danh mục để xoá.");
        }
        catch (DalException ex) when (ex.SqlErrorNumber == 50100)
        {
            // Đã được dịch ở DAL nhưng nâng lên thành BusinessException
            throw new BusinessException(ex.Message, ex);
        }
    }

    // ----------------------------------------------------------------
    // Validate riêng cho từng thao tác
    // ----------------------------------------------------------------

    private static void ValidateForCreate(Category c)
    {
        Validator.NotEmpty(c.CategoryCode, "Mã danh mục");
        Validator.Length(c.CategoryCode, "Mã danh mục", 2, 20);
        Validator.NotEmpty(c.CategoryName, "Tên danh mục");
        Validator.Length(c.CategoryName, "Tên danh mục", 2, 100);
        Validator.MaxLength(c.Description, "Mô tả", 300);
    }

    private static void ValidateForUpdate(Category c)
    {
        // Khi update không cho sửa CategoryCode → chỉ validate name + desc
        Validator.NotEmpty(c.CategoryName, "Tên danh mục");
        Validator.Length(c.CategoryName, "Tên danh mục", 2, 100);
        Validator.MaxLength(c.Description, "Mô tả", 300);
    }
}
