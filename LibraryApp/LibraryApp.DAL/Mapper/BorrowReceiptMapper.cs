using System.Data;
using LibraryApp.DAL.Common;
using LibraryApp.Models;
using LibraryApp.Models.Enums;

namespace LibraryApp.DAL.Mappers;

/// <summary>
/// Mapper cho phiếu mượn và chi tiết phiếu mượn / phiếu trả.
/// </summary>
internal static class BorrowReceiptMapper
{
    public const string SelectColumns = """
        BorrowId, ReceiptCode, ReaderId, UserId, BorrowDate, DueDate,
        Status, TotalFine, RenewCount, Note,
        CreatedAt, CreatedBy, UpdatedAt, UpdatedBy,
        IsDeleted, DeletedAt, DeletedBy, RowVer
        """;

    public static BorrowReceipt Map(IDataReader r) => new()
    {
        BorrowId = r.GetInt32("BorrowId"),
        ReceiptCode = r.GetStringRequired("ReceiptCode"),
        ReaderId = r.GetInt32("ReaderId"),
        UserId = r.GetInt32("UserId"),
        BorrowDate = r.GetDateOnly("BorrowDate"),
        DueDate = r.GetDateOnly("DueDate"),
        Status = r.GetEnum<BorrowStatus>("Status"),
        TotalFine = r.GetDecimal("TotalFine"),
        RenewCount = r.GetInt32("RenewCount"),
        Note = r.GetStringOrNull("Note"),
        CreatedAt = r.GetDateTime("CreatedAt"),
        CreatedBy = r.GetInt32OrNull("CreatedBy"),
        UpdatedAt = r.GetDateTimeOrNull("UpdatedAt"),
        UpdatedBy = r.GetInt32OrNull("UpdatedBy"),
        IsDeleted = r.GetBoolean("IsDeleted"),
        DeletedAt = r.GetDateTimeOrNull("DeletedAt"),
        DeletedBy = r.GetInt32OrNull("DeletedBy"),
        RowVersion = r.GetRowVersion("RowVer")
    };

    public static BorrowReceiptDetail MapDetail(IDataReader r) => new()
    {
        BorrowDetailId = r.GetInt32("BorrowDetailId"),
        BorrowId = r.GetInt32("BorrowId"),
        BookId = r.GetInt32("BookId"),
        Quantity = r.GetInt32("Quantity"),
        ReturnedQty = r.GetInt32("ReturnedQty"),
        Note = r.GetStringOrNull("Note")
    };
}
