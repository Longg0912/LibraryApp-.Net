namespace LibraryApp.BLL.Dtos;

/// <summary>
/// Bộ chỉ số tổng quan cho dashboard trang chủ.
/// Map 1-1 với view <c>dbo.vw_Dashboard_KPI</c>.
/// </summary>
public sealed class DashboardKpi
{
    /// <summary>Tổng số đầu sách (tựa) chưa bị retire.</summary>
    public int TotalBooks { get; set; }

    /// <summary>Tổng số bản copy (tính tất cả).</summary>
    public int TotalCopies { get; set; }

    /// <summary>Tổng số bản đang được mượn.</summary>
    public int TotalBorrowedNow { get; set; }

    /// <summary>Số độc giả đang hoạt động.</summary>
    public int ActiveReaders { get; set; }

    /// <summary>Số phiếu mượn đang mở (chưa trả xong).</summary>
    public int ActiveBorrows { get; set; }

    /// <summary>Số phiếu mượn quá hạn.</summary>
    public int OverdueBorrows { get; set; }

    /// <summary>Doanh thu tiền phạt 30 ngày gần nhất (VNĐ).</summary>
    public decimal FineRevenue30d { get; set; }

    /// <summary>Số lượt đặt trước sách đang chờ.</summary>
    public int PendingReservations { get; set; }
}
