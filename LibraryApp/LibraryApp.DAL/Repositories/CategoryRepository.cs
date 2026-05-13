using System.Data;
using Microsoft.Data.SqlClient;
using LibraryApp.DAL.Common;
using LibraryApp.DAL.Interfaces;
using LibraryApp.DAL.Mappers;
using LibraryApp.Models;

namespace LibraryApp.DAL.Repositories;

/// <summary>
/// Repository thao tác với bảng <c>dbo.Categories</c>.
/// </summary>
public sealed class CategoryRepository : BaseRepository, ICategoryRepository
{
    /// <inheritdoc/>
    public List<Category> GetAll() => Execute(nameof(GetAll), () =>
    {
        const string sql = $"""
            SELECT {CategoryMapper.SelectColumns}
            FROM dbo.Categories
            WHERE IsDeleted = 0
            ORDER BY CategoryName;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        return ReadList(cmd, CategoryMapper.Map);
    });

    /// <inheritdoc/>
    public List<Category> GetActive() => Execute(nameof(GetActive), () =>
    {
        const string sql = $"""
            SELECT {CategoryMapper.SelectColumns}
            FROM dbo.Categories
            WHERE IsDeleted = 0 AND IsActive = 1
            ORDER BY CategoryName;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        return ReadList(cmd, CategoryMapper.Map);
    });

    /// <inheritdoc/>
    public Category? GetById(int id) => Execute(nameof(GetById), () =>
    {
        const string sql = $"""
            SELECT {CategoryMapper.SelectColumns}
            FROM dbo.Categories
            WHERE CategoryId = @Id AND IsDeleted = 0;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        using var reader = cmd.ExecuteReader();
        return reader.Read() ? CategoryMapper.Map(reader) : null;
    });

    /// <inheritdoc/>
    public int Insert(Category entity) => Execute(nameof(Insert), () =>
    {
        const string sql = """
            INSERT INTO dbo.Categories
                (CategoryCode, CategoryName, Description, IsActive, CreatedBy)
            OUTPUT INSERTED.CategoryId
            VALUES
                (@Code, @Name, @Desc, @Active, @CreatedBy);
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Code", SqlDbType.VarChar, 20).Value = entity.CategoryCode;
        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = entity.CategoryName;
        cmd.Parameters.Add("@Desc", SqlDbType.NVarChar, 300).Value = entity.Description.ToDbValue();
        cmd.Parameters.Add("@Active", SqlDbType.Bit).Value = entity.IsActive;
        cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = entity.CreatedBy.ToDbValue();

        var newId = (int)cmd.ExecuteScalar()!;
        entity.CategoryId = newId;
        return newId;
    });

    /// <inheritdoc/>
    public bool Update(Category entity) => Execute(nameof(Update), () =>
    {
        const string sql = """
            UPDATE dbo.Categories
               SET CategoryName = @Name,
                   Description  = @Desc,
                   IsActive     = @Active,
                   UpdatedAt    = SYSUTCDATETIME(),
                   UpdatedBy    = @UpdatedBy
             WHERE CategoryId = @Id AND IsDeleted = 0;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = entity.CategoryId;
        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = entity.CategoryName;
        cmd.Parameters.Add("@Desc", SqlDbType.NVarChar, 300).Value = entity.Description.ToDbValue();
        cmd.Parameters.Add("@Active", SqlDbType.Bit).Value = entity.IsActive;
        cmd.Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = entity.UpdatedBy.ToDbValue();

        return cmd.ExecuteNonQuery() > 0;
    });

    /// <inheritdoc/>
    public bool Delete(int id) => Execute(nameof(Delete), () =>
    {
        // Kiểm tra ràng buộc trước: không cho xoá nếu còn sách tham chiếu
        const string sql = """
            IF EXISTS (SELECT 1 FROM dbo.Books WHERE CategoryId = @Id AND IsDeleted = 0)
                THROW 50100, N'Danh mục đang chứa sách, không thể xoá.', 1;

            UPDATE dbo.Categories
               SET IsDeleted = 1,
                   DeletedAt = SYSUTCDATETIME(),
                   IsActive  = 0
             WHERE CategoryId = @Id AND IsDeleted = 0;

            SELECT @@ROWCOUNT;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        var affected = (int)cmd.ExecuteScalar()!;
        return affected > 0;
    });

    /// <inheritdoc/>
    public bool ExistsByCode(string categoryCode) => Execute(nameof(ExistsByCode), () =>
    {
        const string sql = """
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM dbo.Categories
                WHERE CategoryCode = @Code AND IsDeleted = 0
            ) THEN 1 ELSE 0 END;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Code", SqlDbType.VarChar, 20).Value = categoryCode;
        return (int)cmd.ExecuteScalar()! == 1;
    });
}
