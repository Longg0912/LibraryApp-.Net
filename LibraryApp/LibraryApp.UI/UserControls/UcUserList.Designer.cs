namespace LibraryApp.UI.UserControls;

partial class UcUserList
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

        // Filter
        this.pnlFilter = new Panel();
        this.lblSearchKw = new Label();
        this.txtSearchKeyword = new TextBox();
        this.lblFilterRole = new Label();
        this.cboFilterRole = new ComboBox();
        this.lblFilterStatus = new Label();
        this.cboFilterStatus = new ComboBox();
        this.btnSearch = new Button();
        this.btnReload = new Button();

        // Grid
        this.dgvUsers = new DataGridView();

        // Form
        this.pnlForm = new Panel();
        this.lblFormHeader = new Label();
        this.lblUsername = new Label();
        this.txtUsername = new TextBox();
        this.lblFullName = new Label();
        this.txtFullName = new TextBox();
        this.lblEmail = new Label();
        this.txtEmail = new TextBox();
        this.lblPhone = new Label();
        this.txtPhone = new TextBox();
        this.lblRole = new Label();
        this.cboRole = new ComboBox();
        this.lblTempPwd = new Label();
        this.txtTempPwd = new TextBox();
        this.chkIsActive = new CheckBox();
        this.lblMeta = new Label();

        // Buttons
        this.pnlButtons = new Panel();
        this.btnNew = new Button();
        this.btnSave = new Button();
        this.btnUpdate = new Button();
        this.btnResetPwd = new Button();
        this.btnToggleActive = new Button();
        this.btnDelete = new Button();
        this.btnCancel = new Button();

        this.errorProvider = new ErrorProvider(this.components);

        ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
        this.pnlFilter.SuspendLayout();
        this.pnlForm.SuspendLayout();
        this.pnlButtons.SuspendLayout();
        this.SuspendLayout();

        // ============================================================
        // pnlFilter
        // ============================================================
        this.pnlFilter.BackColor = System.Drawing.Color.White;
        this.pnlFilter.Controls.AddRange(new Control[] {
            this.btnReload, this.btnSearch,
            this.cboFilterStatus, this.lblFilterStatus,
            this.cboFilterRole, this.lblFilterRole,
            this.txtSearchKeyword, this.lblSearchKw
        });
        this.pnlFilter.Dock = DockStyle.Top;
        this.pnlFilter.Height = 65;
        this.pnlFilter.Padding = new Padding(12, 14, 12, 14);

        this.lblSearchKw.AutoSize = true;
        this.lblSearchKw.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblSearchKw.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblSearchKw.Location = new System.Drawing.Point(12, 22);
        this.lblSearchKw.Text = "Tìm:";

        this.txtSearchKeyword.BorderStyle = BorderStyle.FixedSingle;
        this.txtSearchKeyword.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.txtSearchKeyword.Location = new System.Drawing.Point(48, 18);
        this.txtSearchKeyword.PlaceholderText = "Username / Họ tên / Email";
        this.txtSearchKeyword.Size = new System.Drawing.Size(230, 25);
        this.txtSearchKeyword.KeyDown += new KeyEventHandler(this.txtSearchKeyword_KeyDown);

        this.lblFilterRole.AutoSize = true;
        this.lblFilterRole.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblFilterRole.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblFilterRole.Location = new System.Drawing.Point(296, 22);
        this.lblFilterRole.Text = "Vai trò:";

        this.cboFilterRole.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboFilterRole.FlatStyle = FlatStyle.Flat;
        this.cboFilterRole.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboFilterRole.Location = new System.Drawing.Point(348, 18);
        this.cboFilterRole.Size = new System.Drawing.Size(150, 25);

        this.lblFilterStatus.AutoSize = true;
        this.lblFilterStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblFilterStatus.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblFilterStatus.Location = new System.Drawing.Point(516, 22);
        this.lblFilterStatus.Text = "Trạng thái:";

        this.cboFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboFilterStatus.FlatStyle = FlatStyle.Flat;
        this.cboFilterStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboFilterStatus.Location = new System.Drawing.Point(588, 18);
        this.cboFilterStatus.Size = new System.Drawing.Size(130, 25);

        this.btnSearch.BackColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.btnSearch.FlatAppearance.BorderSize = 0;
        this.btnSearch.FlatStyle = FlatStyle.Flat;
        this.btnSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
        this.btnSearch.ForeColor = System.Drawing.Color.White;
        this.btnSearch.Location = new System.Drawing.Point(736, 16);
        this.btnSearch.Size = new System.Drawing.Size(95, 30);
        this.btnSearch.Text = "🔍 Tìm";
        this.btnSearch.Cursor = Cursors.Hand;
        this.btnSearch.UseVisualStyleBackColor = false;
        this.btnSearch.Click += new EventHandler(this.btnSearch_Click);

        this.btnReload.BackColor = System.Drawing.Color.White;
        this.btnReload.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
        this.btnReload.FlatStyle = FlatStyle.Flat;
        this.btnReload.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
        this.btnReload.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.btnReload.Location = new System.Drawing.Point(841, 16);
        this.btnReload.Size = new System.Drawing.Size(100, 30);
        this.btnReload.Text = "♻ Làm mới";
        this.btnReload.Cursor = Cursors.Hand;
        this.btnReload.UseVisualStyleBackColor = false;
        this.btnReload.Click += new EventHandler(this.btnReload_Click);

        // ============================================================
        // dgvUsers
        // ============================================================
        this.dgvUsers.AllowUserToAddRows = false;
        this.dgvUsers.AllowUserToDeleteRows = false;
        this.dgvUsers.AllowUserToResizeRows = false;
        this.dgvUsers.AutoGenerateColumns = false;
        this.dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvUsers.BackgroundColor = System.Drawing.Color.White;
        this.dgvUsers.BorderStyle = BorderStyle.None;
        this.dgvUsers.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        this.dgvUsers.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251),
            ForeColor = System.Drawing.Color.FromArgb(60, 60, 60),
            Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold),
            Padding = new Padding(8, 0, 0, 0)
        };
        this.dgvUsers.ColumnHeadersHeight = 38;
        this.dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        this.dgvUsers.DefaultCellStyle = new DataGridViewCellStyle
        {
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            SelectionBackColor = System.Drawing.Color.FromArgb(218, 232, 252),
            SelectionForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Padding = new Padding(8, 0, 0, 0)
        };
        this.dgvUsers.RowHeadersVisible = false;
        this.dgvUsers.RowTemplate.Height = 32;
        this.dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvUsers.MultiSelect = false;
        this.dgvUsers.ReadOnly = true;
        this.dgvUsers.Dock = DockStyle.Fill;
        this.dgvUsers.EnableHeadersVisualStyles = false;
        this.dgvUsers.GridColor = System.Drawing.Color.FromArgb(232, 234, 240);

        var colId = new DataGridViewTextBoxColumn
        { DataPropertyName = "UserId", HeaderText = "ID", Name = "UserId", Visible = false };
        var colUsername = new DataGridViewTextBoxColumn
        { DataPropertyName = "Username", HeaderText = "Tên đăng nhập", Name = "Username", FillWeight = 14 };
        var colFullName = new DataGridViewTextBoxColumn
        { DataPropertyName = "FullName", HeaderText = "Họ tên", Name = "FullName", FillWeight = 22 };
        var colEmail = new DataGridViewTextBoxColumn
        { DataPropertyName = "Email", HeaderText = "Email", Name = "Email", FillWeight = 20 };
        var colPhone = new DataGridViewTextBoxColumn
        { DataPropertyName = "Phone", HeaderText = "SĐT", Name = "Phone", FillWeight = 11 };
        var colRoleName = new DataGridViewTextBoxColumn
        { DataPropertyName = "RoleName", HeaderText = "Vai trò", Name = "RoleName", FillWeight = 13 };
        var colActive = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "IsActive",
            HeaderText = "Hoạt động",
            Name = "IsActive",
            FillWeight = 10,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        var colLastLogin = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "LastLoginAt",
            HeaderText = "Đăng nhập cuối",
            Name = "LastLoginAt",
            FillWeight = 12,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
        };
        // Cột ẩn để JS truyền RoleId vào form
        var colRoleId = new DataGridViewTextBoxColumn
        { DataPropertyName = "RoleId", HeaderText = "RoleId", Name = "RoleId", Visible = false };

        this.dgvUsers.Columns.AddRange(colId, colUsername, colFullName,
            colEmail, colPhone, colRoleName, colActive, colLastLogin, colRoleId);
        this.dgvUsers.SelectionChanged += new EventHandler(this.dgvUsers_SelectionChanged);
        this.dgvUsers.CellFormatting += new DataGridViewCellFormattingEventHandler(this.dgvUsers_CellFormatting);

        // ============================================================
        // pnlForm
        // ============================================================
        this.pnlForm.BackColor = System.Drawing.Color.White;
        this.pnlForm.Dock = DockStyle.Right;
        this.pnlForm.Padding = new Padding(20);
        this.pnlForm.Width = 360;
        this.pnlForm.AutoScroll = true;
        this.pnlForm.Controls.AddRange(new Control[] {
            this.pnlButtons,
            this.lblMeta,
            this.chkIsActive,
            this.txtTempPwd, this.lblTempPwd,
            this.cboRole, this.lblRole,
            this.txtPhone, this.lblPhone,
            this.txtEmail, this.lblEmail,
            this.txtFullName, this.lblFullName,
            this.txtUsername, this.lblUsername,
            this.lblFormHeader
        });

        this.lblFormHeader.AutoSize = true;
        this.lblFormHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold);
        this.lblFormHeader.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
        this.lblFormHeader.Location = new System.Drawing.Point(20, 15);
        this.lblFormHeader.Text = "Thông tin người dùng";

        ConfigureFieldLabel(this.lblUsername, "Tên đăng nhập", 55);
        ConfigureFieldText(this.txtUsername, 75);
        this.txtUsername.MaxLength = 50;

        ConfigureFieldLabel(this.lblFullName, "Họ tên", 110);
        ConfigureFieldText(this.txtFullName, 130);
        this.txtFullName.MaxLength = 100;

        ConfigureFieldLabel(this.lblEmail, "Email", 165);
        ConfigureFieldText(this.txtEmail, 185);
        this.txtEmail.MaxLength = 100;

        ConfigureFieldLabel(this.lblPhone, "Số điện thoại", 220);
        ConfigureFieldText(this.txtPhone, 240);
        this.txtPhone.MaxLength = 15;

        ConfigureFieldLabel(this.lblRole, "Vai trò", 275);
        this.cboRole.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboRole.FlatStyle = FlatStyle.Flat;
        this.cboRole.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboRole.Location = new System.Drawing.Point(20, 295);
        this.cboRole.Size = new System.Drawing.Size(300, 25);

        ConfigureFieldLabel(this.lblTempPwd, "Mật khẩu tạm (chỉ khi thêm mới)", 330);
        ConfigureFieldText(this.txtTempPwd, 350);
        this.txtTempPwd.MaxLength = 100;
        this.txtTempPwd.PasswordChar = '●';

        this.chkIsActive.AutoSize = true;
        this.chkIsActive.Checked = true;
        this.chkIsActive.Enabled = false;   // chỉ chỉnh qua nút "Toggle Active"
        this.chkIsActive.Font = new System.Drawing.Font("Segoe UI", 10F);
        this.chkIsActive.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.chkIsActive.Location = new System.Drawing.Point(20, 388);
        this.chkIsActive.Text = "Đang hoạt động";
        this.chkIsActive.UseVisualStyleBackColor = true;

        this.lblMeta.AutoSize = false;
        this.lblMeta.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic);
        this.lblMeta.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
        this.lblMeta.Location = new System.Drawing.Point(20, 420);
        this.lblMeta.Size = new System.Drawing.Size(300, 50);
        this.lblMeta.Text = "";

        // ============================================================
        // pnlButtons
        // ============================================================
        this.pnlButtons.Controls.AddRange(new Control[] {
            this.btnCancel, this.btnDelete, this.btnToggleActive,
            this.btnResetPwd, this.btnUpdate, this.btnSave, this.btnNew
        });
        this.pnlButtons.Location = new System.Drawing.Point(20, 485);
        this.pnlButtons.Size = new System.Drawing.Size(300, 215);

        ConfigureActionButton(this.btnNew, "➕  Thêm mới", System.Drawing.Color.FromArgb(33, 64, 154), this.btnNew_Click);
        this.btnNew.Top = 0; this.btnNew.Left = 0; this.btnNew.Width = 300;

        ConfigureActionButton(this.btnSave, "💾  Lưu", System.Drawing.Color.FromArgb(16, 185, 129), this.btnSave_Click);
        this.btnSave.Top = 40; this.btnSave.Left = 0; this.btnSave.Width = 145;

        ConfigureActionButton(this.btnUpdate, "✏  Cập nhật", System.Drawing.Color.FromArgb(245, 158, 11), this.btnUpdate_Click);
        this.btnUpdate.Top = 40; this.btnUpdate.Left = 155; this.btnUpdate.Width = 145;

        ConfigureActionButton(this.btnResetPwd, "🔑 Reset mật khẩu", System.Drawing.Color.FromArgb(99, 102, 241), this.btnResetPwd_Click);
        this.btnResetPwd.Top = 80; this.btnResetPwd.Left = 0; this.btnResetPwd.Width = 300;

        ConfigureActionButton(this.btnToggleActive, "🔒  Khoá / Mở khoá", System.Drawing.Color.FromArgb(6, 182, 212), this.btnToggleActive_Click);
        this.btnToggleActive.Top = 120; this.btnToggleActive.Left = 0; this.btnToggleActive.Width = 300;

        ConfigureActionButton(this.btnDelete, "🗑  Xoá", System.Drawing.Color.FromArgb(239, 68, 68), this.btnDelete_Click);
        this.btnDelete.Top = 160; this.btnDelete.Left = 0; this.btnDelete.Width = 145;

        ConfigureActionButton(this.btnCancel, "✖  Bỏ chọn", System.Drawing.Color.FromArgb(120, 120, 120), this.btnCancel_Click);
        this.btnCancel.Top = 160; this.btnCancel.Left = 155; this.btnCancel.Width = 145;

        // ============================================================
        // UserControl
        // ============================================================
        this.BackColor = System.Drawing.Color.FromArgb(245, 247, 251);
        this.Controls.Add(this.dgvUsers);
        this.Controls.Add(this.pnlForm);
        this.Controls.Add(this.pnlFilter);
        this.Name = "UcUserList";
        this.Size = new System.Drawing.Size(1200, 720);

        ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
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

    // ----------- Fields -----------
    private Panel pnlFilter = null!;
    private Label lblSearchKw = null!;
    private TextBox txtSearchKeyword = null!;
    private Label lblFilterRole = null!;
    private ComboBox cboFilterRole = null!;
    private Label lblFilterStatus = null!;
    private ComboBox cboFilterStatus = null!;
    private Button btnSearch = null!;
    private Button btnReload = null!;

    private DataGridView dgvUsers = null!;

    private Panel pnlForm = null!;
    private Label lblFormHeader = null!;
    private Label lblUsername = null!;
    private TextBox txtUsername = null!;
    private Label lblFullName = null!;
    private TextBox txtFullName = null!;
    private Label lblEmail = null!;
    private TextBox txtEmail = null!;
    private Label lblPhone = null!;
    private TextBox txtPhone = null!;
    private Label lblRole = null!;
    private ComboBox cboRole = null!;
    private Label lblTempPwd = null!;
    private TextBox txtTempPwd = null!;
    private CheckBox chkIsActive = null!;
    private Label lblMeta = null!;

    private Panel pnlButtons = null!;
    private Button btnNew = null!;
    private Button btnSave = null!;
    private Button btnUpdate = null!;
    private Button btnResetPwd = null!;
    private Button btnToggleActive = null!;
    private Button btnDelete = null!;
    private Button btnCancel = null!;

    private ErrorProvider errorProvider = null!;
}
