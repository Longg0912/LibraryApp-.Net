using System.Data;
using Microsoft.Data.SqlClient;
using LibraryApp.DAL.Common;
using LibraryApp.DAL.Interfaces;
using LibraryApp.DAL.Mappers;
using LibraryApp.Models;
using LibraryApp.Models.Enums;

namespace LibraryApp.DAL.Repositories;

/// <summary>
/// Repository thao tác với bảng <c>dbo.Books</c>. Một số thao tác phức tạp
/// (thêm/sửa/xoá) được uỷ thác cho stored procedure để đảm bảo audit log
/// và validate đồng nhất với các tầng khác.
/// </summary>
public sealed class BookRepository : BaseRepository, IBookRepository
{
    /// <inheritdoc/>
    public List<Book> GetAll() => Execute(nameof(GetAll), () =>
    {
        const string sql = $"""
            SELECT {BookMapper.SelectColumns}
            FROM dbo.Books
            WHERE IsDeleted = 0
            ORDER BY Title;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        return ReadList(cmd, BookMapper.Map);
    });

    /// <inheritdoc/>
    public Book? GetById(int id) => Execute(nameof(GetById), () =>
    {
        const string sql = $"""
            SELECT {BookMapper.SelectColumns}
            FROM dbo.Books
            WHERE BookId = @Id AND IsDeleted = 0;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        using var reader = cmd.ExecuteReader();
        return reader.Read() ? BookMapper.Map(reader) : null;
    });

    /// <inheritdoc/>
    public int Insert(Book entity) => Execute(nameof(Insert), () =>
    {
        // Uỷ thác cho stored procedure để bảo đảm audit log và kiểm tra mã trùng
        using var conn = OpenConnection();
        using var cmd = new SqlCommand("dbo.sp_Book_Insert", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.Add("@BookCode", SqlDbType.VarChar, 30).Value = entity.BookCode;
        cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 200).Value = entity.Title;
        cmd.Parameters.Add("@Author", SqlDbType.NVarChar, 150).Value = entity.Author;
        cmd.Parameters.Add("@Publisher", SqlDbType.NVarChar, 150).Value = entity.Publisher.ToDbValue();
        cmd.Parameters.Add("@PublishYear", SqlDbType.Int).Value = entity.PublishYear.ToDbValue();
        cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = entity.CategoryId;
        cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = entity.Quantity;
        cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = entity.Price;
        cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = entity.CreatedBy.ToDbValue();

        var idParam = cmd.Parameters.Add("@NewBookId", SqlDbType.Int);
        idParam.Direction = ParameterDirection.Output;

        cmd.ExecuteNonQuery();
        var newId = (int)idParam.Value!;
        entity.BookId = newId;
        return newId;
    });

    /// <inheritdoc/>
    public bool Update(Book entity) => Execute(nameof(Update), () =>
    {
        using var conn = OpenConnection();
        using var cmd = new SqlCommand("dbo.sp_Book_Update", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.Add("@BookId", SqlDbType.Int).Value = entity.BookId;
        cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 200).Value = entity.Title;
        cmd.Parameters.Add("@Author", SqlDbType.NVarChar, 150).Value = entity.Author;
        cmd.Parameters.Add("@Publisher", SqlDbType.NVarChar, 150).Value = entity.Publisher.ToDbValue();
        cmd.Parameters.Add("@PublishYear", SqlDbType.Int).Value = entity.PublishYear.ToDbValue();
        cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = entity.CategoryId;
        cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = entity.Quantity;
        cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = entity.Price;
        cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = entity.Status.ToString();
        cmd.Parameters.Add("@RowVer", SqlDbType.Binary, 8).Value = entity.RowVersion.ToDbValue();
        cmd.Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = entity.UpdatedBy.ToDbValue();

        cmd.ExecuteNonQuery();
        return true;
    });

