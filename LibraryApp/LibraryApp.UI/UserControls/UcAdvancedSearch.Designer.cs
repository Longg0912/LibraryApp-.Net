namespace LibraryApp.UI.UserControls;

partial class UcAdvancedSearch
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Designer-generated code

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();

        // Filter group
        this.pnlFilter = new Panel();
        this.lblHeader = new Label();
        this.lblSubtitle = new Label();
        this.lblEntity = new Label();
        this.cboEntity = new ComboBox();

        // Row 1 - Keyword + Status
        this.lblKeyword = new Label();
        this.txtKeyword = new TextBox();
        this.lblKeywordHint = new Label();
        this.lblStatus = new Label();
        this.cboStatus = new ComboBox();

        // Row 2 - Category + Author (chỉ áp dụng cho Sách)
        this.lblCategory = new Label();
        this.cboCategory = new ComboBox();
        this.lblAuthor = new Label();
        this.txtAuthor = new TextBox();

        // Row 3 - Year range (chỉ áp dụng cho Sách)
        this.lblYearRange = new Label();
        this.numYearFrom = new NumericUpDown();
        this.lblYearSeparator = new Label();
        this.numYearTo = new NumericUpDown();

        // Row 4 - Date range (chỉ áp dụng cho Phiếu mượn)
        this.lblDateRange = new Label();
        this.dtpDateFrom = new DateTimePicker();
        this.lblDateSeparator = new Label();
        this.dtpDateTo = new DateTimePicker();
        this.chkAllDates = new CheckBox();

        // Action row
        this.btnSearch = new Button();
        this.btnReset = new Button();
        this.btnExport = new Button();
        this.lblResultCount = new Label();

        // Result grid
        this.dgvResults = new DataGridView();

        this.pnlFilter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.numYearFrom)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numYearTo)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
        this.SuspendLayout();

        // ============================================================
        // pnlFilter
        // ============================================================
        this.pnlFilter.BackColor = System.Drawing.Color.White;
        this.pnlFilter.Dock = DockStyle.Top;
        this.pnlFilter.Height = 290;
        this.pnlFilter.Padding = new Padding(20);
        this.pnlFilter.Controls.AddRange(new Control[] {
            this.lblHeader, this.lblSubtitle,
            this.lblEntity, this.cboEntity,
            this.lblKeyword, this.txtKeyword, this.lblKeywordHint,
            this.lblStatus, this.cboStatus,
            this.lblCategory, this.cboCategory,
            this.lblAuthor, this.txtAuthor,
            this.lblYearRange, this.numYearFrom, this.lblYearSeparator, this.numYearTo,
            this.lblDateRange, this.dtpDateFrom, this.lblDateSeparator, this.dtpDateTo, this.chkAllDates,
            this.btnSearch, this.btnReset, this.btnExport, this.lblResultCount
        });

        // Header
        this.lblHeader.AutoSize = true;
        this.lblHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold);
        this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
        this.lblHeader.Location = new System.Drawing.Point(20, 12);
        this.lblHeader.Text = "🔎 Tìm kiếm nâng cao";

        this.lblSubtitle.AutoSize = true;
        this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(110, 110, 110);
        this.lblSubtitle.Location = new System.Drawing.Point(22, 42);
        this.lblSubtitle.Text = "Kết hợp nhiều tiêu chí để thu hẹp phạm vi tìm kiếm. Không phân biệt hoa thường.";

        // ===== Row 0 - Entity selector =====
        ConfigureLabel(this.lblEntity, "Tìm trong:", 20, 75);
        this.cboEntity.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboEntity.FlatStyle = FlatStyle.Flat;
        this.cboEntity.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
        this.cboEntity.Location = new System.Drawing.Point(20, 96);
        this.cboEntity.Size = new System.Drawing.Size(200, 26);
        this.cboEntity.SelectedIndexChanged += new EventHandler(this.cboEntity_SelectedIndexChanged);

        // ===== Row 1 - Keyword + Status =====
        ConfigureLabel(this.lblKeyword, "Từ khoá:", 250, 75);
        this.txtKeyword.BorderStyle = BorderStyle.FixedSingle;
        this.txtKeyword.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.txtKeyword.Location = new System.Drawing.Point(250, 96);
        this.txtKeyword.Size = new System.Drawing.Size(350, 25);
        this.txtKeyword.KeyDown += new KeyEventHandler(this.txtKeyword_KeyDown);

        this.lblKeywordHint.AutoSize = true;
        this.lblKeywordHint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic);
        this.lblKeywordHint.ForeColor = System.Drawing.Color.FromArgb(140, 140, 140);
        this.lblKeywordHint.Location = new System.Drawing.Point(252, 122);
        this.lblKeywordHint.Text = "Tìm theo tên, mã, tác giả... (không phân biệt hoa thường)";

        ConfigureLabel(this.lblStatus, "Trạng thái:", 620, 75);
        this.cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboStatus.FlatStyle = FlatStyle.Flat;
        this.cboStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboStatus.Location = new System.Drawing.Point(620, 96);
        this.cboStatus.Size = new System.Drawing.Size(200, 26);

        // ===== Row 2 - Category + Author (Book mode) =====
        ConfigureLabel(this.lblCategory, "Danh mục:", 20, 150);
        this.cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboCategory.FlatStyle = FlatStyle.Flat;
        this.cboCategory.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboCategory.Location = new System.Drawing.Point(20, 171);
        this.cboCategory.Size = new System.Drawing.Size(200, 26);

        ConfigureLabel(this.lblAuthor, "Tác giả:", 250, 150);
        this.txtAuthor.BorderStyle = BorderStyle.FixedSingle;
        this.txtAuthor.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.txtAuthor.Location = new System.Drawing.Point(250, 171);
        this.txtAuthor.Size = new System.Drawing.Size(350, 25);

        // ===== Row 3 - Year range (Book mode) =====
        ConfigureLabel(this.lblYearRange, "Năm xuất bản:", 620, 150);
        this.numYearFrom.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.numYearFrom.Location = new System.Drawing.Point(620, 171);
        this.numYearFrom.Size = new System.Drawing.Size(85, 25);
        this.numYearFrom.Minimum = 1500;
        this.numYearFrom.Maximum = 2100;
        this.numYearFrom.Value = 1500;

        this.lblYearSeparator.AutoSize = true;
        this.lblYearSeparator.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.lblYearSeparator.Location = new System.Drawing.Point(710, 175);
        this.lblYearSeparator.Text = "—";

        this.numYearTo.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.numYearTo.Location = new System.Drawing.Point(735, 171);
        this.numYearTo.Size = new System.Drawing.Size(85, 25);
        this.numYearTo.Minimum = 1500;
        this.numYearTo.Maximum = 2100;
        this.numYearTo.Value = 2100;

        // ===== Row 4 - Date range (Borrow mode) =====
        ConfigureLabel(this.lblDateRange, "Ngày mượn:", 20, 150);
        this.dtpDateFrom.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.dtpDateFrom.Format = DateTimePickerFormat.Short;
        this.dtpDateFrom.Location = new System.Drawing.Point(20, 171);
        this.dtpDateFrom.Size = new System.Drawing.Size(120, 25);
        this.dtpDateFrom.Value = DateTime.Today.AddMonths(-1);

        this.lblDateSeparator.AutoSize = true;
        this.lblDateSeparator.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.lblDateSeparator.Location = new System.Drawing.Point(145, 175);
        this.lblDateSeparator.Text = "—";

        this.dtpDateTo.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.dtpDateTo.Format = DateTimePickerFormat.Short;
        this.dtpDateTo.Location = new System.Drawing.Point(170, 171);
        this.dtpDateTo.Size = new System.Drawing.Size(120, 25);
        this.dtpDateTo.Value = DateTime.Today;

        this.chkAllDates.AutoSize = true;
        this.chkAllDates.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.chkAllDates.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.chkAllDates.Location = new System.Drawing.Point(300, 174);
        this.chkAllDates.Text = "Tất cả thời gian";
        this.chkAllDates.UseVisualStyleBackColor = true;
        this.chkAllDates.CheckedChanged += new EventHandler(this.chkAllDates_CheckedChanged);

        // ===== Action row =====
        this.btnSearch.BackColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.btnSearch.FlatAppearance.BorderSize = 0;
        this.btnSearch.FlatStyle = FlatStyle.Flat;
        this.btnSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
        this.btnSearch.ForeColor = System.Drawing.Color.White;
        this.btnSearch.Location = new System.Drawing.Point(20, 235);
        this.btnSearch.Size = new System.Drawing.Size(150, 38);
        this.btnSearch.Text = "🔍 Tìm kiếm";
        this.btnSearch.Cursor = Cursors.Hand;
        this.btnSearch.UseVisualStyleBackColor = false;
        this.btnSearch.Click += new EventHandler(this.btnSearch_Click);

        this.btnReset.BackColor = System.Drawing.Color.White;
        this.btnReset.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
        this.btnReset.FlatStyle = FlatStyle.Flat;
        this.btnReset.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
        this.btnReset.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.btnReset.Location = new System.Drawing.Point(180, 235);
        this.btnReset.Size = new System.Drawing.Size(130, 38);
        this.btnReset.Text = "♻ Đặt lại";
        this.btnReset.Cursor = Cursors.Hand;
        this.btnReset.UseVisualStyleBackColor = false;
        this.btnReset.Click += new EventHandler(this.btnReset_Click);

        this.btnExport.BackColor = System.Drawing.Color.FromArgb(16, 185, 129);
        this.btnExport.FlatAppearance.BorderSize = 0;
        this.btnExport.FlatStyle = FlatStyle.Flat;
        this.btnExport.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
        this.btnExport.ForeColor = System.Drawing.Color.White;
        this.btnExport.Location = new System.Drawing.Point(320, 235);
        this.btnExport.Size = new System.Drawing.Size(150, 38);
        this.btnExport.Text = "📊 Xuất CSV";
        this.btnExport.Cursor = Cursors.Hand;
        this.btnExport.UseVisualStyleBackColor = false;
        this.btnExport.Click += new EventHandler(this.btnExport_Click);

        this.lblResultCount.AutoSize = false;
        this.lblResultCount.Dock = DockStyle.None;
        this.lblResultCount.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
        this.lblResultCount.ForeColor = System.Drawing.Color.FromArgb(99, 102, 241);
        this.lblResultCount.Location = new System.Drawing.Point(490, 235);
        this.lblResultCount.Size = new System.Drawing.Size(400, 38);
        this.lblResultCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblResultCount.Text = "";

        // ============================================================
        // dgvResults
        // ============================================================
        this.dgvResults.AllowUserToAddRows = false;
        this.dgvResults.AllowUserToDeleteRows = false;
        this.dgvResults.AllowUserToResizeRows = false;
        this.dgvResults.AutoGenerateColumns = true;
        this.dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvResults.BackgroundColor = System.Drawing.Color.White;
        this.dgvResults.BorderStyle = BorderStyle.None;
        this.dgvResults.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        this.dgvResults.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        this.dgvResults.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251),
            ForeColor = System.Drawing.Color.FromArgb(60, 60, 60),
            Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold),
            Alignment = DataGridViewContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 0, 0)
        };
        this.dgvResults.ColumnHeadersHeight = 38;
        this.dgvResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        this.dgvResults.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.White,
            ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            SelectionBackColor = System.Drawing.Color.FromArgb(218, 232, 252),
            SelectionForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Padding = new Padding(8, 0, 0, 0)
        };
        this.dgvResults.RowHeadersVisible = false;
        this.dgvResults.RowTemplate.Height = 32;
        this.dgvResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvResults.MultiSelect = false;
        this.dgvResults.ReadOnly = true;
        this.dgvResults.Dock = DockStyle.Fill;
        this.dgvResults.EnableHeadersVisualStyles = false;
        this.dgvResults.GridColor = System.Drawing.Color.FromArgb(232, 234, 240);

        // ============================================================
        // UserControl
        // ============================================================
        this.BackColor = System.Drawing.Color.FromArgb(245, 247, 251);
        this.Controls.Add(this.dgvResults);
        this.Controls.Add(this.pnlFilter);
        this.Name = "UcAdvancedSearch";
        this.Size = new System.Drawing.Size(1100, 700);

        this.pnlFilter.ResumeLayout(false);
        this.pnlFilter.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.numYearFrom)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numYearTo)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
        this.ResumeLayout(false);
    }

    private void ConfigureLabel(Label lbl, string text, int x, int y)
    {
        lbl.AutoSize = true;
        lbl.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
        lbl.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        lbl.Location = new System.Drawing.Point(x, y);
        lbl.Text = text;
    }

    #endregion

    // ----------- Control fields -----------
    private Panel pnlFilter = null!;
    private Label lblHeader = null!;
    private Label lblSubtitle = null!;
    private Label lblEntity = null!;
    private ComboBox cboEntity = null!;

    private Label lblKeyword = null!;
    private TextBox txtKeyword = null!;
    private Label lblKeywordHint = null!;
    private Label lblStatus = null!;
    private ComboBox cboStatus = null!;

    private Label lblCategory = null!;
    private ComboBox cboCategory = null!;
    private Label lblAuthor = null!;
    private TextBox txtAuthor = null!;

    private Label lblYearRange = null!;
    private NumericUpDown numYearFrom = null!;
    private Label lblYearSeparator = null!;
    private NumericUpDown numYearTo = null!;

    private Label lblDateRange = null!;
    private DateTimePicker dtpDateFrom = null!;
    private Label lblDateSeparator = null!;
    private DateTimePicker dtpDateTo = null!;
    private CheckBox chkAllDates = null!;

    private Button btnSearch = null!;
    private Button btnReset = null!;
    private Button btnExport = null!;
    private Label lblResultCount = null!;

    private DataGridView dgvResults = null!;
}
