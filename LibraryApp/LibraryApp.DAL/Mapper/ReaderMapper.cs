using System.Data;
using LibraryApp.DAL.Common;
using LibraryApp.Models;
using LibraryApp.Models.Enums;

namespace LibraryApp.DAL.Mappers;

/// <summary>
/// Mapper cho entity <see cref="Reader"/>.
/// </summary>
internal static class ReaderMapper
{
    public const string SelectColumns = """
        ReaderId, CardNumber, FullName, DateOfBirth, Gender, Address, Phone, Email,
        CardIssueDate, CardExpireDate, Status,
        CreatedAt, CreatedBy, UpdatedAt, UpdatedBy,
        IsDeleted, DeletedAt, DeletedBy, RowVer
        """;

    public static Reader Map(IDataReader r) => new()
    {
        ReaderId = r.GetInt32("ReaderId"),
        CardNumber = r.GetStringRequired("CardNumber"),
        FullName = r.GetStringRequired("FullName"),
        DateOfBirth = r.GetDateOnlyOrNull("DateOfBirth"),
        Gender = MapGender(r.GetStringOrNull("Gender")),
        Address = r.GetStringOrNull("Address"),
        Phone = r.GetStringOrNull("Phone"),
        Email = r.GetStringOrNull("Email"),
        CardIssueDate = r.GetDateOnly("CardIssueDate"),
        CardExpireDate = r.GetDateOnly("CardExpireDate"),
        Status = r.GetEnum<ReaderStatus>("Status"),
        CreatedAt = r.GetDateTime("CreatedAt"),
        CreatedBy = r.GetInt32OrNull("CreatedBy"),
        UpdatedAt = r.GetDateTimeOrNull("UpdatedAt"),
        UpdatedBy = r.GetInt32OrNull("UpdatedBy"),
        IsDeleted = r.GetBoolean("IsDeleted"),
        DeletedAt = r.GetDateTimeOrNull("DeletedAt"),
        DeletedBy = r.GetInt32OrNull("DeletedBy"),
        RowVersion = r.GetRowVersion("RowVer")
    };

    /// <summary>
    /// Map giá trị Gender lưu dưới dạng tiếng Việt trong DB sang enum.
    /// </summary>
    private static Gender? MapGender(string? raw) => raw switch
    {
        "Nam" => Models.Enums.Gender.Male,
        "Nữ" => Models.Enums.Gender.Female,
        "Khác" => Models.Enums.Gender.Other,
        _ => null
    };

    /// <summary>Chuyển ngược enum sang chuỗi tiếng Việt để insert vào DB.</summary>
    public static string? ToDbString(Gender? gender) => gender switch
    {
        Models.Enums.Gender.Male => "Nam",
        Models.Enums.Gender.Female => "Nữ",
        Models.Enums.Gender.Other => "Khác",
        _ => null
    };
}
