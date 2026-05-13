using System.Data;
using LibraryApp.DAL.Common;
using LibraryApp.Models;

namespace LibraryApp.DAL.Mappers;

/// <summary>
/// Mapper cho entity <see cref="User"/>.
/// </summary>
internal static class UserMapper
{
    public const string SelectColumns = """
        UserId, Username, PasswordHash, PasswordSalt, FullName, Email, Phone, RoleId,
        IsActive, FailedLoginCount, LockoutUntil, LastLoginAt, LastLoginIp,
        PasswordChangedAt, MustChangePassword,
        CreatedAt, CreatedBy, UpdatedAt, UpdatedBy,
        IsDeleted, DeletedAt, DeletedBy, RowVer
        """;

    public static User Map(IDataReader r) => new()
    {
        UserId = r.GetInt32("UserId"),
        Username = r.GetStringRequired("Username"),
        PasswordHash = r.GetStringRequired("PasswordHash"),
        PasswordSalt = r.GetStringOrNull("PasswordSalt"),
        FullName = r.GetStringRequired("FullName"),
        Email = r.GetStringOrNull("Email"),
        Phone = r.GetStringOrNull("Phone"),
        RoleId = r.GetInt32("RoleId"),
        IsActive = r.GetBoolean("IsActive"),
        FailedLoginCount = r.GetInt32("FailedLoginCount"),
        LockoutUntil = r.GetDateTimeOrNull("LockoutUntil"),
        LastLoginAt = r.GetDateTimeOrNull("LastLoginAt"),
        LastLoginIp = r.GetStringOrNull("LastLoginIp"),
        PasswordChangedAt = r.GetDateTimeOrNull("PasswordChangedAt"),
        MustChangePassword = r.GetBoolean("MustChangePassword"),
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
