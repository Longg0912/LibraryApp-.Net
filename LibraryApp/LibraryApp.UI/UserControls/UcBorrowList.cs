using System.Data;
using LibraryApp.Models.Enums;
using LibraryApp.UI.Common;
using LibraryApp.UI.Forms;

namespace LibraryApp.UI.UserControls;

/// <summary>
/// UserControl danh sách phiếu mượn với khả năng lọc + mở dialog ghi nhận trả.
/// </summary>
public sealed class UcBorrowList : UserControl
{
    private DataGridView _grid = null!;
    private TextBox _txtKeyword = null!;
    private ComboBox _cboStatus = null!;
    private DateTimePicker _dtpFrom = null!;
    private DateTimePicker _dtpTo = null!;
    private CheckBox _chkAllDates = null!;
    private Button _btnSearch = null!;
    private Button _btnReload = null!;
    private Button _btnReturn = null!;
    private Button _btnViewDetail = null!;
    private Label _lblCount = null!;

    public UcBorrowList()
    {
        BackColor = System.Drawing.Color.FromArgb(245, 247, 251);
        Size = new System.Drawing.Size(1100, 700);
        BuildUi();
        Load += (_, _) => Reload();
    }

    // ================================================================
    // UI build (code-only, không dùng Designer file riêng cho gọn)
    // ================================================================

