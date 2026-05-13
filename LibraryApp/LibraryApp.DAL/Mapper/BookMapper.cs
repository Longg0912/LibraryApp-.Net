using System.Data;
using LibraryApp.DAL.Common;
using LibraryApp.Models;
using LibraryApp.Models.Enums;

namespace LibraryApp.DAL.Mappers;

/// <summary>
/// Chuyển đổi giữa <see cref="IDataReader"/> và <see cref="Book"/>.
/// </summary>
internal static class BookMapper
{
    public const string SelectColumns = """
        BookId, BookCode, Title, Author, Publisher, PublishYear, CategoryId,
        Quantity, AvailableQty, Price, Status,
        CreatedAt, CreatedBy, UpdatedAt, UpdatedBy,
        IsDeleted, DeletedAt, DeletedBy, RowVer
        """;

    public static Book Map(IDataReader r) => new()
    {
        BookId = r.GetInt32("BookId"),
        BookCode = r.GetStringRequired("BookCode"),
        Title = r.GetStringRequired("Title"),
        Author = r.GetStringRequired("Author"),
        Publisher = r.GetStringOrNull("Publisher"),
        PublishYear = r.GetInt32OrNull("PublishYear"),
        CategoryId = r.GetInt32("CategoryId"),
        Quantity = r.GetInt32("Quantity"),
        AvailableQty = r.GetInt32("AvailableQty"),
        Price = r.GetDecimal("Price"),
        Status = r.GetEnum<BookStatus>("Status"),
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