    /// <inheritdoc/>
    public bool Delete(int id) => Execute(nameof(Delete), () =>
    {
        using var conn = OpenConnection();
        using var cmd = new SqlCommand("dbo.sp_Book_Delete", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.Add("@BookId", SqlDbType.Int).Value = id;
        cmd.Parameters.Add("@DeletedBy", SqlDbType.Int).Value = DBNull.Value;
        cmd.ExecuteNonQuery();
        return true;
    });

    /// <inheritdoc/>
    public List<Book> Search(string? keyword, int? categoryId, BookStatus? status, int? yearFrom, int? yearTo)
        => Execute(nameof(Search), () =>
        {
            using var conn = OpenConnection();
            using var cmd = BuildSearchCommand(conn, keyword, categoryId, status, yearFrom, yearTo);

            using var reader = cmd.ExecuteReader();
            // SP trả về 2 result set: tổng count + dữ liệu. Bỏ qua count, đọc dữ liệu
            if (!reader.NextResult()) return [];

            var list = new List<Book>();
            while (reader.Read())
                list.Add(BookMapper.Map(reader));
            return list;
        });

    /// <inheritdoc/>
    public DataTable SearchAsDataTable(string? keyword, int? categoryId, BookStatus? status, int? yearFrom, int? yearTo)
        => Execute(nameof(SearchAsDataTable), () =>
        {
            // Dùng query trực tiếp để dễ build DataTable không kèm count
            var sql = $"""
            SELECT b.BookId, b.BookCode, b.Title, b.Author, b.Publisher,
                   b.PublishYear, c.CategoryName, b.Quantity, b.AvailableQty,
                   b.Price, b.Status
            FROM dbo.Books b
            JOIN dbo.Categories c ON c.CategoryId = b.CategoryId
            WHERE b.IsDeleted = 0
              AND (@Keyword    IS NULL OR b.Title    LIKE N'%' + @Keyword + N'%'
                                       OR b.Author   LIKE N'%' + @Keyword + N'%'
                                       OR b.BookCode LIKE       @Keyword + '%')
              AND (@CategoryId IS NULL OR b.CategoryId  = @CategoryId)
              AND (@Status     IS NULL OR b.Status      = @Status)
              AND (@YearFrom   IS NULL OR b.PublishYear >= @YearFrom)
              AND (@YearTo     IS NULL OR b.PublishYear <= @YearTo)
            ORDER BY b.Title;
            """;

            using var conn = OpenConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 200).Value = (object?)keyword ?? DBNull.Value;
            cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId.ToDbValue();
            cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = status?.ToString() ?? (object)DBNull.Value;
            cmd.Parameters.Add("@YearFrom", SqlDbType.Int).Value = yearFrom.ToDbValue();
            cmd.Parameters.Add("@YearTo", SqlDbType.Int).Value = yearTo.ToDbValue();

            return ReadDataTable(cmd);
        });

    /// <inheritdoc/>
    public bool ExistsByCode(string bookCode) => Execute(nameof(ExistsByCode), () =>
    {
        const string sql = """
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM dbo.Books WHERE BookCode = @Code AND IsDeleted = 0
            ) THEN 1 ELSE 0 END;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Code", SqlDbType.VarChar, 30).Value = bookCode;
        return (int)cmd.ExecuteScalar()! == 1;
    });

    /// <summary>
    /// Build <see cref="SqlCommand"/> gọi <c>sp_Book_Search</c> với tham số.
    /// Tách ra để tái sử dụng giữa <see cref="Search"/> (List) và phiên bản async tương lai.
    /// </summary>
    private static SqlCommand BuildSearchCommand(SqlConnection conn,
        string? keyword, int? categoryId, BookStatus? status, int? yearFrom, int? yearTo)
    {
        var cmd = new SqlCommand("dbo.sp_Book_Search", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 200).Value = (object?)keyword ?? DBNull.Value;
        cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId.ToDbValue();
        cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = status?.ToString() ?? (object)DBNull.Value;
        cmd.Parameters.Add("@YearFrom", SqlDbType.Int).Value = yearFrom.ToDbValue();
        cmd.Parameters.Add("@YearTo", SqlDbType.Int).Value = yearTo.ToDbValue();
        cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = 1;
        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = 1000;
        return cmd;
    }
}
