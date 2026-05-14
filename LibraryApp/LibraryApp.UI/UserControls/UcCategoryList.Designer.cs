namespace LibraryApp.UI.UserControls;

partial class UcCategoryList
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
        this.chkOnlyActive = new CheckBox();
        this.btnSearch = new Button();
        this.btnReload = new Button();

        // Grid
        this.dgvCategories = new DataGridView();

        // Form nhập liệu
        this.pnlForm = new Panel();
        this.lblFormHeader = new Label();
        this.lblCode = new Label();
        this.txtCode = new TextBox();
        this.lblName = new Label();
        this.txtName = new TextBox();
        this.lblDescription = new Label();
        this.txtDescription = new TextBox();
        this.chkIsActive = new CheckBox();

        // Buttons
        this.pnlButtons = new Panel();
        this.btnNew = new Button();
        this.btnSave = new Button();
        this.btnUpdate = new Button();
        this.btnDelete = new Button();
        this.btnCancel = new Button();

        this.errorProvider = new ErrorProvider(this.components);

        ((System.ComponentModel.ISupportInitialize)(this.dgvCategories)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
        this.pnlFilter.SuspendLayout();
        this.pnlForm.SuspendLayout();
        this.pnlButtons.SuspendLayout();
        this.SuspendLayout();

        // ============================================================
        // pnlFilter - thanh tìm kiếm
        // ============================================================
        this.pnlFilter.BackColor = System.Drawing.Color.White;
        this.pnlFilter.Controls.Add(this.btnReload);
        this.pnlFilter.Controls.Add(this.btnSearch);
        this.pnlFilter.Controls.Add(this.chkOnlyActive);
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
        this.txtSearchKeyword.PlaceholderText = "Mã hoặc tên danh mục";
        this.txtSearchKeyword.Size = new System.Drawing.Size(280, 25);
        this.txtSearchKeyword.KeyDown += new KeyEventHandler(this.txtSearchKeyword_KeyDown);

        this.chkOnlyActive.AutoSize = true;
        this.chkOnlyActive.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.chkOnlyActive.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.chkOnlyActive.Location = new System.Drawing.Point(372, 21);
        this.chkOnlyActive.Text = "Chỉ hiện danh mục đang hoạt động";
        this.chkOnlyActive.UseVisualStyleBackColor = true;
        this.chkOnlyActive.CheckedChanged += new EventHandler(this.chkOnlyActive_CheckedChanged);

        this.btnSearch.BackColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.btnSearch.FlatAppearance.BorderSize = 0;
        this.btnSearch.FlatStyle = FlatStyle.Flat;
        this.btnSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
        this.btnSearch.ForeColor = System.Drawing.Color.White;
        this.btnSearch.Location = new System.Drawing.Point(620, 16);
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
        this.btnReload.Location = new System.Drawing.Point(725, 16);
        this.btnReload.Size = new System.Drawing.Size(105, 30);
        this.btnReload.Text = "♻ Làm mới";
        this.btnReload.Cursor = Cursors.Hand;
        this.btnReload.UseVisualStyleBackColor = false;
        this.btnReload.Click += new EventHandler(this.btnReload_Click);

        // ============================================================
        // dgvCategories - lưới hiển thị
        // ============================================================
        this.dgvCategories.AllowUserToAddRows = false;
        this.dgvCategories.AllowUserToDeleteRows = false;
        this.dgvCategories.AllowUserToResizeRows = false;
        this.dgvCategories.AutoGenerateColumns = false;
        this.dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvCategories.BackgroundColor = System.Drawing.Color.White;
        this.dgvCategories.BorderStyle = BorderStyle.None;
        this.dgvCategories.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        this.dgvCategories.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        this.dgvCategories.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251),
            ForeColor = System.Drawing.Color.FromArgb(60, 60, 60),
            Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold),
            Alignment = DataGridViewContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 0, 0)
        };
        this.dgvCategories.ColumnHeadersHeight = 38;
        this.dgvCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        this.dgvCategories.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.White,
            ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            SelectionBackColor = System.Drawing.Color.FromArgb(218, 232, 252),
            SelectionForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Padding = new Padding(8, 0, 0, 0)
        };
        this.dgvCategories.RowHeadersVisible = false;
        this.dgvCategories.RowTemplate.Height = 32;
        this.dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvCategories.MultiSelect = false;
        this.dgvCategories.ReadOnly = true;
        this.dgvCategories.Dock = DockStyle.Fill;
        this.dgvCategories.EnableHeadersVisualStyles = false;
        this.dgvCategories.GridColor = System.Drawing.Color.FromArgb(232, 234, 240);

        // Cột (ẩn CategoryId để giữ ID; hiển thị 4 cột chính)
        var colId = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "CategoryId",
            HeaderText = "ID",
            Name = "CategoryId",
            Visible = false
        };
        var colCode = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "CategoryCode",
            HeaderText = "Mã danh mục",
            Name = "CategoryCode",
            FillWeight = 15
        };
        var colName = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "CategoryName",
            HeaderText = "Tên danh mục",
            Name = "CategoryName",
            FillWeight = 30
        };
        var colDesc = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Description",
            HeaderText = "Mô tả",
            Name = "Description",
            FillWeight = 45
        };
        var colActive = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "IsActive",
            HeaderText = "Hoạt động",
            Name = "IsActive",
            FillWeight = 10,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };

        this.dgvCategories.Columns.AddRange(colId, colCode, colName, colDesc, colActive);
        this.dgvCategories.SelectionChanged += new EventHandler(this.dgvCategories_SelectionChanged);
        this.dgvCategories.CellFormatting += new DataGridViewCellFormattingEventHandler(this.dgvCategories_CellFormatting);

        // ============================================================
        // pnlForm - panel nhập liệu
        // ============================================================
        this.pnlForm.BackColor = System.Drawing.Color.White;
        this.pnlForm.Dock = DockStyle.Right;
        this.pnlForm.Padding = new Padding(20);
        this.pnlForm.Width = 340;
        this.pnlForm.Controls.Add(this.pnlButtons);
        this.pnlForm.Controls.Add(this.chkIsActive);
        this.pnlForm.Controls.Add(this.txtDescription);
        this.pnlForm.Controls.Add(this.lblDescription);
        this.pnlForm.Controls.Add(this.txtName);
        this.pnlForm.Controls.Add(this.lblName);
        this.pnlForm.Controls.Add(this.txtCode);
        this.pnlForm.Controls.Add(this.lblCode);
        this.pnlForm.Controls.Add(this.lblFormHeader);

        this.lblFormHeader.AutoSize = true;
        this.lblFormHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold);
        this.lblFormHeader.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
        this.lblFormHeader.Location = new System.Drawing.Point(20, 15);
        this.lblFormHeader.Text = "Thông tin danh mục";

        ConfigureFieldLabel(this.lblCode, "Mã danh mục", 55);
        ConfigureFieldText(this.txtCode, 75);
        this.txtCode.MaxLength = 20;

        ConfigureFieldLabel(this.lblName, "Tên danh mục", 115);
        ConfigureFieldText(this.txtName, 135);
        this.txtName.MaxLength = 100;

        ConfigureFieldLabel(this.lblDescription, "Mô tả", 175);
        this.txtDescription.BorderStyle = BorderStyle.FixedSingle;
        this.txtDescription.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.txtDescription.Location = new System.Drawing.Point(20, 195);
        this.txtDescription.Multiline = true;
        this.txtDescription.ScrollBars = ScrollBars.Vertical;
        this.txtDescription.Size = new System.Drawing.Size(300, 120);
        this.txtDescription.MaxLength = 300;

        this.chkIsActive.AutoSize = true;
        this.chkIsActive.Checked = true;
        this.chkIsActive.Font = new System.Drawing.Font("Segoe UI", 10F);
        this.chkIsActive.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.chkIsActive.Location = new System.Drawing.Point(20, 330);
        this.chkIsActive.Text = "Đang hoạt động";
        this.chkIsActive.UseVisualStyleBackColor = true;

        // ============================================================
        // pnlButtons - các nút thao tác
        // ============================================================
        this.pnlButtons.Controls.Add(this.btnCancel);
        this.pnlButtons.Controls.Add(this.btnDelete);
        this.pnlButtons.Controls.Add(this.btnUpdate);
        this.pnlButtons.Controls.Add(this.btnSave);
        this.pnlButtons.Controls.Add(this.btnNew);
        this.pnlButtons.Dock = DockStyle.Bottom;
        this.pnlButtons.Height = 130;
        this.pnlButtons.Padding = new Padding(0, 12, 0, 0);

        ConfigureActionButton(this.btnNew, "➕  Thêm mới", System.Drawing.Color.FromArgb(33, 64, 154), this.btnNew_Click);
        this.btnNew.Top = 0; this.btnNew.Left = 0; this.btnNew.Width = 300;

        ConfigureActionButton(this.btnSave, "💾  Lưu", System.Drawing.Color.FromArgb(16, 185, 129), this.btnSave_Click);
        this.btnSave.Top = 40; this.btnSave.Left = 0; this.btnSave.Width = 145;

        ConfigureActionButton(this.btnUpdate, "✏  Cập nhật", System.Drawing.Color.FromArgb(245, 158, 11), this.btnUpdate_Click);
        this.btnUpdate.Top = 40; this.btnUpdate.Left = 155; this.btnUpdate.Width = 145;

        ConfigureActionButton(this.btnDelete, "🗑  Xoá", System.Drawing.Color.FromArgb(239, 68, 68), this.btnDelete_Click);
        this.btnDelete.Top = 80; this.btnDelete.Left = 0; this.btnDelete.Width = 145;

        ConfigureActionButton(this.btnCancel, "✖  Bỏ chọn", System.Drawing.Color.FromArgb(120, 120, 120), this.btnCancel_Click);
        this.btnCancel.Top = 80; this.btnCancel.Left = 155; this.btnCancel.Width = 145;

        // ============================================================
        // UserControl
        // ============================================================
        this.BackColor = System.Drawing.Color.FromArgb(245, 247, 251);
        this.Controls.Add(this.dgvCategories);
        this.Controls.Add(this.pnlForm);
        this.Controls.Add(this.pnlFilter);
        this.Name = "UcCategoryList";
        this.Size = new System.Drawing.Size(1100, 600);

        ((System.ComponentModel.ISupportInitialize)(this.dgvCategories)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
        this.pnlFilter.ResumeLayout(false);
        this.pnlFilter.PerformLayout();
        this.pnlForm.ResumeLayout(false);
        this.pnlForm.PerformLayout();
        this.pnlButtons.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    private void ConfigureFieldLabel(Label lbl, string text, int top)
    {
        lbl.AutoSize = true;
        lbl.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
        lbl.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        lbl.Location = new System.Drawing.Point(20, top);
        lbl.Text = text;
    }

    private void ConfigureFieldText(TextBox txt, int top)
    {
        txt.BorderStyle = BorderStyle.FixedSingle;
        txt.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        txt.Location = new System.Drawing.Point(20, top);
        txt.Size = new System.Drawing.Size(300, 25);
    }

    private void ConfigureActionButton(Button btn, string text, System.Drawing.Color color, EventHandler onClick)
    {
        btn.BackColor = color;
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatStyle = FlatStyle.Flat;
        btn.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
        btn.ForeColor = System.Drawing.Color.White;
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
    private CheckBox chkOnlyActive = null!;
    private Button btnSearch = null!;
    private Button btnReload = null!;

    private DataGridView dgvCategories = null!;

    private Panel pnlForm = null!;
    private Label lblFormHeader = null!;
    private Label lblCode = null!;
    private TextBox txtCode = null!;
    private Label lblName = null!;
    private TextBox txtName = null!;
    private Label lblDescription = null!;
    private TextBox txtDescription = null!;
    private CheckBox chkIsActive = null!;

    private Panel pnlButtons = null!;
    private Button btnNew = null!;
    private Button btnSave = null!;
    private Button btnUpdate = null!;
    private Button btnDelete = null!;
    private Button btnCancel = null!;

    private ErrorProvider errorProvider = null!;
}
