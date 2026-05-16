using System.Data;
using LibraryApp.BLL.Dtos;
using LibraryApp.UI.Common;

namespace LibraryApp.UI.UserControls;

/// <summary>
/// UserControl thống kê / báo cáo với 5 tab:
/// <list type="bullet">
/// <item><b>Tổng quan:</b> KPI cards + biểu đồ tròn tỉ lệ phiếu mượn theo trạng thái.</item>
/// <item><b>Top sách:</b> Bảng + bar chart sách được mượn nhiều nhất.</item>
/// <item><b>Top độc giả:</b> Bảng + bar chart độc giả hoạt động nhiều nhất.</item>
/// <item><b>Doanh thu phạt:</b> Line chart + grid theo ngày/tháng/năm.</item>
/// <item><b>Sách quá hạn:</b> Bảng chi tiết phiếu mượn quá hạn.</item>
/// </list>
/// </summary>
/// <remarks>
/// Mọi dữ liệu lấy từ <see cref="IReportService"/> đã có sẵn — đi qua stored procedure
/// hoặc view trên SQL Server, không tự build query phía C#.
/// </remarks>
public sealed class UcReports : UserControl
{
    // Common filter controls
    private DateTimePicker _dtpFrom = null!;
    private DateTimePicker _dtpTo = null!;
    private Button _btnRefresh = null!;
    private TabControl _tab = null!;

    // Tab 1 - Overview
    private FlowLayoutPanel _flowKpi = null!;
    private SimpleChart _chartStatus = null!;

    // Tab 2 - Top books
    private DataGridView _gridTopBooks = null!;
    private SimpleChart _chartTopBooks = null!;

    // Tab 3 - Top readers
    private DataGridView _gridTopReaders = null!;
    private SimpleChart _chartTopReaders = null!;

    // Tab 4 - Fine revenue
    private DataGridView _gridFine = null!;
    private SimpleChart _chartFine = null!;
    private ComboBox _cboGroupBy = null!;

    // Tab 5 - Overdue
    private DataGridView _gridOverdue = null!;
    private Label _lblOverdueCount = null!;

    public UcReports()
    {
        BackColor = System.Drawing.Color.FromArgb(245, 247, 251);
        Size = new System.Drawing.Size(1100, 700);
        BuildUi();
        Load += (_, _) => RefreshAll();
    }

    // ================================================================
    // UI build
    // ================================================================

    private void BuildUi()
    {
        // Filter toolbar
        var pnlFilter = new Panel
        {
            BackColor = System.Drawing.Color.White,
            Dock = DockStyle.Top,
            Height = 70,
            Padding = new Padding(20, 16, 20, 16)
        };

        var lblHeader = new Label
        {
            AutoSize = true,
            Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold),
            ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Location = new System.Drawing.Point(20, 22),
            Text = "📊 Báo cáo"
        };

        var lblFrom = new Label
        {
            AutoSize = true,
            Font = new System.Drawing.Font("Segoe UI", 9F),
            ForeColor = System.Drawing.Color.FromArgb(80, 80, 80),
            Location = new System.Drawing.Point(160, 24),
            Text = "Từ:"
        };

