using System.Data;
using Microsoft.Data.SqlClient;
using LibraryApp.DAL.Common;
using LibraryApp.DAL.Interfaces;
using LibraryApp.DAL.Mappers;
using LibraryApp.Models;
using LibraryApp.Models.Enums;

namespace LibraryApp.DAL.Repositories;

/// <summary>
/// Repository thao tác với bảng <c>dbo.Readers</c>.
/// </summary>
public sealed class ReaderRepository : BaseRepository, IReaderRepository
{
    /// <inheritdoc/>
    public List<Reader> GetAll() => Execute(nameof(GetAll), () =>
    {
        const string sql = $"""
            SELECT {ReaderMapper.SelectColumns}
            FROM dbo.Readers
            WHERE IsDeleted = 0
            ORDER BY FullName;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        return ReadList(cmd, ReaderMapper.Map);
    });

    /// <inheritdoc/>
    public Reader? GetById(int id) => Execute(nameof(GetById), () =>
    {
        const string sql = $"""
            SELECT {ReaderMapper.SelectColumns}
            FROM dbo.Readers
            WHERE ReaderId = @Id AND IsDeleted = 0;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? ReaderMapper.Map(reader) : null;
    });

    /// <inheritdoc/>
    public Reader? GetByCardNumber(string cardNumber) => Execute(nameof(GetByCardNumber), () =>
    {
        const string sql = $"""
            SELECT {ReaderMapper.SelectColumns}
            FROM dbo.Readers
            WHERE CardNumber = @Card AND IsDeleted = 0;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Card", SqlDbType.VarChar, 20).Value = cardNumber;
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? ReaderMapper.Map(reader) : null;
    });

    /// <inheritdoc/>
    public int Insert(Reader entity) => Execute(nameof(Insert), () =>
    {
        const string sql = """
            INSERT INTO dbo.Readers
                (CardNumber, FullName, DateOfBirth, Gender, Address, Phone, Email,
                 CardIssueDate, CardExpireDate, Status, CreatedBy)
            OUTPUT INSERTED.ReaderId
            VALUES
                (@Card, @Name, @DOB, @Gender, @Addr, @Phone, @Email,
                 @Issue, @Expire, @Status, @CreatedBy);
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Card", SqlDbType.VarChar, 20).Value = entity.CardNumber;
        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = entity.FullName;
        cmd.Parameters.Add("@DOB", SqlDbType.Date).Value = entity.DateOfBirth.HasValue
                                                                          ? entity.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue)
                                                                          : DBNull.Value;
        cmd.Parameters.Add("@Gender", SqlDbType.NVarChar, 10).Value = ReaderMapper.ToDbString(entity.Gender).ToDbValue();
        cmd.Parameters.Add("@Addr", SqlDbType.NVarChar, 200).Value = entity.Address.ToDbValue();
        cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 20).Value = entity.Phone.ToDbValue();
        cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = entity.Email.ToDbValue();
        cmd.Parameters.Add("@Issue", SqlDbType.Date).Value = entity.CardIssueDate.ToDateTime(TimeOnly.MinValue);
        cmd.Parameters.Add("@Expire", SqlDbType.Date).Value = entity.CardExpireDate.ToDateTime(TimeOnly.MinValue);
        cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = entity.Status.ToString();
        cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = entity.CreatedBy.ToDbValue();

