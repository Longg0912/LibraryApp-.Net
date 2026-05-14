namespace LibraryApp.UI.UserControls;

partial class UcBookList
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

        // Filter toolbar
        this.pnlFilter = new Panel();
        this.lblSearchKw = new Label();
        this.txtSearchKeyword = new TextBox();
        this.lblFilterCategory = new Label();
        this.cboFilterCategory = new ComboBox();
        this.lblFilterStatus = new Label();
        this.cboFilterStatus = new ComboBox();
        this.btnSearch = new Button();
        this.btnReload = new Button();

        // Grid
        this.dgvBooks = new DataGridView();

        // Form nhập liệu
        this.pnlForm = new Panel();
        this.lblFormHeader = new Label();
        this.lblCode = new Label();
        this.txtCode = new TextBox();
        this.lblTitle = new Label();
        this.txtTitle = new TextBox();
        this.lblAuthor = new Label();
        this.txtAuthor = new TextBox();
        this.lblPublisher = new Label();
        this.txtPublisher = new TextBox();
        this.lblYear = new Label();
        this.numYear = new NumericUpDown();
        this.lblCategory = new Label();
        this.cboCategory = new ComboBox();
        this.lblQuantity = new Label();
        this.numQuantity = new NumericUpDown();
        this.lblPrice = new Label();
        this.numPrice = new NumericUpDown();
        this.lblStatus = new Label();
        this.cboStatus = new ComboBox();

        // Buttons
        this.pnlButtons = new Panel();
        this.btnNew = new Button();
        this.btnSave = new Button();
        this.btnUpdate = new Button();
        this.btnDelete = new Button();
        this.btnCancel = new Button();

        this.errorProvider = new ErrorProvider(this.components);

        ((System.ComponentModel.ISupportInitialize)(this.dgvBooks)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numYear)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numPrice)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
        this.pnlFilter.SuspendLayout();
        this.pnlForm.SuspendLayout();
        this.pnlButtons.SuspendLayout();
        this.SuspendLayout();

        // ============================================================
        // pnlFilter - thanh tìm kiếm phía trên
        // ============================================================
        this.pnlFilter.BackColor = System.Drawing.Color.White;
        this.pnlFilter.Controls.Add(this.btnReload);
        this.pnlFilter.Controls.Add(this.btnSearch);
        this.pnlFilter.Controls.Add(this.cboFilterStatus);
        this.pnlFilter.Controls.Add(this.lblFilterStatus);
        this.pnlFilter.Controls.Add(this.cboFilterCategory);
        this.pnlFilter.Controls.Add(this.lblFilterCategory);
        this.pnlFilter.Controls.Add(this.txtSearchKeyword);
        this.pnlFilter.Controls.Add(this.lblSearchKw);
        this.pnlFilter.Dock = DockStyle.Top;
        this.pnlFilter.Height = 65;
        this.pnlFilter.Padding = new Padding(12, 14, 12, 14);

        this.lblSearchKw.AutoSize = true;
        this.lblSearchKw.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblSearchKw.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblSearchKw.Location = new System.Drawing.Point(12, 22);
        this.lblSearchKw.Text = "Từ khoá:";

        this.txtSearchKeyword.BorderStyle = BorderStyle.FixedSingle;
        this.txtSearchKeyword.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.txtSearchKeyword.Location = new System.Drawing.Point(74, 18);
        this.txtSearchKeyword.PlaceholderText = "Tên / Tác giả / Mã sách";
        this.txtSearchKeyword.Size = new System.Drawing.Size(220, 25);
        this.txtSearchKeyword.KeyDown += new KeyEventHandler(this.txtSearchKeyword_KeyDown);

        this.lblFilterCategory.AutoSize = true;
        this.lblFilterCategory.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblFilterCategory.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblFilterCategory.Location = new System.Drawing.Point(312, 22);
        this.lblFilterCategory.Text = "Danh mục:";

        this.cboFilterCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboFilterCategory.FlatStyle = FlatStyle.Flat;
        this.cboFilterCategory.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboFilterCategory.Location = new System.Drawing.Point(383, 18);
        this.cboFilterCategory.Size = new System.Drawing.Size(170, 25);

        this.lblFilterStatus.AutoSize = true;
        this.lblFilterStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblFilterStatus.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblFilterStatus.Location = new System.Drawing.Point(571, 22);
        this.lblFilterStatus.Text = "Trạng thái:";

        this.cboFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboFilterStatus.FlatStyle = FlatStyle.Flat;
        this.cboFilterStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboFilterStatus.Location = new System.Drawing.Point(645, 18);
        this.cboFilterStatus.Size = new System.Drawing.Size(140, 25);

        this.btnSearch.BackColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.btnSearch.FlatAppearance.BorderSize = 0;
        this.btnSearch.FlatStyle = FlatStyle.Flat;
        this.btnSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
        this.btnSearch.ForeColor = System.Drawing.Color.White;
        this.btnSearch.Location = new System.Drawing.Point(800, 16);
        this.btnSearch.Size = new System.Drawing.Size(95, 30);
        this.btnSearch.Text = "🔍 Tìm kiếm";
        this.btnSearch.Cursor = Cursors.Hand;
        this.btnSearch.UseVisualStyleBackColor = false;
        this.btnSearch.Click += new EventHandler(this.btnSearch_Click);

        this.btnReload.BackColor = System.Drawing.Color.White;
        this.btnReload.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
        this.btnReload.FlatStyle = FlatStyle.Flat;
        this.btnReload.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
        this.btnReload.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.btnReload.Location = new System.Drawing.Point(905, 16);
        this.btnReload.Size = new System.Drawing.Size(105, 30);
        this.btnReload.Text = "♻ Làm mới";
        this.btnReload.Cursor = Cursors.Hand;
        this.btnReload.UseVisualStyleBackColor = false;
        this.btnReload.Click += new EventHandler(this.btnReload_Click);

        // ============================================================
        // dgvBooks - lưới hiển thị danh sách
        // ============================================================
        this.dgvBooks.AllowUserToAddRows = false;
        this.dgvBooks.AllowUserToDeleteRows = false;
        this.dgvBooks.AllowUserToResizeRows = false;
        this.dgvBooks.AutoGenerateColumns = false;
        this.dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvBooks.BackgroundColor = System.Drawing.Color.White;
        this.dgvBooks.BorderStyle = BorderStyle.None;
        this.dgvBooks.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        this.dgvBooks.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        this.dgvBooks.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251),
            ForeColor = System.Drawing.Color.FromArgb(60, 60, 60),
            Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold),
            Alignment = DataGridViewContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 0, 0)
        };
        this.dgvBooks.ColumnHeadersHeight = 38;
        this.dgvBooks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        this.dgvBooks.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.White,
            ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            SelectionBackColor = System.Drawing.Color.FromArgb(218, 232, 252),
            SelectionForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Padding = new Padding(8, 0, 0, 0)
        };
        this.dgvBooks.RowHeadersVisible = false;
        this.dgvBooks.RowTemplate.Height = 32;
        this.dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvBooks.MultiSelect = false;
        this.dgvBooks.ReadOnly = true;
        this.dgvBooks.Dock = DockStyle.Fill;
        this.dgvBooks.EnableHeadersVisualStyles = false;
        this.dgvBooks.GridColor = System.Drawing.Color.FromArgb(232, 234, 240);

        // Định nghĩa cột (mapping với DataPropertyName từ DataTable do BLL trả về)
        var colCode = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "BookCode",
            HeaderText = "Mã sách",
            Name = "BookCode",
            FillWeight = 12
        };
        var colTitle = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Title",
            HeaderText = "Tên sách",
            Name = "Title",
            FillWeight = 30
        };
        var colAuthor = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Author",
            HeaderText = "Tác giả",
            Name = "Author",
            FillWeight = 20
        };
        var colCategory = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "CategoryName",
            HeaderText = "Danh mục",
            Name = "CategoryName",
            FillWeight = 14
        };
        var colYear = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "PublishYear",
            HeaderText = "Năm XB",
            Name = "PublishYear",
            FillWeight = 8,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        var colQty = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Quantity",
            HeaderText = "SL",
            Name = "Quantity",
            FillWeight = 7,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        var colAvail = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "AvailableQty",
            HeaderText = "Còn",
            Name = "AvailableQty",
            FillWeight = 7,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        var colStatus = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Status",
            HeaderText = "Trạng thái",
            Name = "Status",
            FillWeight = 12
        };
        var colId = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "BookId",
            HeaderText = "BookId",
            Name = "BookId",
            Visible = false   // ẩn nhưng giữ để lấy ID khi click
        };

        this.dgvBooks.Columns.AddRange(colId, colCode, colTitle, colAuthor,
            colCategory, colYear, colQty, colAvail, colStatus);
        this.dgvBooks.SelectionChanged += new EventHandler(this.dgvBooks_SelectionChanged);
        this.dgvBooks.CellFormatting += new DataGridViewCellFormattingEventHandler(this.dgvBooks_CellFormatting);

        // ============================================================
        // pnlForm - panel nhập liệu (right side)
        // ============================================================
        this.pnlForm.BackColor = System.Drawing.Color.White;
        this.pnlForm.Dock = DockStyle.Right;
        this.pnlForm.Padding = new Padding(20);
        this.pnlForm.Width = 340;
        this.pnlForm.Controls.Add(this.pnlButtons);
        this.pnlForm.Controls.Add(this.cboStatus);
        this.pnlForm.Controls.Add(this.lblStatus);
        this.pnlForm.Controls.Add(this.numPrice);
        this.pnlForm.Controls.Add(this.lblPrice);
        this.pnlForm.Controls.Add(this.numQuantity);
        this.pnlForm.Controls.Add(this.lblQuantity);
        this.pnlForm.Controls.Add(this.cboCategory);
        this.pnlForm.Controls.Add(this.lblCategory);
        this.pnlForm.Controls.Add(this.numYear);
        this.pnlForm.Controls.Add(this.lblYear);
        this.pnlForm.Controls.Add(this.txtPublisher);
        this.pnlForm.Controls.Add(this.lblPublisher);
        this.pnlForm.Controls.Add(this.txtAuthor);
        this.pnlForm.Controls.Add(this.lblAuthor);
        this.pnlForm.Controls.Add(this.txtTitle);
        this.pnlForm.Controls.Add(this.lblTitle);
        this.pnlForm.Controls.Add(this.txtCode);
        this.pnlForm.Controls.Add(this.lblCode);
        this.pnlForm.Controls.Add(this.lblFormHeader);

        this.lblFormHeader.AutoSize = true;
        this.lblFormHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold);
        this.lblFormHeader.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
        this.lblFormHeader.Location = new System.Drawing.Point(20, 15);
        this.lblFormHeader.Text = "Thông tin sách";

        // Đặt từng cặp Label + control theo hàng dọc, cùng khoảng cách
        ConfigureFieldLabel(this.lblCode, "Mã sách", 50);
        ConfigureFieldText(this.txtCode, 70);

        ConfigureFieldLabel(this.lblTitle, "Tên sách", 105);
        ConfigureFieldText(this.txtTitle, 125);

        ConfigureFieldLabel(this.lblAuthor, "Tác giả", 160);
        ConfigureFieldText(this.txtAuthor, 180);

        ConfigureFieldLabel(this.lblPublisher, "Nhà xuất bản", 215);
        ConfigureFieldText(this.txtPublisher, 235);

        ConfigureFieldLabel(this.lblYear, "Năm XB", 270);
        this.numYear.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.numYear.Location = new System.Drawing.Point(20, 290);
        this.numYear.Size = new System.Drawing.Size(140, 25);
        this.numYear.Minimum = 1500;
        this.numYear.Maximum = 2100;
        this.numYear.Value = DateTime.Now.Year;

        ConfigureFieldLabel(this.lblCategory, "Danh mục", 270);
        this.lblCategory.Location = new System.Drawing.Point(170, 270);
        this.cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboCategory.FlatStyle = FlatStyle.Flat;
        this.cboCategory.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboCategory.Location = new System.Drawing.Point(170, 290);
        this.cboCategory.Size = new System.Drawing.Size(150, 25);

        ConfigureFieldLabel(this.lblQuantity, "Số lượng", 325);
        this.numQuantity.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.numQuantity.Location = new System.Drawing.Point(20, 345);
        this.numQuantity.Size = new System.Drawing.Size(140, 25);
        this.numQuantity.Minimum = 0;
        this.numQuantity.Maximum = 9999;

        ConfigureFieldLabel(this.lblPrice, "Giá (VNĐ)", 325);
        this.lblPrice.Location = new System.Drawing.Point(170, 325);
        this.numPrice.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.numPrice.Location = new System.Drawing.Point(170, 345);
        this.numPrice.Size = new System.Drawing.Size(150, 25);
        this.numPrice.Minimum = 0;
        this.numPrice.Maximum = 999_999_999;
        this.numPrice.Increment = 1000;
        this.numPrice.ThousandsSeparator = true;

        ConfigureFieldLabel(this.lblStatus, "Trạng thái", 380);
        this.cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboStatus.FlatStyle = FlatStyle.Flat;
        this.cboStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboStatus.Location = new System.Drawing.Point(20, 400);
        this.cboStatus.Size = new System.Drawing.Size(300, 25);

        // ============================================================
        // pnlButtons - các nút thao tác phía dưới panel nhập liệu
        // ============================================================
        this.pnlButtons.Controls.Add(this.btnCancel);
        this.pnlButtons.Controls.Add(this.btnDelete);
        this.pnlButtons.Controls.Add(this.btnUpdate);
        this.pnlButtons.Controls.Add(this.btnSave);
        this.pnlButtons.Controls.Add(this.btnNew);
        this.pnlButtons.Dock = DockStyle.Bottom;
        this.pnlButtons.Height = 130;
        this.pnlButtons.Padding = new Padding(0, 12, 0, 0);

        ConfigureActionButton(this.btnNew, "➕  Thêm mới", 0, System.Drawing.Color.FromArgb(33, 64, 154), this.btnNew_Click);
        ConfigureActionButton(this.btnSave, "💾  Lưu", 40, System.Drawing.Color.FromArgb(16, 185, 129), this.btnSave_Click);
        ConfigureActionButton(this.btnUpdate, "✏  Cập nhật", 80, System.Drawing.Color.FromArgb(245, 158, 11), this.btnUpdate_Click);
        this.btnSave.Width = 145;
        this.btnUpdate.Left = 155; this.btnUpdate.Top = 40; this.btnUpdate.Width = 145;
        this.btnSave.Top = 40;

        ConfigureActionButton(this.btnDelete, "🗑  Xoá", 80, System.Drawing.Color.FromArgb(239, 68, 68), this.btnDelete_Click);
        this.btnDelete.Width = 145;

        ConfigureActionButton(this.btnCancel, "✖  Bỏ chọn", 80, System.Drawing.Color.FromArgb(120, 120, 120), this.btnCancel_Click);
        this.btnCancel.Left = 155; this.btnCancel.Top = 80; this.btnCancel.Width = 145;

        this.btnNew.Top = 0; this.btnNew.Width = 300;

        // ============================================================
        // UserControl
        // ============================================================
        this.BackColor = System.Drawing.Color.FromArgb(245, 247, 251);
        this.Controls.Add(this.dgvBooks);
        this.Controls.Add(this.pnlForm);
        this.Controls.Add(this.pnlFilter);
        this.Name = "UcBookList";
        this.Size = new System.Drawing.Size(1100, 600);

        ((System.ComponentModel.ISupportInitialize)(this.dgvBooks)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numYear)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numPrice)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
        this.pnlFilter.ResumeLayout(false);
        this.pnlFilter.PerformLayout();
        this.pnlForm.ResumeLayout(false);
        this.pnlForm.PerformLayout();
        this.pnlButtons.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    /// <summary>Cấu hình một label cho field nhập liệu.</summary>
    private void ConfigureFieldLabel(Label lbl, string text, int top)
    {
        lbl.AutoSize = true;
        lbl.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
        lbl.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        lbl.Location = new System.Drawing.Point(20, top);
        lbl.Text = text;
    }

    /// <summary>Cấu hình một textbox cho field nhập liệu.</summary>
    private void ConfigureFieldText(TextBox txt, int top)
    {
        txt.BorderStyle = BorderStyle.FixedSingle;
        txt.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        txt.Location = new System.Drawing.Point(20, top);
        txt.Size = new System.Drawing.Size(300, 25);
    }

    /// <summary>Cấu hình một nút action với màu nền.</summary>
    private void ConfigureActionButton(Button btn, string text, int top, System.Drawing.Color color, EventHandler onClick)
    {
        btn.BackColor = color;
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatStyle = FlatStyle.Flat;
        btn.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
        btn.ForeColor = System.Drawing.Color.White;
        btn.Location = new System.Drawing.Point(0, top);
        btn.Size = new System.Drawing.Size(300, 34);
        btn.Text = text;
        btn.UseVisualStyleBackColor = false;
        btn.Cursor = Cursors.Hand;
        btn.Click += onClick;
    }

    #endregion

    // ----------- Control fields -----------
    private Panel pnlFilter = null!;
    private Label lblSearchKw = null!;
    private TextBox txtSearchKeyword = null!;
    private Label lblFilterCategory = null!;
    private ComboBox cboFilterCategory = null!;
    private Label lblFilterStatus = null!;
    private ComboBox cboFilterStatus = null!;
    private Button btnSearch = null!;
    private Button btnReload = null!;

    private DataGridView dgvBooks = null!;

    private Panel pnlForm = null!;
    private Label lblFormHeader = null!;
    private Label lblCode = null!;
    private TextBox txtCode = null!;
    private Label lblTitle = null!;
    private TextBox txtTitle = null!;
    private Label lblAuthor = null!;
    private TextBox txtAuthor = null!;
    private Label lblPublisher = null!;
    private TextBox txtPublisher = null!;
    private Label lblYear = null!;
    private NumericUpDown numYear = null!;
    private Label lblCategory = null!;
    private ComboBox cboCategory = null!;
    private Label lblQuantity = null!;
    private NumericUpDown numQuantity = null!;
    private Label lblPrice = null!;
    private NumericUpDown numPrice = null!;
    private Label lblStatus = null!;
    private ComboBox cboStatus = null!;

    private Panel pnlButtons = null!;
    private Button btnNew = null!;
    private Button btnSave = null!;
    private Button btnUpdate = null!;
    private Button btnDelete = null!;
    private Button btnCancel = null!;

    private ErrorProvider errorProvider = null!;
}
