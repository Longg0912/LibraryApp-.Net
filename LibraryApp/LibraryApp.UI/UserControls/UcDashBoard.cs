using LibraryApp.BLL.Dtos;
using LibraryApp.UI.Common;

namespace LibraryApp.UI.UserControls;

/// <summary>
/// Dashboard tổng quan hiển thị khi mở MainForm. Bao gồm 8 thẻ KPI lấy từ
/// view <c>vw_Dashboard_KPI</c> qua <c>ReportService.GetDashboardKpi()</c>.
/// </summary>
public sealed class UcDashboard : UserControl
{
    private readonly FlowLayoutPanel _flow;
    private readonly Label _lblError;

    /// <summary>Khởi tạo UserControl + tự load KPI sau khi handle được tạo.</summary>
    public UcDashboard()
    {
        BackColor = System.Drawing.Color.FromArgb(245, 247, 251);

        _flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = System.Drawing.Color.Transparent,
            Padding = new Padding(0)
        };

        _lblError = new Label
        {
            Dock = DockStyle.Top,
            Visible = false,
            BackColor = System.Drawing.Color.FromArgb(252, 235, 235),
            ForeColor = System.Drawing.Color.FromArgb(178, 50, 50),
            Font = new System.Drawing.Font("Segoe UI", 10F),
            Height = 36,
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 16, 0)
        };

        Controls.Add(_flow);
        Controls.Add(_lblError);

        // Load KPI ngay sau khi control được add vào parent
        Load += (_, _) => LoadKpiAsync();
    }

    /// <summary>
    /// Load KPI từ ReportService bất đồng bộ và bind vào các thẻ.
    /// </summary>
    private async void LoadKpiAsync()
    {
        _flow.Controls.Clear();
        _lblError.Visible = false;

        try
        {
            var kpi = await Task.Run(ServiceLocator.Reports.GetDashboardKpi);
            BuildKpiCards(kpi);
        }
        catch (Exception ex)
        {
            _lblError.Text = $"⚠ Không thể tải dữ liệu dashboard: {ex.Message}";
            _lblError.Visible = true;
        }
    }

    /// <summary>Tạo 8 thẻ KPI từ <see cref="DashboardKpi"/>.</summary>
    private void BuildKpiCards(DashboardKpi kpi)
    {
        AddCard("📚", "Tổng số đầu sách", kpi.TotalBooks.ToString("N0"), "#3B82F6");
        AddCard("📦", "Tổng số bản copy", kpi.TotalCopies.ToString("N0"), "#10B981");
        AddCard("🔄", "Đang được mượn", kpi.TotalBorrowedNow.ToString("N0"), "#F59E0B");
        AddCard("👥", "Độc giả hoạt động", kpi.ActiveReaders.ToString("N0"), "#8B5CF6");
        AddCard("📋", "Phiếu mượn mở", kpi.ActiveBorrows.ToString("N0"), "#06B6D4");
        AddCard("⏰", "Quá hạn", kpi.OverdueBorrows.ToString("N0"), "#EF4444");
        AddCard("💰", "Phạt 30 ngày (VNĐ)", kpi.FineRevenue30d.ToString("N0"), "#EC4899");
        AddCard("🔖", "Đặt trước chờ", kpi.PendingReservations.ToString("N0"), "#6366F1");
    }

    /// <summary>
    /// Thêm một thẻ KPI vào FlowLayoutPanel. Mỗi thẻ là một panel với icon,
    /// nhãn mô tả, và giá trị số.
    /// </summary>
    private void AddCard(string icon, string label, string value, string accentHex)
    {
        var card = new Panel
        {
            Width = 240,
            Height = 130,
            Margin = new Padding(0, 0, 16, 16),
            BackColor = System.Drawing.Color.White,
            Padding = new Padding(20, 16, 20, 16)
        };

        // Viền màu accent ở mép trái
        var accent = new Panel
        {
            Dock = DockStyle.Left,
            Width = 4,
            BackColor = ColorFromHex(accentHex)
        };

        var lblIcon = new Label
        {
            AutoSize = true,
            Font = new System.Drawing.Font("Segoe UI Emoji", 18F),
            Text = icon,
            Location = new System.Drawing.Point(20, 14)
        };

        var lblLabel = new Label
        {
            AutoSize = false,
            Font = new System.Drawing.Font("Segoe UI", 9F),
            ForeColor = System.Drawing.Color.FromArgb(120, 120, 120),
            Text = label.ToUpperInvariant(),
            Location = new System.Drawing.Point(60, 20),
            Size = new System.Drawing.Size(160, 20)
        };

        var lblValue = new Label
        {
            AutoSize = false,
            Font = new System.Drawing.Font("Segoe UI Semibold", 22F, System.Drawing.FontStyle.Bold),
            ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Text = value,
            Location = new System.Drawing.Point(20, 60),
            Size = new System.Drawing.Size(200, 50)
        };

        card.Controls.Add(lblValue);
        card.Controls.Add(lblLabel);
        card.Controls.Add(lblIcon);
        card.Controls.Add(accent);

        _flow.Controls.Add(card);
    }

    /// <summary>Chuyển chuỗi hex "#RRGGBB" thành <see cref="System.Drawing.Color"/>.</summary>
    private static System.Drawing.Color ColorFromHex(string hex)
    {
        hex = hex.TrimStart('#');
        return System.Drawing.Color.FromArgb(
            Convert.ToInt32(hex[..2], 16),
            Convert.ToInt32(hex.Substring(2, 2), 16),
            Convert.ToInt32(hex.Substring(4, 2), 16));
    }
}
