using System.Data;
using Microsoft.Data.SqlClient;
using LibraryApp.DAL.Common;
using LibraryApp.DAL.Interfaces;
using LibraryApp.DAL.Mappers;
using LibraryApp.Models;
using LibraryApp.Models.Enums;

namespace LibraryApp.DAL.Repositories;

/// <summary>
/// Repository thao tác với <c>dbo.BorrowReceipts</c>. Các thao tác nghiệp vụ chính
/// (lập phiếu, ghi nhận trả) được uỷ thác cho stored procedure để bảo đảm
/// transaction và chống race condition (UPDLOCK + HOLDLOCK trên bảng <c>Books</c>).
/// </summary>
public sealed class BorrowReceiptRepository : BaseRepository, IBorrowReceiptRepository
{
    /// <inheritdoc/>
    public List<BorrowReceipt> GetAll() => Execute(nameof(GetAll), () =>
    {
        const string sql = $"""
            SELECT {BorrowReceiptMapper.SelectColumns}
            FROM dbo.BorrowReceipts
            WHERE IsDeleted = 0
            ORDER BY BorrowDate DESC;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        return ReadList(cmd, BorrowReceiptMapper.Map);
    });

    /// <inheritdoc/>
    public BorrowReceipt? GetById(int id) => Execute(nameof(GetById), () =>
    {
        const string sql = $"""
            SELECT {BorrowReceiptMapper.SelectColumns}
            FROM dbo.BorrowReceipts
            WHERE BorrowId = @Id AND IsDeleted = 0;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? BorrowReceiptMapper.Map(reader) : null;
    });

    /// <inheritdoc/>
    public BorrowReceipt? GetByIdWithDetails(int borrowId) => Execute(nameof(GetByIdWithDetails), () =>
    {
        // 2 result set: phiếu mượn + chi tiết. Đọc tuần tự rồi gắn vào nhau.
        const string sql = $"""
            SELECT {BorrowReceiptMapper.SelectColumns}
            FROM dbo.BorrowReceipts
            WHERE BorrowId = @Id AND IsDeleted = 0;

            SELECT d.BorrowDetailId, d.BorrowId, d.BookId, d.Quantity, d.ReturnedQty, d.Note
            FROM dbo.BorrowReceiptDetails d
            WHERE d.BorrowId = @Id;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = borrowId;
        using var reader = cmd.ExecuteReader();

        if (!reader.Read()) return null;
        var receipt = BorrowReceiptMapper.Map(reader);

        if (reader.NextResult())
        {
            while (reader.Read())
                receipt.Details.Add(BorrowReceiptMapper.MapDetail(reader));
        }
        return receipt;
    });

    /// <inheritdoc/>
    public List<BorrowReceipt> GetActiveByReader(int readerId) => Execute(nameof(GetActiveByReader), () =>
    {
        const string sql = $"""
            SELECT {BorrowReceiptMapper.SelectColumns}
            FROM dbo.BorrowReceipts
            WHERE ReaderId = @Reader
              AND IsDeleted = 0
              AND Status IN ('Borrowing','PartiallyReturned','Overdue')
            ORDER BY BorrowDate DESC;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Reader", SqlDbType.Int).Value = readerId;
        return ReadList(cmd, BorrowReceiptMapper.Map);
    });

    /// <inheritdoc/>
    /// <remarks>
    /// Method <see cref="Insert"/> được uỷ quyền cho <see cref="CreateBorrow"/> để
    /// đảm bảo bao giờ cũng đi qua stored procedure. Không tạo trực tiếp ở đây
    /// vì stored procedure mới xử lý đúng logic kiểm tra tồn kho và transaction.
    /// </remarks>
    public int Insert(BorrowReceipt entity)
        => CreateBorrow(entity, entity.Details);

