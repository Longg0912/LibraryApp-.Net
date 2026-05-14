using System.Data;
using LibraryApp.BLL.Dtos;

namespace LibraryApp.BLL.Interfaces;

/// <summary>
/// Service báo cáo / thống kê. Trả về <see cref="DataTable"/> để bind trực tiếp
/// vào <c>DataGridView</c> hoặc <c>Chart</c>, không cần object trung gian.
/// </summary>
public interface IReportService
{
    /// <summary>Lấy KPI tổng quan cho dashboard.</summary>
    DashboardKpi GetDashboardKpi();

    /// <summary>Top sách được mượn nhiều nhất trong khoảng thời gian.</summary>
    DataTable GetTopBorrowedBooks(DateOnly? fromDate, DateOnly? toDate, int topN = 10);

    /// <summary>Top độc giả hoạt động nhiều nhất.</summary>
    DataTable GetTopActiveReaders(DateOnly? fromDate, DateOnly? toDate, int topN = 10);

    /// <summary>Doanh thu tiền phạt theo ngày/tháng/năm.</summary>
    /// <param name="groupBy">"Day", "Month" hoặc "Year".</param>
    DataTable GetFineRevenue(DateOnly fromDate, DateOnly toDate, string groupBy = "Day");

    /// <summary>Chi tiết sách quá hạn tại thời điểm <paramref name="asOfDate"/>.</summary>
    DataTable GetOverdueBooks(DateOnly? asOfDate = null);

    /// <summary>Danh sách sách đang được mượn (có thể lọc theo danh mục).</summary>
    DataTable GetBooksCurrentlyBorrowed(int? categoryId = null);
}