        var newId = (int)cmd.ExecuteScalar()!;
        entity.ReaderId = newId;
        return newId;
    });

    /// <inheritdoc/>
    public bool Update(Reader entity) => Execute(nameof(Update), () =>
    {
        const string sql = """
            UPDATE dbo.Readers
               SET FullName       = @Name,
                   DateOfBirth    = @DOB,
                   Gender         = @Gender,
                   Address        = @Addr,
                   Phone          = @Phone,
                   Email          = @Email,
                   CardIssueDate  = @Issue,
                   CardExpireDate = @Expire,
                   Status         = @Status,
                   UpdatedAt      = SYSUTCDATETIME(),
                   UpdatedBy      = @UpdatedBy
             WHERE ReaderId = @Id AND IsDeleted = 0;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = entity.ReaderId;
        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = entity.FullName;
        cmd.Parameters.Add("@DOB", SqlDbType.Date).Value = entity.DateOfBirth.HasValue
                                                                          ? entity.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue)
                                                                          : DBNull.Value;
        cmd.Parameters.Add("@Gender", SqlDbType.NVarChar, 10).Value = ReaderMapper.ToDbString(entity.Gender).ToDbValue();
        cmd.Parameters.Add("@Addr", SqlDbType.NVarChar, 200).Value = entity.Address.ToDbValue();
        cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 20).Value = entity.Phone.ToDbValue();
        cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = entity.Email.ToDbValue();
        cmd.Parameters.Add("@Issue", SqlDbType.Date).Value = entity.CardIssueDate.ToDateTime(TimeOnly.MinValue);
        cmd.Parameters.Add("@Expire", SqlDbType.Date).Value = entity.CardExpireDate.ToDateTime(TimeOnly.MinValue);
        cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = entity.Status.ToString();
        cmd.Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = entity.UpdatedBy.ToDbValue();

        return cmd.ExecuteNonQuery() > 0;
    });

    /// <inheritdoc/>
    public bool Delete(int id) => Execute(nameof(Delete), () =>
    {
        const string sql = """
            IF EXISTS (
                SELECT 1 FROM dbo.BorrowReceipts
                WHERE ReaderId = @Id
                  AND IsDeleted = 0
                  AND Status IN ('Borrowing','PartiallyReturned','Overdue')
            )
                THROW 50101, N'Độc giả đang có sách chưa trả, không thể xoá.', 1;

            UPDATE dbo.Readers
               SET IsDeleted = 1,
                   DeletedAt = SYSUTCDATETIME(),
                   Status    = 'Locked'
             WHERE ReaderId = @Id AND IsDeleted = 0;

            SELECT @@ROWCOUNT;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        return (int)cmd.ExecuteScalar()! > 0;
    });

    /// <inheritdoc/>
    public List<Reader> Search(string? keyword, ReaderStatus? status) => Execute(nameof(Search), () =>
    {
        using var conn = OpenConnection();
        using var cmd = BuildSearchCommand(conn, keyword, status);
        return ReadList(cmd, ReaderMapper.Map);
    });

    /// <inheritdoc/>
    public DataTable SearchAsDataTable(string? keyword, ReaderStatus? status)
        => Execute(nameof(SearchAsDataTable), () =>
        {
            using var conn = OpenConnection();
            using var cmd = BuildSearchCommand(conn, keyword, status);
            return ReadDataTable(cmd);
        });

    /// <inheritdoc/>
    public bool ExistsByCardNumber(string cardNumber) => Execute(nameof(ExistsByCardNumber), () =>
    {
        const string sql = """
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM dbo.Readers WHERE CardNumber = @Card AND IsDeleted = 0
            ) THEN 1 ELSE 0 END;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Card", SqlDbType.VarChar, 20).Value = cardNumber;
        return (int)cmd.ExecuteScalar()! == 1;
    });

    private static SqlCommand BuildSearchCommand(SqlConnection conn, string? keyword, ReaderStatus? status)
    {
        var sql = $"""
            SELECT {ReaderMapper.SelectColumns}
            FROM dbo.Readers
            WHERE IsDeleted = 0
              AND (@Keyword IS NULL OR FullName   LIKE N'%' + @Keyword + N'%'
                                    OR CardNumber LIKE       @Keyword + '%'
                                    OR Phone      LIKE       @Keyword + '%')
              AND (@Status  IS NULL OR Status = @Status)
            ORDER BY FullName;
            """;
        var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 100).Value = (object?)keyword ?? DBNull.Value;
        cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = status?.ToString() ?? (object)DBNull.Value;
        return cmd;
    }
}
