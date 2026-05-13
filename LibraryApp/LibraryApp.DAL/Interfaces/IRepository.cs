namespace LibraryApp.DAL.Interfaces;

/// <summary>
/// Interface chung cho tất cả repository theo CRUD pattern.
/// Mỗi entity có thể bổ sung thêm method nghiệp vụ trong interface riêng
/// (kế thừa từ interface này).
/// </summary>
/// <typeparam name="TEntity">Kiểu entity nghiệp vụ.</typeparam>
/// <typeparam name="TKey">Kiểu khoá chính (thường là <c>int</c>).</typeparam>
public interface IRepository<TEntity, in TKey> where TEntity : class
{
    /// <summary>Lấy toàn bộ bản ghi chưa bị xoá mềm.</summary>
    List<TEntity> GetAll();

    /// <summary>
    /// Lấy bản ghi theo khoá chính. Trả về <c>null</c> nếu không tìm thấy
    /// hoặc bản ghi đã bị xoá mềm.
    /// </summary>
    TEntity? GetById(TKey id);

    /// <summary>
    /// Thêm bản ghi mới. Sau khi thành công, khoá chính (auto-increment)
    /// được gán ngược trở lại entity.
    /// </summary>
    /// <returns>Khoá chính của bản ghi vừa tạo.</returns>
    TKey Insert(TEntity entity);

    /// <summary>
    /// Cập nhật bản ghi. Trả về <c>true</c> nếu có dòng được sửa,
    /// <c>false</c> nếu không tìm thấy bản ghi.
    /// </summary>
    bool Update(TEntity entity);

    /// <summary>
    /// Xoá mềm bản ghi (set <c>IsDeleted = 1</c>). Trả về <c>true</c> nếu thành công.
    /// </summary>
    bool Delete(TKey id);
}
