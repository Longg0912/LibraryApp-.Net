using System.Data;
using Microsoft.Data.SqlClient;
using LibraryApp.DAL.Common;
using LibraryApp.DAL.Interfaces;
using LibraryApp.DAL.Mappers;
using LibraryApp.Models;

namespace LibraryApp.DAL.Repositories;

/// <summary>
/// Repository thao tác với bảng <c>dbo.Users</c>.
/// </summary>
public sealed class UserRepository : BaseRepository, IUserRepository
{
    /// <inheritdoc/>
    public List<User> GetAll() => Execute(nameof(GetAll), () =>
    {
        const string sql = $"""
            SELECT {UserMapper.SelectColumns}
            FROM dbo.Users
            WHERE IsDeleted = 0
            ORDER BY FullName;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        return ReadList(cmd, UserMapper.Map);
    });

    /// <inheritdoc/>
    public User? GetById(int id) => Execute(nameof(GetById), () =>
    {
        const string sql = $"""
            SELECT {UserMapper.SelectColumns}
            FROM dbo.Users
            WHERE UserId = @Id AND IsDeleted = 0;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? UserMapper.Map(reader) : null;
    });

    /// <inheritdoc/>
    public User? GetByUsername(string username) => Execute(nameof(GetByUsername), () =>
    {
        const string sql = $"""
            SELECT {UserMapper.SelectColumns}
            FROM dbo.Users
            WHERE Username = @U AND IsDeleted = 0;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@U", SqlDbType.VarChar, 50).Value = username;
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? UserMapper.Map(reader) : null;
    });

    /// <inheritdoc/>
    public int Insert(User entity) => Execute(nameof(Insert), () =>
    {
        const string sql = """
            INSERT INTO dbo.Users
                (Username, PasswordHash, PasswordSalt, FullName, Email, Phone,
                 RoleId, IsActive, MustChangePassword, PasswordChangedAt, CreatedBy)
            OUTPUT INSERTED.UserId
            VALUES
                (@U, @Hash, @Salt, @Name, @Email, @Phone,
                 @Role, @Active, @MustChg, SYSUTCDATETIME(), @CreatedBy);
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@U", SqlDbType.VarChar, 50).Value = entity.Username;
        cmd.Parameters.Add("@Hash", SqlDbType.VarChar, 255).Value = entity.PasswordHash;
        cmd.Parameters.Add("@Salt", SqlDbType.VarChar, 128).Value = entity.PasswordSalt.ToDbValue();
        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = entity.FullName;
        cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = entity.Email.ToDbValue();
        cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 20).Value = entity.Phone.ToDbValue();
        cmd.Parameters.Add("@Role", SqlDbType.Int).Value = entity.RoleId;
        cmd.Parameters.Add("@Active", SqlDbType.Bit).Value = entity.IsActive;
        cmd.Parameters.Add("@MustChg", SqlDbType.Bit).Value = entity.MustChangePassword;
        cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = entity.CreatedBy.ToDbValue();

        var newId = (int)cmd.ExecuteScalar()!;
        entity.UserId = newId;
        return newId;
    });

    /// <inheritdoc/>
    public bool Update(User entity) => Execute(nameof(Update), () =>
    {
        // Không cập nhật mật khẩu ở đây — dùng riêng UpdatePassword
        const string sql = """
            UPDATE dbo.Users
               SET FullName  = @Name,
                   Email     = @Email,
                   Phone     = @Phone,
                   RoleId    = @Role,
                   IsActive  = @Active,
                   UpdatedAt = SYSUTCDATETIME(),
                   UpdatedBy = @UpdatedBy
             WHERE UserId = @Id AND IsDeleted = 0;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = entity.UserId;
        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = entity.FullName;
        cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = entity.Email.ToDbValue();
        cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 20).Value = entity.Phone.ToDbValue();
        cmd.Parameters.Add("@Role", SqlDbType.Int).Value = entity.RoleId;
        cmd.Parameters.Add("@Active", SqlDbType.Bit).Value = entity.IsActive;
        cmd.Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = entity.UpdatedBy.ToDbValue();
        return cmd.ExecuteNonQuery() > 0;
    });

    /// <inheritdoc/>
    public bool Delete(int id) => Execute(nameof(Delete), () =>
    {
        const string sql = """
            UPDATE dbo.Users
               SET IsDeleted = 1,
                   DeletedAt = SYSUTCDATETIME(),
                   IsActive  = 0
             WHERE UserId = @Id AND IsDeleted = 0;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        return cmd.ExecuteNonQuery() > 0;
    });

    /// <inheritdoc/>
    public List<User> Search(string? keyword, int? roleId, bool? isActive) => Execute(nameof(Search), () =>
    {
        var sql = $"""
            SELECT {UserMapper.SelectColumns}
            FROM dbo.Users
            WHERE IsDeleted = 0
              AND (@Keyword IS NULL OR Username LIKE @Keyword + '%'
                                    OR FullName LIKE N'%' + @Keyword + N'%')
              AND (@Role    IS NULL OR RoleId   = @Role)
              AND (@Active  IS NULL OR IsActive = @Active)
            ORDER BY FullName;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 100).Value = (object?)keyword ?? DBNull.Value;
        cmd.Parameters.Add("@Role", SqlDbType.Int).Value = roleId.ToDbValue();
        cmd.Parameters.Add("@Active", SqlDbType.Bit).Value = isActive.ToDbValue();
        return ReadList(cmd, UserMapper.Map);
    });

    /// <inheritdoc/>
    public bool ExistsByUsername(string username) => Execute(nameof(ExistsByUsername), () =>
    {
        const string sql = """
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM dbo.Users WHERE Username = @U AND IsDeleted = 0
            ) THEN 1 ELSE 0 END;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@U", SqlDbType.VarChar, 50).Value = username;
        return (int)cmd.ExecuteScalar()! == 1;
    });

    /// <inheritdoc/>
    public bool UpdatePassword(int userId, string newPasswordHash) => Execute(nameof(UpdatePassword), () =>
    {
        const string sql = """
            UPDATE dbo.Users
               SET PasswordHash       = @Hash,
                   PasswordChangedAt  = SYSUTCDATETIME(),
                   MustChangePassword = 0,
                   FailedLoginCount   = 0,
                   LockoutUntil       = NULL,
                   UpdatedAt          = SYSUTCDATETIME()
             WHERE UserId = @Id AND IsDeleted = 0;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;
        cmd.Parameters.Add("@Hash", SqlDbType.VarChar, 255).Value = newPasswordHash;
        return cmd.ExecuteNonQuery() > 0;
    });
}
