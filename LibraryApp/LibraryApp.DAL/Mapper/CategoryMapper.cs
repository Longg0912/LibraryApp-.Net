using System.Data;
using LibraryApp.DAL.Common;
using LibraryApp.Models;

namespace LibraryApp.DAL.Mappers;

/// <summary>
/// Chuyển đổi giữa <see cref="IDataReader"/> và <see cref="Category"/>.
/// </summary>
internal static class CategoryMapper
{
    /// <summary>Tên các cột chuẩn dùng trong <c>SELECT</c>.</summary>
    public const string SelectColumns = """
        CategoryId, CategoryCode, CategoryName, Description, IsActive,
        CreatedAt, CreatedBy, UpdatedAt, UpdatedBy,
        IsDeleted, DeletedAt, DeletedBy, RowVer
        """;

    /// <summary>Map một row hiện tại của reader thành <see cref="Category"/>.</summary>
    public static Category Map(IDataReader r) => new()
    {
        CategoryId = r.GetInt32("CategoryId"),
        CategoryCode = r.GetStringRequired("CategoryCode"),
        CategoryName = r.GetStringRequired("CategoryName"),
        Description = r.GetStringOrNull("Description"),
        IsActive = r.GetBoolean("IsActive"),
        CreatedAt = r.GetDateTime("CreatedAt"),
        CreatedBy = r.GetInt32OrNull("CreatedBy"),
        UpdatedAt = r.GetDateTimeOrNull("UpdatedAt"),
        UpdatedBy = r.GetInt32OrNull("UpdatedBy"),
        IsDeleted = r.GetBoolean("IsDeleted"),
        DeletedAt = r.GetDateTimeOrNull("DeletedAt"),
        DeletedBy = r.GetInt32OrNull("DeletedBy"),
        RowVersion = r.GetRowVersion("RowVer")
    };
}