    /// <inheritdoc/>
    public bool Update(BorrowReceipt entity) => Execute(nameof(Update), () =>
    {
        // Chỉ cho update các trường an toàn (note, dueDate khi gia hạn).
        // KHÔNG cho phép sửa Status thủ công — trạng thái được trigger/SP quản lý.
        const string sql = """
            UPDATE dbo.BorrowReceipts
               SET DueDate    = @Due,
                   Note       = @Note,
                   RenewCount = @Renew,
                   UpdatedAt  = SYSUTCDATETIME(),
                   UpdatedBy  = @UpdatedBy
             WHERE BorrowId = @Id AND IsDeleted = 0;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = entity.BorrowId;
        cmd.Parameters.Add("@Due", SqlDbType.Date).Value = entity.DueDate.ToDateTime(TimeOnly.MinValue);
        cmd.Parameters.Add("@Note", SqlDbType.NVarChar, 300).Value = entity.Note.ToDbValue();
        cmd.Parameters.Add("@Renew", SqlDbType.Int).Value = entity.RenewCount;
        cmd.Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = entity.UpdatedBy.ToDbValue();
        return cmd.ExecuteNonQuery() > 0;
    });

    /// <inheritdoc/>
    public bool Delete(int id) => Execute(nameof(Delete), () =>
    {
        // Chỉ huỷ phiếu chưa giao sách. Không cho xoá phiếu đã có sách mượn ra.
        const string sql = """
            IF EXISTS (
                SELECT 1 FROM dbo.BorrowReceiptDetails d
                WHERE d.BorrowId = @Id AND d.ReturnedQty < d.Quantity
            ) AND (
                SELECT Status FROM dbo.BorrowReceipts WHERE BorrowId = @Id
            ) <> 'Cancelled'
                THROW 50102, N'Phiếu còn sách chưa trả, không thể xoá.', 1;

            UPDATE dbo.BorrowReceipts
               SET IsDeleted = 1,
                   DeletedAt = SYSUTCDATETIME(),
                   Status    = 'Cancelled'
             WHERE BorrowId = @Id AND IsDeleted = 0;

            SELECT @@ROWCOUNT;
            """;
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        return (int)cmd.ExecuteScalar()! > 0;
    });

    /// <inheritdoc/>
    public List<BorrowReceipt> Search(string? receiptCode, int? readerId, BorrowStatus? status,
        DateOnly? fromDate, DateOnly? toDate) => Execute(nameof(Search), () =>
        {
            using var conn = OpenConnection();
            using var cmd = BuildSearchCommand(conn, receiptCode, readerId, status, fromDate, toDate);
            return ReadList(cmd, BorrowReceiptMapper.Map);
        });

    /// <inheritdoc/>
    public DataTable SearchAsDataTable(string? receiptCode, int? readerId, BorrowStatus? status,
        DateOnly? fromDate, DateOnly? toDate) => Execute(nameof(SearchAsDataTable), () =>
        {
            // Query JOIN để hiển thị thân thiện (tên độc giả, tên thủ thư)
            var sql = """
            SELECT br.BorrowId, br.ReceiptCode,
                   r.CardNumber, r.FullName AS ReaderName,
                   u.FullName AS LibrarianName,
                   br.BorrowDate, br.DueDate, br.Status, br.TotalFine
            FROM dbo.BorrowReceipts br
            JOIN dbo.Readers r ON r.ReaderId = br.ReaderId
            JOIN dbo.Users   u ON u.UserId   = br.UserId
            WHERE br.IsDeleted = 0
              AND (@Code   IS NULL OR br.ReceiptCode LIKE @Code + '%')
              AND (@Reader IS NULL OR br.ReaderId    = @Reader)
              AND (@Status IS NULL OR br.Status      = @Status)
              AND (@From   IS NULL OR br.BorrowDate  >= @From)
              AND (@To     IS NULL OR br.BorrowDate  <= @To)
            ORDER BY br.BorrowDate DESC;
            """;
            using var conn = OpenConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@Code", SqlDbType.VarChar, 30).Value = (object?)receiptCode ?? DBNull.Value;
            cmd.Parameters.Add("@Reader", SqlDbType.Int).Value = readerId.ToDbValue();
            cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = status?.ToString() ?? (object)DBNull.Value;
            cmd.Parameters.Add("@From", SqlDbType.Date).Value = fromDate.HasValue
                                                                         ? fromDate.Value.ToDateTime(TimeOnly.MinValue)
                                                                         : DBNull.Value;
            cmd.Parameters.Add("@To", SqlDbType.Date).Value = toDate.HasValue
                                                                         ? toDate.Value.ToDateTime(TimeOnly.MinValue)
                                                                         : DBNull.Value;
            return ReadDataTable(cmd);
        });

    /// <inheritdoc/>
    public int CreateBorrow(BorrowReceipt receipt, IEnumerable<BorrowReceiptDetail> items)
        => Execute(nameof(CreateBorrow), () =>
        {
            var itemList = items.ToList();
            if (itemList.Count == 0)
                throw new DalException("Phiếu mượn phải có ít nhất một dòng sách.");

            // Tạo DataTable khớp với Table-Valued Type dbo.BorrowItemList
            var itemsTable = new DataTable();
            itemsTable.Columns.Add("BookId", typeof(int));
            itemsTable.Columns.Add("Quantity", typeof(int));
            foreach (var it in itemList)
                itemsTable.Rows.Add(it.BookId, it.Quantity);

            using var conn = OpenConnection();
            using var cmd = new SqlCommand("dbo.sp_Borrow_Create", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add("@ReceiptCode", SqlDbType.VarChar, 30).Value = receipt.ReceiptCode;
            cmd.Parameters.Add("@ReaderId", SqlDbType.Int).Value = receipt.ReaderId;
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = receipt.UserId;
            cmd.Parameters.Add("@BorrowDate", SqlDbType.Date).Value = receipt.BorrowDate.ToDateTime(TimeOnly.MinValue);
            cmd.Parameters.Add("@DueDate", SqlDbType.Date).Value = receipt.DueDate.ToDateTime(TimeOnly.MinValue);
            cmd.Parameters.Add("@Note", SqlDbType.NVarChar, 300).Value = receipt.Note.ToDbValue();

            var tvp = cmd.Parameters.AddWithValue("@Items", itemsTable);
            tvp.SqlDbType = SqlDbType.Structured;
            tvp.TypeName = "dbo.BorrowItemList";

            var outParam = cmd.Parameters.Add("@NewBorrowId", SqlDbType.Int);
            outParam.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();
            var newId = (int)outParam.Value!;
            receipt.BorrowId = newId;
            return newId;
        });

    /// <inheritdoc/>
    public int CreateReturn(ReturnReceipt returnReceipt, IEnumerable<ReturnReceiptDetail> items)
        => Execute(nameof(CreateReturn), () =>
        {
            var itemList = items.ToList();
            if (itemList.Count == 0)
                throw new DalException("Phiếu trả phải có ít nhất một dòng sách.");

            // DataTable khớp Table-Valued Type dbo.ReturnItemList
            var itemsTable = new DataTable();
            itemsTable.Columns.Add("BorrowDetailId", typeof(int));
            itemsTable.Columns.Add("Quantity", typeof(int));
            itemsTable.Columns.Add("Condition", typeof(string));
            itemsTable.Columns.Add("Fine", typeof(decimal));
            foreach (var it in itemList)
                itemsTable.Rows.Add(it.BorrowDetailId, it.Quantity, it.Condition.ToString(), it.Fine);

            using var conn = OpenConnection();
            using var cmd = new SqlCommand("dbo.sp_Return_Create", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add("@ReturnCode", SqlDbType.VarChar, 30).Value = returnReceipt.ReturnCode;
            cmd.Parameters.Add("@BorrowId", SqlDbType.Int).Value = returnReceipt.BorrowId;
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = returnReceipt.UserId;
            cmd.Parameters.Add("@ReturnDate", SqlDbType.Date).Value = returnReceipt.ReturnDate.ToDateTime(TimeOnly.MinValue);
            cmd.Parameters.Add("@Note", SqlDbType.NVarChar, 300).Value = returnReceipt.Note.ToDbValue();

            var tvp = cmd.Parameters.AddWithValue("@Items", itemsTable);
            tvp.SqlDbType = SqlDbType.Structured;
            tvp.TypeName = "dbo.ReturnItemList";

            var outParam = cmd.Parameters.Add("@NewReturnId", SqlDbType.Int);
            outParam.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();
            var newId = (int)outParam.Value!;
            returnReceipt.ReturnId = newId;
            return newId;
        });

    private static SqlCommand BuildSearchCommand(SqlConnection conn, string? receiptCode, int? readerId,
        BorrowStatus? status, DateOnly? fromDate, DateOnly? toDate)
    {
        var sql = $"""
            SELECT {BorrowReceiptMapper.SelectColumns}
            FROM dbo.BorrowReceipts
            WHERE IsDeleted = 0
              AND (@Code   IS NULL OR ReceiptCode LIKE @Code + '%')
              AND (@Reader IS NULL OR ReaderId    = @Reader)
              AND (@Status IS NULL OR Status      = @Status)
              AND (@From   IS NULL OR BorrowDate  >= @From)
              AND (@To     IS NULL OR BorrowDate  <= @To)
            ORDER BY BorrowDate DESC;
            """;
        var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Code", SqlDbType.VarChar, 30).Value = (object?)receiptCode ?? DBNull.Value;
        cmd.Parameters.Add("@Reader", SqlDbType.Int).Value = readerId.ToDbValue();
        cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = status?.ToString() ?? (object)DBNull.Value;
        cmd.Parameters.Add("@From", SqlDbType.Date).Value = fromDate.HasValue
                                                                    ? fromDate.Value.ToDateTime(TimeOnly.MinValue)
                                                                    : DBNull.Value;
        cmd.Parameters.Add("@To", SqlDbType.Date).Value = toDate.HasValue
                                                                    ? toDate.Value.ToDateTime(TimeOnly.MinValue)
                                                                    : DBNull.Value;
        return cmd;
    }
}