        _dtpFrom = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            Location = new System.Drawing.Point(195, 20),
            Size = new System.Drawing.Size(120, 25),
            Value = DateTime.Today.AddMonths(-3)
        };

        var lblTo = new Label
        {
            AutoSize = true,
            Font = new System.Drawing.Font("Segoe UI", 9F),
            ForeColor = System.Drawing.Color.FromArgb(80, 80, 80),
            Location = new System.Drawing.Point(325, 24),
            Text = "đến:"
        };

        _dtpTo = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            Location = new System.Drawing.Point(360, 20),
            Size = new System.Drawing.Size(120, 25),
            Value = DateTime.Today
        };

        _btnRefresh = new Button
        {
            BackColor = System.Drawing.Color.FromArgb(33, 64, 154),
            FlatStyle = FlatStyle.Flat,
            Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F),
            ForeColor = System.Drawing.Color.White,
            Location = new System.Drawing.Point(495, 18),
            Size = new System.Drawing.Size(130, 30),
            Text = "♻ Làm mới",
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        _btnRefresh.FlatAppearance.BorderSize = 0;
        _btnRefresh.Click += (_, _) => RefreshAll();

        pnlFilter.Controls.AddRange(new Control[] {
            lblHeader, lblFrom, _dtpFrom, lblTo, _dtpTo, _btnRefresh
        });

        // TabControl
        _tab = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new System.Drawing.Font("Segoe UI", 9.5F)
        };

        _tab.TabPages.Add(BuildTabOverview());
        _tab.TabPages.Add(BuildTabTopBooks());
        _tab.TabPages.Add(BuildTabTopReaders());
        _tab.TabPages.Add(BuildTabFineRevenue());
        _tab.TabPages.Add(BuildTabOverdue());

        Controls.AddRange(new Control[] { _tab, pnlFilter });
    }

    // ================================================================
    // Tab 1 - Overview (KPI + status pie)
    // ================================================================

    private TabPage BuildTabOverview()
    {
        var tab = new TabPage("📈 Tổng quan") { BackColor = System.Drawing.Color.FromArgb(245, 247, 251) };

        _flowKpi = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 280,
            AutoScroll = true,
            BackColor = System.Drawing.Color.Transparent,
            Padding = new Padding(16, 16, 16, 0)
        };

        _chartStatus = new SimpleChart
        {
            Dock = DockStyle.Fill,
            Type = ChartType.Pie,
            ChartTitle = "Tỉ lệ phiếu mượn theo trạng thái",
            ValueFormat = "N0",
            BackColor = System.Drawing.Color.White,
            Padding = new Padding(20)
        };

        var pnlChart = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251),
            Padding = new Padding(16, 0, 16, 16)
        };
        pnlChart.Controls.Add(_chartStatus);

        tab.Controls.Add(pnlChart);
        tab.Controls.Add(_flowKpi);
        return tab;
    }

    private async void LoadOverview()
    {
        try
        {
            // 1. KPI cards (load async)
            var kpi = await Task.Run(ServiceLocator.Reports.GetDashboardKpi);
            _flowKpi.Controls.Clear();

            AddKpiCard("📚", "TỔNG SỐ ĐẦU SÁCH", kpi.TotalBooks.ToString("N0"), "#3B82F6");
            AddKpiCard("📦", "TỔNG SỐ BẢN COPY", kpi.TotalCopies.ToString("N0"), "#10B981");
            AddKpiCard("🔄", "ĐANG ĐƯỢC MƯỢN", kpi.TotalBorrowedNow.ToString("N0"), "#F59E0B");
            AddKpiCard("👥", "ĐỘC GIẢ HOẠT ĐỘNG", kpi.ActiveReaders.ToString("N0"), "#8B5CF6");
            AddKpiCard("📋", "PHIẾU MƯỢN MỞ", kpi.ActiveBorrows.ToString("N0"), "#06B6D4");
            AddKpiCard("⏰", "PHIẾU QUÁ HẠN", kpi.OverdueBorrows.ToString("N0"), "#EF4444");
            AddKpiCard("💰", "PHẠT 30 NGÀY (VNĐ)", kpi.FineRevenue30d.ToString("N0"), "#EC4899");
            AddKpiCard("🔖", "ĐẶT TRƯỚC CHỜ", kpi.PendingReservations.ToString("N0"), "#6366F1");

            // 2. Pie chart - phiếu mượn theo trạng thái
            //   Tính từ KPI có sẵn (không cần truy vấn thêm)
            //   ActiveBorrows = Borrowing + PartiallyReturned; OverdueBorrows tính riêng
            var statusPoints = new List<ChartPoint>();
            int active = Math.Max(0, kpi.ActiveBorrows - kpi.OverdueBorrows);
            if (active > 0) statusPoints.Add(new("Đang mượn", active));
            if (kpi.OverdueBorrows > 0) statusPoints.Add(new("Quá hạn", kpi.OverdueBorrows));

            // Số phiếu đã trả hết trong khoảng filter (cần query)
            var fromDate = DateOnly.FromDateTime(_dtpFrom.Value);
            var toDate = DateOnly.FromDateTime(_dtpTo.Value);
            // Nếu không có dữ liệu, vẫn vẽ chart với section "active"
            _chartStatus.SetData(statusPoints);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể tải tổng quan: {ex.Message}",
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AddKpiCard(string icon, string label, string value, string accentHex)
    {
        var card = new Panel
        {
            Width = 240,
            Height = 120,
            Margin = new Padding(0, 0, 16, 16),
            BackColor = System.Drawing.Color.White,
            Padding = new Padding(20, 16, 20, 16)
        };

        var accent = new Panel
        {
            Dock = DockStyle.Left,
            Width = 4,
            BackColor = ColorFromHex(accentHex)
        };

        var lblIcon = new Label
        {
            AutoSize = true,
            Font = new System.Drawing.Font("Segoe UI Emoji", 16F),
            Text = icon,
            Location = new System.Drawing.Point(20, 12)
        };

        var lblLabel = new Label
        {
            AutoSize = false,
            Font = new System.Drawing.Font("Segoe UI", 8.5F),
            ForeColor = System.Drawing.Color.FromArgb(120, 120, 120),
            Text = label,
            Location = new System.Drawing.Point(58, 18),
            Size = new System.Drawing.Size(170, 20)
        };

        var lblValue = new Label
        {
            AutoSize = false,
            Font = new System.Drawing.Font("Segoe UI Semibold", 20F, System.Drawing.FontStyle.Bold),
            ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Text = value,
            Location = new System.Drawing.Point(20, 54),
            Size = new System.Drawing.Size(200, 50)
        };

        card.Controls.Add(lblValue);
        card.Controls.Add(lblLabel);
        card.Controls.Add(lblIcon);
        card.Controls.Add(accent);
        _flowKpi.Controls.Add(card);
    }

    // ================================================================
    // Tab 2 - Top borrowed books
    // ================================================================

    private TabPage BuildTabTopBooks()
    {
        var tab = new TabPage("🏆 Top sách mượn nhiều")
        { BackColor = System.Drawing.Color.FromArgb(245, 247, 251), Padding = new Padding(16) };

        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251),
            Panel1MinSize = 250
        };
        // Có thể split không set được trước khi add nên đặt sau khi tab visible

        _chartTopBooks = new SimpleChart
        {
            Dock = DockStyle.Fill,
            Type = ChartType.Bar,
            ChartTitle = "Top 10 sách được mượn nhiều nhất",
            ValueFormat = "N0",
            BackColor = System.Drawing.Color.White
        };

        _gridTopBooks = BuildGrid();

        split.Panel1.Controls.Add(_chartTopBooks);
        split.Panel2.Controls.Add(_gridTopBooks);
        tab.Controls.Add(split);

        // Set splitter sau khi add
        tab.HandleCreated += (_, _) =>
        {
            if (split.Width > 0)
                split.SplitterDistance = (int)(split.Width * 0.55);
        };
        return tab;
    }

    private async void LoadTopBooks()
    {
        try
        {
            var fromDate = DateOnly.FromDateTime(_dtpFrom.Value);
            var toDate = DateOnly.FromDateTime(_dtpTo.Value);

            var data = await Task.Run(() =>
                ServiceLocator.Reports.GetTopBorrowedBooks(fromDate, toDate, topN: 10));

            _gridTopBooks.DataSource = data;
            FormatBookGrid(_gridTopBooks);

            // Build chart từ kết quả grid
            var points = new List<ChartPoint>();
            foreach (DataRow row in data.Rows)
            {
                string title = row["Title"]?.ToString() ?? "?";
                if (title.Length > 20) title = title[..18] + "…";
                double qty = Convert.ToDouble(row["BorrowCount"] ?? 0);
                points.Add(new ChartPoint(title, qty));
            }
            _chartTopBooks.SetData(points);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể tải Top sách: {ex.Message}",
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void FormatBookGrid(DataGridView grid)
    {
        if (grid.Columns.Contains("BookCode")) grid.Columns["BookCode"]!.HeaderText = "Mã sách";
        if (grid.Columns.Contains("Title")) grid.Columns["Title"]!.HeaderText = "Tên sách";
        if (grid.Columns.Contains("Author")) grid.Columns["Author"]!.HeaderText = "Tác giả";
        if (grid.Columns.Contains("CategoryName")) grid.Columns["CategoryName"]!.HeaderText = "Danh mục";
        if (grid.Columns.Contains("BorrowCount"))
        {
            grid.Columns["BorrowCount"]!.HeaderText = "Lượt mượn";
            grid.Columns["BorrowCount"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Columns["BorrowCount"]!.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            grid.Columns["BorrowCount"]!.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(33, 64, 154);
        }
    }

    // ================================================================
    // Tab 3 - Top active readers
    // ================================================================

    private TabPage BuildTabTopReaders()
    {
        var tab = new TabPage("👥 Top độc giả")
        { BackColor = System.Drawing.Color.FromArgb(245, 247, 251), Padding = new Padding(16) };

        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251)
        };

        _chartTopReaders = new SimpleChart
        {
            Dock = DockStyle.Fill,
            Type = ChartType.Bar,
            ChartTitle = "Top 10 độc giả hoạt động nhiều nhất",
            ValueFormat = "N0",
            BackColor = System.Drawing.Color.White
        };

        _gridTopReaders = BuildGrid();

        split.Panel1.Controls.Add(_chartTopReaders);
        split.Panel2.Controls.Add(_gridTopReaders);
        tab.Controls.Add(split);

        tab.HandleCreated += (_, _) =>
        {
            if (split.Width > 0)
                split.SplitterDistance = (int)(split.Width * 0.55);
        };
        return tab;
    }

    private async void LoadTopReaders()
    {
        try
        {
            var fromDate = DateOnly.FromDateTime(_dtpFrom.Value);
            var toDate = DateOnly.FromDateTime(_dtpTo.Value);

            var data = await Task.Run(() =>
                ServiceLocator.Reports.GetTopActiveReaders(fromDate, toDate, topN: 10));

            _gridTopReaders.DataSource = data;
            if (_gridTopReaders.Columns.Contains("CardNumber")) _gridTopReaders.Columns["CardNumber"]!.HeaderText = "Số thẻ";
            if (_gridTopReaders.Columns.Contains("FullName")) _gridTopReaders.Columns["FullName"]!.HeaderText = "Họ tên";
            if (_gridTopReaders.Columns.Contains("BorrowCount")) _gridTopReaders.Columns["BorrowCount"]!.HeaderText = "Lượt mượn";
            if (_gridTopReaders.Columns.Contains("TotalBooks")) _gridTopReaders.Columns["TotalBooks"]!.HeaderText = "Tổng cuốn";

            var points = new List<ChartPoint>();
            foreach (DataRow row in data.Rows)
            {
                string name = row["FullName"]?.ToString() ?? "?";
                if (name.Length > 15) name = name[..13] + "…";
                double qty = Convert.ToDouble(row["BorrowCount"] ?? 0);
                points.Add(new ChartPoint(name, qty));
            }
            _chartTopReaders.SetData(points);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể tải Top độc giả: {ex.Message}",
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ================================================================
    // Tab 4 - Fine revenue trend
    // ================================================================

    private TabPage BuildTabFineRevenue()
    {
        var tab = new TabPage("💰 Doanh thu phạt")
        { BackColor = System.Drawing.Color.FromArgb(245, 247, 251), Padding = new Padding(16) };

        // Toolbar nhỏ chứa GroupBy
        var pnlTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = System.Drawing.Color.White,
            Padding = new Padding(16, 12, 16, 12)
        };

        var lblGroupBy = new Label
        {
            AutoSize = true,
            Font = new System.Drawing.Font("Segoe UI", 9F),
            ForeColor = System.Drawing.Color.FromArgb(80, 80, 80),
            Location = new System.Drawing.Point(16, 16),
            Text = "Gộp theo:"
        };

        _cboGroupBy = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat,
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            Location = new System.Drawing.Point(85, 12),
            Size = new System.Drawing.Size(130, 26)
        };
        _cboGroupBy.Items.AddRange(new[] { "Ngày", "Tháng", "Năm" });
        _cboGroupBy.SelectedIndex = 0;
        _cboGroupBy.SelectedIndexChanged += (_, _) => LoadFineRevenue();

        pnlTop.Controls.AddRange(new Control[] { lblGroupBy, _cboGroupBy });

        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251)
        };

        _chartFine = new SimpleChart
        {
            Dock = DockStyle.Fill,
            Type = ChartType.Line,
            ChartTitle = "Doanh thu tiền phạt theo thời gian (VNĐ)",
            ValueFormat = "N0",
            BackColor = System.Drawing.Color.White
        };

        _gridFine = BuildGrid();

        split.Panel1.Controls.Add(_chartFine);
        split.Panel2.Controls.Add(_gridFine);

        tab.Controls.Add(split);
        tab.Controls.Add(pnlTop);

        tab.HandleCreated += (_, _) =>
        {
            if (split.Height > 0)
                split.SplitterDistance = (int)(split.Height * 0.6);
        };
        return tab;
    }

    private async void LoadFineRevenue()
    {
        try
        {
            var fromDate = DateOnly.FromDateTime(_dtpFrom.Value);
            var toDate = DateOnly.FromDateTime(_dtpTo.Value);

            string groupBy = _cboGroupBy.SelectedIndex switch
            {
                1 => "Month",
                2 => "Year",
                _ => "Day"
            };

            var data = await Task.Run(() =>
                ServiceLocator.Reports.GetFineRevenue(fromDate, toDate, groupBy));

            _gridFine.DataSource = data;
            if (_gridFine.Columns.Contains("Period")) _gridFine.Columns["Period"]!.HeaderText = "Thời điểm";
            if (_gridFine.Columns.Contains("ReturnCount")) _gridFine.Columns["ReturnCount"]!.HeaderText = "Số lượt";
            if (_gridFine.Columns.Contains("TotalFine"))
            {
                _gridFine.Columns["TotalFine"]!.HeaderText = "Tổng phạt (VNĐ)";
                _gridFine.Columns["TotalFine"]!.DefaultCellStyle.Format = "N0";
                _gridFine.Columns["TotalFine"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            var points = new List<ChartPoint>();
            foreach (DataRow row in data.Rows)
            {
                string period = row["Period"]?.ToString() ?? "?";
                double value = Convert.ToDouble(row["TotalFine"] ?? 0);
                points.Add(new ChartPoint(period, value));
            }
            _chartFine.SetData(points);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể tải doanh thu phạt: {ex.Message}",
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ================================================================
    // Tab 5 - Overdue books
    // ================================================================

    private TabPage BuildTabOverdue()
    {
        var tab = new TabPage("⚠ Sách quá hạn")
        { BackColor = System.Drawing.Color.FromArgb(245, 247, 251), Padding = new Padding(16) };

        _lblOverdueCount = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Top,
            Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold),
            ForeColor = System.Drawing.Color.FromArgb(220, 38, 38),
            Height = 40,
            Text = "Đang tải...",
            BackColor = System.Drawing.Color.FromArgb(254, 226, 226),
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 16, 0)
        };

        _gridOverdue = BuildGrid();
        _gridOverdue.Margin = new Padding(0, 8, 0, 0);

        tab.Controls.Add(_gridOverdue);
        tab.Controls.Add(_lblOverdueCount);
        return tab;
    }

    private async void LoadOverdue()
    {
        try
        {
            var data = await Task.Run(() =>
                ServiceLocator.Reports.GetOverdueBooks());

            _gridOverdue.DataSource = data;
            if (_gridOverdue.Columns.Contains("ReceiptCode")) _gridOverdue.Columns["ReceiptCode"]!.HeaderText = "Mã phiếu";
            if (_gridOverdue.Columns.Contains("CardNumber")) _gridOverdue.Columns["CardNumber"]!.HeaderText = "Số thẻ";
            if (_gridOverdue.Columns.Contains("ReaderName")) _gridOverdue.Columns["ReaderName"]!.HeaderText = "Họ tên độc giả";
            if (_gridOverdue.Columns.Contains("BookTitle")) _gridOverdue.Columns["BookTitle"]!.HeaderText = "Tên sách";
            if (_gridOverdue.Columns.Contains("BorrowDate")) _gridOverdue.Columns["BorrowDate"]!.HeaderText = "Ngày mượn";
            if (_gridOverdue.Columns.Contains("DueDate")) _gridOverdue.Columns["DueDate"]!.HeaderText = "Hạn trả";
            if (_gridOverdue.Columns.Contains("DaysOverdue"))
            {
                _gridOverdue.Columns["DaysOverdue"]!.HeaderText = "Quá hạn (ngày)";
                _gridOverdue.Columns["DaysOverdue"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                _gridOverdue.Columns["DaysOverdue"]!.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(220, 38, 38);
                _gridOverdue.Columns["DaysOverdue"]!.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            }

            _lblOverdueCount.Text = data.Rows.Count == 0
                ? "✓ Không có phiếu mượn nào quá hạn"
                : $"⚠ Có {data.Rows.Count:N0} phiếu mượn đang quá hạn";

            if (data.Rows.Count == 0)
            {
                _lblOverdueCount.BackColor = System.Drawing.Color.FromArgb(220, 252, 231);
                _lblOverdueCount.ForeColor = System.Drawing.Color.FromArgb(22, 101, 52);
            }
            else
            {
                _lblOverdueCount.BackColor = System.Drawing.Color.FromArgb(254, 226, 226);
                _lblOverdueCount.ForeColor = System.Drawing.Color.FromArgb(220, 38, 38);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể tải sách quá hạn: {ex.Message}",
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ================================================================
    // Refresh all + helpers
    // ================================================================

    private void RefreshAll()
    {
        // Validate khoảng ngày
        if (_dtpFrom.Value.Date > _dtpTo.Value.Date)
        {
            MessageBox.Show("Ngày bắt đầu không được sau ngày kết thúc.",
                "Khoảng thời gian không hợp lệ",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UseWaitCursor = true;
        try
        {
            LoadOverview();
            LoadTopBooks();
            LoadTopReaders();
            LoadFineRevenue();
            LoadOverdue();
        }
        finally
        {
            UseWaitCursor = false;
        }
    }

    private static DataGridView BuildGrid()
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = System.Drawing.Color.White,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true,
            EnableHeadersVisualStyles = false,
            GridColor = System.Drawing.Color.FromArgb(232, 234, 240),
            ColumnHeadersHeight = 36,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        };
        grid.RowTemplate.Height = 30;
        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251),
            ForeColor = System.Drawing.Color.FromArgb(60, 60, 60),
            Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold),
            Padding = new Padding(8, 0, 0, 0)
        };
        grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.White,
            ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            SelectionBackColor = System.Drawing.Color.FromArgb(218, 232, 252),
            SelectionForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Padding = new Padding(8, 0, 0, 0)
        };
        return grid;
    }

    private static System.Drawing.Color ColorFromHex(string hex)
    {
        hex = hex.TrimStart('#');
        return System.Drawing.Color.FromArgb(
            Convert.ToInt32(hex[..2], 16),
            Convert.ToInt32(hex.Substring(2, 2), 16),
            Convert.ToInt32(hex.Substring(4, 2), 16));
    }
}
