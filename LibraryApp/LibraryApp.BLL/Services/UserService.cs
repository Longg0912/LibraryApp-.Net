using System.Data;
using LibraryApp.BLL.Common;
using LibraryApp.BLL.Interfaces;
using LibraryApp.BLL.Validation;
using LibraryApp.DAL;
using LibraryApp.DAL.Common;
using LibraryApp.Models;
using Microsoft.Data.SqlClient;

namespace LibraryApp.BLL.Services;

/// <summary>
/// Service quản lý user (Admin). Gọi SQL trực tiếp qua
/// <see cref="DatabaseConnection"/> — tương tự pattern của <c>ReportService</c>.
/// </summary>
/// <remarks>
/// Không dùng repository pattern ở đây vì các thao tác quản lý user mang tính
/// admin-only và đơn giản (không có concurrency phức tạp). Direct SQL gọn hơn
/// và dễ audit từng câu query.
/// </remarks>
public sealed class UserService : IUserService
{
    /// <inheritdoc/>
    public DataTable SearchAsDataTable(string? keyword, int? roleId, bool? isActive)
    {
        const string baseSql = """
            SELECT u.UserId, u.Username, u.FullName, u.Email, u.Phone,
                   r.RoleId, r.RoleCode, r.RoleName,
                   u.IsActive, u.LastLoginAt, u.LastLoginIp,
                   u.FailedLoginCount, u.LockoutUntil, u.MustChangePassword,
                   u.CreatedAt
            FROM dbo.Users u
            JOIN dbo.Roles r ON r.RoleId = u.RoleId
            WHERE u.IsDeleted = 0
            """;

        var sql = new System.Text.StringBuilder(baseSql);
        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand { Connection = conn };

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            sql.AppendLine("""
                  AND (u.Username LIKE @Keyword + '%'
                    OR u.FullName LIKE N'%' + @Keyword + N'%'
                    OR u.Email    LIKE       @Keyword + '%'
                    OR u.Phone    LIKE       @Keyword + '%')
                """);
            cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 100).Value = keyword.Trim();
        }

        if (roleId.HasValue && roleId.Value > 0)
        {
            sql.AppendLine("  AND u.RoleId = @RoleId");
            cmd.Parameters.Add("@RoleId", SqlDbType.Int).Value = roleId.Value;
        }

        if (isActive.HasValue)
        {
            sql.AppendLine("  AND u.IsActive = @IsActive");
            cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive.Value;
        }

        sql.AppendLine("ORDER BY u.Username;");
        cmd.CommandText = sql.ToString();

        using var reader = cmd.ExecuteReader();
        var dt = new DataTable();
        dt.Load(reader);
        return dt;
    }

    /// <inheritdoc/>
    public User? GetById(int userId)
    {
        Validator.Positive(userId, "ID người dùng");

        const string sql = """
            SELECT UserId, Username, PasswordHash, FullName, Email, Phone,
                   RoleId, IsActive, LastLoginAt, LastLoginIp,
                   FailedLoginCount, LockoutUntil, MustChangePassword,
                   CreatedAt, RowVer
            FROM dbo.Users
            WHERE UserId = @Id AND IsDeleted = 0;
            """;

        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;
        using var rd = cmd.ExecuteReader();
        if (!rd.Read()) return null;

        return new User
        {
            UserId = rd.GetInt32(0),
            Username = rd.GetString(1),
            PasswordHash = rd.GetString(2),
            FullName = rd.GetString(3),
            Email = rd.IsDBNull(4) ? null : rd.GetString(4),
            Phone = rd.IsDBNull(5) ? null : rd.GetString(5),
            RoleId = rd.GetInt32(6),
            IsActive = rd.GetBoolean(7),
            LastLoginAt = rd.IsDBNull(8) ? null : rd.GetDateTime(8),
            LastLoginIp = rd.IsDBNull(9) ? null : rd.GetString(9),
            FailedLoginCount = rd.GetInt32(10),
            LockoutUntil = rd.IsDBNull(11) ? null : rd.GetDateTime(11),
            MustChangePassword = rd.GetBoolean(12),
            CreatedAt = rd.GetDateTime(13)
        };
    }

    /// <inheritdoc/>
    public List<Role> GetAllRoles()
    {
        const string sql = "SELECT RoleId, RoleCode, RoleName, Description FROM dbo.Roles ORDER BY RoleId;";
        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        using var rd = cmd.ExecuteReader();

        var list = new List<Role>();
        while (rd.Read())
        {
            list.Add(new Role
            {
                RoleId = rd.GetInt32(0),
                RoleCode = rd.GetString(1),
                RoleName = rd.GetString(2),
                Description = rd.IsDBNull(3) ? null : rd.GetString(3)
            });
        }
        return list;
    }

    /// <inheritdoc/>
    public int Create(string username, string fullName, string? email, string? phone,
                      int roleId, string tempPassword)
    {
        // ---- Validate ----
        Validator.NotEmpty(username, "Tên đăng nhập");
        Validator.Length(username, "Tên đăng nhập", 3, 50);
        Validator.NotEmpty(fullName, "Họ tên");
        Validator.Length(fullName, "Họ tên", 2, 100);
        Validator.Email(email);
        Validator.Phone(phone);
        Validator.Positive(roleId, "Vai trò");
        Validator.Password(tempPassword, "Mật khẩu tạm");

        if (UsernameExists(username))
            throw new BusinessException("Tên đăng nhập",
                $"Tên đăng nhập '{username}' đã tồn tại.");

        var hash = BCrypt.Net.BCrypt.HashPassword(tempPassword, workFactor: 11);

        const string sql = """
            INSERT INTO dbo.Users
                (Username, PasswordHash, FullName, Email, Phone, RoleId,
                 IsActive, MustChangePassword, FailedLoginCount, CreatedAt)
            OUTPUT INSERTED.UserId
            VALUES (@Username, @Hash, @FullName, @Email, @Phone, @RoleId,
                    1, 1, 0, SYSUTCDATETIME());
            """;

        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Username", SqlDbType.VarChar, 50).Value = username.Trim();
        cmd.Parameters.Add("@Hash", SqlDbType.VarChar, 200).Value = hash;
        cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = fullName.Trim();
        cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = (object?)email?.Trim() ?? DBNull.Value;
        cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 15).Value = (object?)phone?.Trim() ?? DBNull.Value;
        cmd.Parameters.Add("@RoleId", SqlDbType.Int).Value = roleId;

        return (int)cmd.ExecuteScalar()!;
    }

    /// <inheritdoc/>
    public void Update(int userId, string fullName, string? email, string? phone, int roleId)
    {
        Validator.Positive(userId, "ID người dùng");
        Validator.NotEmpty(fullName, "Họ tên");
        Validator.Length(fullName, "Họ tên", 2, 100);
        Validator.Email(email);
        Validator.Phone(phone);
        Validator.Positive(roleId, "Vai trò");

        const string sql = """
            UPDATE dbo.Users
            SET FullName  = @FullName,
                Email     = @Email,
                Phone     = @Phone,
                RoleId    = @RoleId,
                UpdatedAt = SYSUTCDATETIME()
            WHERE UserId = @Id AND IsDeleted = 0;
            """;

        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;
        cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = fullName.Trim();
        cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = (object?)email?.Trim() ?? DBNull.Value;
        cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 15).Value = (object?)phone?.Trim() ?? DBNull.Value;
        cmd.Parameters.Add("@RoleId", SqlDbType.Int).Value = roleId;

        int affected = cmd.ExecuteNonQuery();
        if (affected == 0)
            throw new BusinessException("Không tìm thấy người dùng để cập nhật.");
    }

    /// <inheritdoc/>
    public void SetActive(int userId, bool isActive)
    {
        Validator.Positive(userId, "ID người dùng");

        const string sql = """
            UPDATE dbo.Users
            SET IsActive  = @IsActive,
                LockoutUntil = CASE WHEN @IsActive = 1 THEN NULL ELSE LockoutUntil END,
                FailedLoginCount = CASE WHEN @IsActive = 1 THEN 0 ELSE FailedLoginCount END,
                UpdatedAt = SYSUTCDATETIME()
            WHERE UserId = @Id AND IsDeleted = 0;
            """;

        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;
        cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;

        if (cmd.ExecuteNonQuery() == 0)
            throw new BusinessException("Không tìm thấy người dùng.");
    }

    /// <inheritdoc/>
    public string ResetPassword(int userId)
    {
        Validator.Positive(userId, "ID người dùng");

        // Sinh mật khẩu tạm: 4 chữ + 4 số ngẫu nhiên (vd "abcd1234")
        var temp = GenerateTempPassword();
        var hash = BCrypt.Net.BCrypt.HashPassword(temp, workFactor: 11);

        const string sql = """
            UPDATE dbo.Users
            SET PasswordHash       = @Hash,
                MustChangePassword = 1,
                FailedLoginCount   = 0,
                LockoutUntil       = NULL,
                UpdatedAt          = SYSUTCDATETIME()
            WHERE UserId = @Id AND IsDeleted = 0;
            """;

        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;
        cmd.Parameters.Add("@Hash", SqlDbType.VarChar, 200).Value = hash;

        if (cmd.ExecuteNonQuery() == 0)
            throw new BusinessException("Không tìm thấy người dùng.");

        return temp;
    }

    /// <inheritdoc/>
    public void Delete(int userId)
    {
        Validator.Positive(userId, "ID người dùng");

        // Không cho xoá user còn phiếu mượn đang mở (giữ tính toàn vẹn audit)
        const string checkSql = """
            SELECT COUNT(*) FROM dbo.BorrowReceipts
            WHERE UserId = @Id AND Status IN ('Borrowing','PartiallyReturned','Overdue')
              AND IsDeleted = 0;
            """;

        using var conn = DatabaseConnection.OpenConnection();
        using (var checkCmd = new SqlCommand(checkSql, conn))
        {
            checkCmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;
            int count = (int)checkCmd.ExecuteScalar()!;
            if (count > 0)
                throw new BusinessException(
                    "Không thể xoá người dùng đang có phiếu mượn mở. " +
                    "Vui lòng đợi tất cả phiếu mượn của người dùng được đóng.");
        }

        const string sql = """
            UPDATE dbo.Users
            SET IsDeleted = 1,
                IsActive  = 0,
                DeletedAt = SYSUTCDATETIME(),
                UpdatedAt = SYSUTCDATETIME()
            WHERE UserId = @Id AND IsDeleted = 0;
            """;

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;

        if (cmd.ExecuteNonQuery() == 0)
            throw new BusinessException("Không tìm thấy người dùng để xoá.");
    }

    // ----------------------------------------------------------------
    // Helpers
    // ----------------------------------------------------------------

    private static bool UsernameExists(string username)
    {
        const string sql = "SELECT 1 FROM dbo.Users WHERE Username = @U AND IsDeleted = 0;";
        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@U", SqlDbType.VarChar, 50).Value = username.Trim();
        return cmd.ExecuteScalar() is not null;
    }

    /// <summary>Sinh mật khẩu tạm có 4 chữ thường + 4 số (đáp ứng yêu cầu phức tạp).</summary>
    private static string GenerateTempPassword()
    {
        const string letters = "abcdefghjkmnpqrstuvwxyz";   // bỏ i,l,o gây nhầm lẫn
        const string digits = "23456789";                  // bỏ 0,1 gây nhầm lẫn

        var rnd = Random.Shared;
        var chars = new char[8];
        for (int i = 0; i < 4; i++) chars[i] = letters[rnd.Next(letters.Length)];
        for (int i = 4; i < 8; i++) chars[i] = digits[rnd.Next(digits.Length)];

        // Shuffle
        for (int i = chars.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
        return new string(chars);
    }
}
