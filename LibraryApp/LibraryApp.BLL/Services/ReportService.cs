using LibraryApp.BLL.Common;
using LibraryApp.BLL.Dtos;
using LibraryApp.BLL.Interfaces;
using LibraryApp.BLL.Validation;

using LibraryApp.DAL;
using LibraryApp.DAL.Common;

using Microsoft.Data.SqlClient;

using System.Data;

namespace LibraryApp.BLL.Services;

/// <summary>
/// Service báo cáo / thống kê. Mọi truy vấn gọi thẳng stored procedure
/// hoặc view đã được tối ưu phía SQL Server và trả về <see cref="DataTable"/>
/// để bind trực tiếp vào <c>DataGridView</c> / <c>Chart</c>.
/// </summary>
/// <remarks>
/// Service này không cần repository riêng vì chỉ thực thi truy vấn read-only
/// và chuyển dữ liệu thô lên UI. Việc đi thẳng qua <see cref="DatabaseConnection"/>
/// chấp nhận được vì các SP/view ở đây đều đã được test trong script SQL.
/// </remarks>
public sealed class ReportService : IReportService
{
    /// <inheritdoc/>
    public DashboardKpi GetDashboardKpi()
    {
        const string sql = "SELECT * FROM dbo.vw_Dashboard_KPI;";
        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        using var rd = cmd.ExecuteReader();
        if (!rd.Read())
            return new DashboardKpi();

        return new DashboardKpi
        {
            TotalBooks = rd.GetInt32(rd.GetOrdinal("TotalBooks")),
            TotalCopies = rd.GetInt32(rd.GetOrdinal("TotalCopies")),
            TotalBorrowedNow = rd.GetInt32(rd.GetOrdinal("TotalBorrowedNow")),
            ActiveReaders = rd.GetInt32(rd.GetOrdinal("ActiveReaders")),
            ActiveBorrows = rd.GetInt32(rd.GetOrdinal("ActiveBorrows")),
            OverdueBorrows = rd.GetInt32(rd.GetOrdinal("OverdueBorrows")),
            FineRevenue30d = rd.GetDecimal(rd.GetOrdinal("FineRevenue30d")),
            PendingReservations = rd.GetInt32(rd.GetOrdinal("PendingReservations"))
        };
    }

    /// <inheritdoc/>
    public DataTable GetTopBorrowedBooks(DateOnly? fromDate, DateOnly? toDate, int topN = 10)
    {
        ValidateDateRange(fromDate, toDate);
        Validator.Range(topN, "Top N", 1, 100);

        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand("dbo.sp_Stat_TopBorrowedBooks", conn)
        { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = ToDbDate(fromDate);
        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDbDate(toDate);
        cmd.Parameters.Add("@TopN", SqlDbType.Int).Value = topN;
        return Fill(cmd);
    }

    /// <inheritdoc/>
    public DataTable GetTopActiveReaders(DateOnly? fromDate, DateOnly? toDate, int topN = 10)
    {
        ValidateDateRange(fromDate, toDate);
        Validator.Range(topN, "Top N", 1, 100);

        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand("dbo.sp_Stat_TopActiveReaders", conn)
        { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = ToDbDate(fromDate);
        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDbDate(toDate);
        cmd.Parameters.Add("@TopN", SqlDbType.Int).Value = topN;
        return Fill(cmd);
    }

    /// <inheritdoc/>
    public DataTable GetFineRevenue(DateOnly fromDate, DateOnly toDate, string groupBy = "Day")
    {
        if (fromDate > toDate)
            throw new BusinessException("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

        var validGroups = new[] { "Day", "Month", "Year" };
        if (Array.IndexOf(validGroups, groupBy) < 0)
            throw new BusinessException("Group by phải là 'Day', 'Month' hoặc 'Year'.");

        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand("dbo.sp_Stat_FineRevenue", conn)
        { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromDate.ToDateTime(TimeOnly.MinValue);
        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = toDate.ToDateTime(TimeOnly.MinValue);
        cmd.Parameters.Add("@GroupBy", SqlDbType.VarChar, 10).Value = groupBy;
        return Fill(cmd);
    }

    /// <inheritdoc/>
    public DataTable GetOverdueBooks(DateOnly? asOfDate = null)
    {
        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand("dbo.sp_Stat_OverdueBooks", conn)
        { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.Add("@AsOfDate", SqlDbType.Date).Value = ToDbDate(asOfDate);
        return Fill(cmd);
    }

    /// <inheritdoc/>
    public DataTable GetBooksCurrentlyBorrowed(int? categoryId = null)
    {
        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand("dbo.sp_Stat_BooksCurrentlyBorrowed", conn)
        { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = (object?)categoryId ?? DBNull.Value;
        return Fill(cmd);
    }

    // ----------------------------------------------------------------
    // Helpers
    // ----------------------------------------------------------------

    private static DataTable Fill(SqlCommand cmd)
    {
        using var reader = cmd.ExecuteReader();
        var dt = new DataTable();
        dt.Load(reader);
        return dt;
    }

    private static object ToDbDate(DateOnly? d) =>
        d.HasValue ? d.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value;

    private static void ValidateDateRange(DateOnly? from, DateOnly? to)
    {
        if (from.HasValue && to.HasValue && from > to)
            throw new BusinessException("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
    }
}