    private void BuildUi()
    {
        // Filter toolbar
        var pnlFilter = new Panel
        {
            BackColor = System.Drawing.Color.White,
            Dock = DockStyle.Top,
            Height = 65,
            Padding = new Padding(12, 14, 12, 14)
        };

        var lblKw = new Label
        {
            AutoSize = true,
            Text = "Từ khoá:",
            Font = new System.Drawing.Font("Segoe UI", 9F),
            ForeColor = System.Drawing.Color.FromArgb(80, 80, 80),
            Location = new System.Drawing.Point(12, 22)
        };

        _txtKeyword = new TextBox
        {
            BorderStyle = BorderStyle.FixedSingle,
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            PlaceholderText = "Mã phiếu / Số thẻ / Họ tên",
            Location = new System.Drawing.Point(74, 18),
            Size = new System.Drawing.Size(220, 25)
        };
        _txtKeyword.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Reload(); } };

        var lblStatus = new Label
        {
            AutoSize = true,
            Text = "Trạng thái:",
            Font = new System.Drawing.Font("Segoe UI", 9F),
            ForeColor = System.Drawing.Color.FromArgb(80, 80, 80),
            Location = new System.Drawing.Point(312, 22)
        };

        _cboStatus = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat,
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            Location = new System.Drawing.Point(385, 18),
            Size = new System.Drawing.Size(140, 25)
        };

        var lblFrom = new Label
        {
            AutoSize = true,
            Text = "Từ:",
            Font = new System.Drawing.Font("Segoe UI", 9F),
            ForeColor = System.Drawing.Color.FromArgb(80, 80, 80),
            Location = new System.Drawing.Point(540, 22)
        };
        _dtpFrom = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            Location = new System.Drawing.Point(570, 18),
            Size = new System.Drawing.Size(105, 25),
            Value = DateTime.Today.AddMonths(-1)
        };

        var lblTo = new Label
        {
            AutoSize = true,
            Text = "đến:",
            Font = new System.Drawing.Font("Segoe UI", 9F),
            ForeColor = System.Drawing.Color.FromArgb(80, 80, 80),
            Location = new System.Drawing.Point(685, 22)
        };
        _dtpTo = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            Location = new System.Drawing.Point(720, 18),
            Size = new System.Drawing.Size(105, 25),
            Value = DateTime.Today
        };

        _chkAllDates = new CheckBox
        {
            AutoSize = true,
            Text = "Tất cả",
            Font = new System.Drawing.Font("Segoe UI", 9F),
            ForeColor = System.Drawing.Color.FromArgb(80, 80, 80),
            Location = new System.Drawing.Point(835, 21)
        };
        _chkAllDates.CheckedChanged += (_, _) =>
        {
            _dtpFrom.Enabled = !_chkAllDates.Checked;
            _dtpTo.Enabled = !_chkAllDates.Checked;
        };

        _btnSearch = new Button
        {
            BackColor = System.Drawing.Color.FromArgb(33, 64, 154),
            FlatStyle = FlatStyle.Flat,
            Font = new System.Drawing.Font("Segoe UI Semibold", 9F),
            ForeColor = System.Drawing.Color.White,
            Location = new System.Drawing.Point(910, 16),
            Size = new System.Drawing.Size(85, 30),
            Text = "🔍 Tìm",
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        _btnSearch.FlatAppearance.BorderSize = 0;
        _btnSearch.Click += (_, _) => Reload();

        _btnReload = new Button
        {
            BackColor = System.Drawing.Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new System.Drawing.Font("Segoe UI Semibold", 9F),
            ForeColor = System.Drawing.Color.FromArgb(60, 60, 60),
            Location = new System.Drawing.Point(1005, 16),
            Size = new System.Drawing.Size(85, 30),
            Text = "♻ Reset",
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        _btnReload.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
        _btnReload.Click += (_, _) =>
        {
            _txtKeyword.Clear();
            _cboStatus.SelectedIndex = 0;
            _chkAllDates.Checked = false;
            _dtpFrom.Value = DateTime.Today.AddMonths(-1);
            _dtpTo.Value = DateTime.Today;
            Reload();
        };

        pnlFilter.Controls.AddRange(new Control[] {
            lblKw, _txtKeyword, lblStatus, _cboStatus,
            lblFrom, _dtpFrom, lblTo, _dtpTo, _chkAllDates,
            _btnSearch, _btnReload
        });

        // Action footer
        var pnlFooter = new Panel
        {
            BackColor = System.Drawing.Color.White,
            Dock = DockStyle.Bottom,
            Height = 60,
            Padding = new Padding(20, 12, 20, 12)
        };

        _lblCount = new Label
        {
            AutoSize = false,
            Font = new System.Drawing.Font("Segoe UI Semibold", 10F),
            ForeColor = System.Drawing.Color.FromArgb(80, 80, 80),
            Location = new System.Drawing.Point(20, 18),
            Size = new System.Drawing.Size(400, 24),
            Text = ""
        };

        _btnViewDetail = new Button
        {
            BackColor = System.Drawing.Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F),
            ForeColor = System.Drawing.Color.FromArgb(60, 60, 60),
            Location = new System.Drawing.Point(620, 12),
            Size = new System.Drawing.Size(140, 36),
            Text = "🔎 Xem chi tiết",
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        _btnViewDetail.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
        _btnViewDetail.Click += (_, _) => ShowDetailDialog();

        var btnExport = new Button
        {
            BackColor = System.Drawing.Color.FromArgb(16, 185, 129),
            FlatStyle = FlatStyle.Flat,
            Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F),
            ForeColor = System.Drawing.Color.White,
            Location = new System.Drawing.Point(770, 12),
            Size = new System.Drawing.Size(140, 36),
            Text = "📊 Xuất Excel",
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        btnExport.FlatAppearance.BorderSize = 0;
        btnExport.Click += (_, _) => ExportToExcel();

        _btnReturn = new Button
        {
            BackColor = System.Drawing.Color.FromArgb(33, 64, 154),
            FlatStyle = FlatStyle.Flat,
            Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold),
            ForeColor = System.Drawing.Color.White,
            Location = new System.Drawing.Point(920, 12),
            Size = new System.Drawing.Size(160, 36),
            Text = "✓ GHI NHẬN TRẢ",
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        _btnReturn.FlatAppearance.BorderSize = 0;
        _btnReturn.Click += (_, _) => OpenReturnDialog();

        pnlFooter.Controls.AddRange(new Control[] { _lblCount, _btnViewDetail, btnExport, _btnReturn });

        // Grid
        _grid = new DataGridView
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
            ColumnHeadersHeight = 38,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        };
        _grid.RowTemplate.Height = 32;
        _grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251),
            ForeColor = System.Drawing.Color.FromArgb(60, 60, 60),
            Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold),
            Padding = new Padding(8, 0, 0, 0)
        };
        _grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.White,
            ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            SelectionBackColor = System.Drawing.Color.FromArgb(218, 232, 252),
            SelectionForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Padding = new Padding(8, 0, 0, 0)
        };
        _grid.CellFormatting += Grid_CellFormatting;
        _grid.CellDoubleClick += (_, _) => ShowDetailDialog();

        // Status combo
        var statusItems = new List<StatusItem>
{
    new StatusItem(null, "-- Tất cả --"),
    new StatusItem(BorrowStatus.Borrowing, "Đang mượn"),
    new StatusItem(BorrowStatus.PartiallyReturned, "Trả 1 phần"),
    new StatusItem(BorrowStatus.Returned, "Đã trả hết"),
    new StatusItem(BorrowStatus.Overdue, "Quá hạn"),
    new StatusItem(BorrowStatus.Cancelled, "Đã huỷ")
};

        _cboStatus.DisplayMember = "Display";
        _cboStatus.ValueMember = "Value";
        _cboStatus.DataSource = statusItems;

        if (_cboStatus.Items.Count > 0)
            _cboStatus.SelectedIndex = 0;

        Controls.AddRange(new Control[] { _grid, pnlFooter, pnlFilter });
    }

    // ================================================================
    // Data loading
    // ================================================================

    private void Reload()
    {
        try
        {
            UseWaitCursor = true;

            string? keyword = string.IsNullOrWhiteSpace(_txtKeyword.Text) ? null : _txtKeyword.Text.Trim();
            BorrowStatus? status = (_cboStatus.SelectedItem as StatusItem)?.Value;

            DateOnly? from = _chkAllDates.Checked ? null : DateOnly.FromDateTime(_dtpFrom.Value);
            DateOnly? to = _chkAllDates.Checked ? null : DateOnly.FromDateTime(_dtpTo.Value);

            var table = ServiceLocator.Borrow.Search(keyword, null, status, from, to);
            _grid.DataSource = table;
            _lblCount.Text = $"Tìm thấy {table.Rows.Count:N0} phiếu mượn.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            UseWaitCursor = false;
        }
    }

    private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        var col = _grid.Columns[e.ColumnIndex];
        if (col.Name == "Status" && e.Value is string s
            && Enum.TryParse<BorrowStatus>(s, out var bs))
        {
            e.Value = bs switch
            {
                BorrowStatus.Borrowing => "Đang mượn",
                BorrowStatus.PartiallyReturned => "Trả 1 phần",
                BorrowStatus.Returned => "Đã trả hết",
                BorrowStatus.Overdue => "Quá hạn",
                BorrowStatus.Cancelled => "Đã huỷ",
                _ => bs.ToString()
            };
            e.FormattingApplied = true;

            // Tô màu cảnh báo cho Overdue
            if (bs == BorrowStatus.Overdue)
                _grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(220, 53, 69);
        }
    }

    // ================================================================
    // Detail dialog + Return dialog
    // ================================================================

    private int? GetSelectedBorrowId()
    {
        if (_grid.CurrentRow?.DataBoundItem is DataRowView drv)
            return Convert.ToInt32(drv["BorrowId"]);
        return null;
    }

    private void ShowDetailDialog()
    {
        var id = GetSelectedBorrowId();
        if (id is null)
        {
            MessageBox.Show("Vui lòng chọn một phiếu mượn.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            var detail = ServiceLocator.Borrow.GetDetail(id.Value);
            if (detail is null)
            {
                MessageBox.Show("Phiếu mượn không còn tồn tại.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Reload();
                return;
            }

            var lines = string.Join("\n", detail.Details.Select(d =>
                $"  • BookId {d.BookId}: mượn {d.Quantity}, đã trả {d.ReturnedQty}"));

            MessageBox.Show(
                $"Mã phiếu: {detail.ReceiptCode}\n" +
                $"Trạng thái: {detail.Status}\n" +
                $"Ngày mượn: {detail.BorrowDate:dd/MM/yyyy}\n" +
                $"Hạn trả: {detail.DueDate:dd/MM/yyyy}\n" +
                $"Tổng phạt: {detail.TotalFine:N0} VNĐ\n\n" +
                $"Chi tiết:\n{lines}",
                "Chi tiết phiếu mượn",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OpenReturnDialog()
    {
        var id = GetSelectedBorrowId();
        if (id is null)
        {
            MessageBox.Show("Vui lòng chọn phiếu mượn cần ghi nhận trả.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new FrmReturn(id.Value);
        if (dlg.ShowDialog(this) == DialogResult.OK)
            Reload();
    }

    /// <summary>Xuất danh sách phiếu mượn hiện tại ra Excel.</summary>
    private void ExportToExcel()
    {
        ExcelExporter.ExportDataGridView(
            _grid,
            defaultFileName: "DanhSachPhieuMuon",
            sheetName: "Phiếu mượn",
            title: "DANH SÁCH PHIẾU MƯỢN",
            owner: FindForm());
    }

    private sealed class StatusItem
    {
        public BorrowStatus? Value { get; set; }
        public string Display { get; set; }

        public StatusItem(BorrowStatus? value, string display)
        {
            Value = value;
            Display = display;
        }

        public override string ToString() => Display;
    }
}
