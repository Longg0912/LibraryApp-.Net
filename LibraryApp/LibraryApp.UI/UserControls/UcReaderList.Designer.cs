namespace LibraryApp.UI.UserControls;

partial class UcReaderList
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
        this.lblFilterStatus = new Label();
        this.cboFilterStatus = new ComboBox();
        this.btnSearch = new Button();
        this.btnReload = new Button();

        // Grid
        this.dgvReaders = new DataGridView();

        // Form nhập liệu
        this.pnlForm = new Panel();
        this.lblFormHeader = new Label();
        this.lblCard = new Label();
        this.txtCard = new TextBox();
        this.lblFullName = new Label();
        this.txtFullName = new TextBox();
        this.lblDob = new Label();
        this.dtpDob = new DateTimePicker();
        this.lblGender = new Label();
        this.cboGender = new ComboBox();
        this.lblAddress = new Label();
        this.txtAddress = new TextBox();
        this.lblPhone = new Label();
        this.txtPhone = new TextBox();
        this.lblEmail = new Label();
        this.txtEmail = new TextBox();
        this.lblIssueDate = new Label();
        this.dtpIssueDate = new DateTimePicker();
        this.lblExpireDate = new Label();
        this.dtpExpireDate = new DateTimePicker();
        this.lblStatus = new Label();
        this.cboStatus = new ComboBox();

        // Buttons
        this.pnlButtons = new Panel();
        this.btnNew = new Button();
        this.btnSave = new Button();
        this.btnUpdate = new Button();
        this.btnRenew = new Button();
        this.btnDelete = new Button();
        this.btnCancel = new Button();

        this.errorProvider = new ErrorProvider(this.components);

        ((System.ComponentModel.ISupportInitialize)(this.dgvReaders)).BeginInit();
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
        this.pnlFilter.Controls.Add(this.cboFilterStatus);
        this.pnlFilter.Controls.Add(this.lblFilterStatus);
        this.pnlFilter.Controls.Add(this.txtSearchKeyword);
        this.pnlFilter.Controls.Add(this.lblSearchKw);
        this.pnlFilter.Dock = DockStyle.Top;
        this.pnlFilter.Height = 65;
        this.pnlFilter.Padding = new Padding(12, 14, 12, 14);

        this.lblSearchKw.AutoSize = true;
        this.lblSearchKw.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblSearchKw.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblSearchKw.Location = new System.Drawing.Point(12, 22);
        this.lblSearchKw.Text = "Tìm nhanh:";

        this.txtSearchKeyword.BorderStyle = BorderStyle.FixedSingle;
        this.txtSearchKeyword.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.txtSearchKeyword.Location = new System.Drawing.Point(90, 18);
        this.txtSearchKeyword.PlaceholderText = "Họ tên / Số thẻ / Số điện thoại";
        this.txtSearchKeyword.Size = new System.Drawing.Size(280, 25);
        this.txtSearchKeyword.KeyDown += new KeyEventHandler(this.txtSearchKeyword_KeyDown);

        this.lblFilterStatus.AutoSize = true;
        this.lblFilterStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblFilterStatus.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblFilterStatus.Location = new System.Drawing.Point(388, 22);
        this.lblFilterStatus.Text = "Trạng thái:";

        this.cboFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboFilterStatus.FlatStyle = FlatStyle.Flat;
        this.cboFilterStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboFilterStatus.Location = new System.Drawing.Point(462, 18);
        this.cboFilterStatus.Size = new System.Drawing.Size(140, 25);

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
        // dgvReaders
        // ============================================================
        this.dgvReaders.AllowUserToAddRows = false;
        this.dgvReaders.AllowUserToDeleteRows = false;
        this.dgvReaders.AllowUserToResizeRows = false;
        this.dgvReaders.AutoGenerateColumns = false;
        this.dgvReaders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvReaders.BackgroundColor = System.Drawing.Color.White;
        this.dgvReaders.BorderStyle = BorderStyle.None;
        this.dgvReaders.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        this.dgvReaders.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        this.dgvReaders.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251),
            ForeColor = System.Drawing.Color.FromArgb(60, 60, 60),
            Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold),
            Alignment = DataGridViewContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 0, 0)
        };
        this.dgvReaders.ColumnHeadersHeight = 38;
        this.dgvReaders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        this.dgvReaders.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.White,
            ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            SelectionBackColor = System.Drawing.Color.FromArgb(218, 232, 252),
            SelectionForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Padding = new Padding(8, 0, 0, 0)
        };
        this.dgvReaders.RowHeadersVisible = false;
        this.dgvReaders.RowTemplate.Height = 32;
        this.dgvReaders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvReaders.MultiSelect = false;
        this.dgvReaders.ReadOnly = true;
        this.dgvReaders.Dock = DockStyle.Fill;
        this.dgvReaders.EnableHeadersVisualStyles = false;
        this.dgvReaders.GridColor = System.Drawing.Color.FromArgb(232, 234, 240);

        // Cột (ẩn ReaderId)
        var colId = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "ReaderId",
            HeaderText = "ID",
            Name = "ReaderId",
            Visible = false
        };
        var colCard = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "CardNumber",
            HeaderText = "Số thẻ",
            Name = "CardNumber",
            FillWeight = 10
        };
        var colName = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "FullName",
            HeaderText = "Họ tên",
            Name = "FullName",
            FillWeight = 25
        };
        var colPhone = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Phone",
            HeaderText = "Số điện thoại",
            Name = "Phone",
            FillWeight = 12
        };
        var colEmail = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Email",
            HeaderText = "Email",
            Name = "Email",
            FillWeight = 20
        };
        var colExpire = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "CardExpireDate",
            HeaderText = "Hết hạn",
            Name = "CardExpireDate",
            FillWeight = 12,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "dd/MM/yyyy",
                Alignment = DataGridViewContentAlignment.MiddleCenter
            }
        };
        var colStatus = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Status",
            HeaderText = "Trạng thái",
            Name = "Status",
            FillWeight = 11
        };

        this.dgvReaders.Columns.AddRange(colId, colCard, colName, colPhone, colEmail, colExpire, colStatus);
        this.dgvReaders.SelectionChanged += new EventHandler(this.dgvReaders_SelectionChanged);
        this.dgvReaders.CellFormatting += new DataGridViewCellFormattingEventHandler(this.dgvReaders_CellFormatting);

        // ============================================================
        // pnlForm - panel nhập liệu (right side)
        // ============================================================
        this.pnlForm.BackColor = System.Drawing.Color.White;
        this.pnlForm.Dock = DockStyle.Right;
        this.pnlForm.Padding = new Padding(20);
        this.pnlForm.Width = 360;
        this.pnlForm.AutoScroll = true;     // form có nhiều field → cho scroll khi cần
        this.pnlForm.Controls.Add(this.pnlButtons);
        this.pnlForm.Controls.Add(this.cboStatus);
        this.pnlForm.Controls.Add(this.lblStatus);
        this.pnlForm.Controls.Add(this.dtpExpireDate);
        this.pnlForm.Controls.Add(this.lblExpireDate);
        this.pnlForm.Controls.Add(this.dtpIssueDate);
        this.pnlForm.Controls.Add(this.lblIssueDate);
        this.pnlForm.Controls.Add(this.txtEmail);
        this.pnlForm.Controls.Add(this.lblEmail);
        this.pnlForm.Controls.Add(this.txtPhone);
        this.pnlForm.Controls.Add(this.lblPhone);
        this.pnlForm.Controls.Add(this.txtAddress);
        this.pnlForm.Controls.Add(this.lblAddress);
        this.pnlForm.Controls.Add(this.cboGender);
        this.pnlForm.Controls.Add(this.lblGender);
        this.pnlForm.Controls.Add(this.dtpDob);
        this.pnlForm.Controls.Add(this.lblDob);
        this.pnlForm.Controls.Add(this.txtFullName);
        this.pnlForm.Controls.Add(this.lblFullName);
        this.pnlForm.Controls.Add(this.txtCard);
        this.pnlForm.Controls.Add(this.lblCard);
        this.pnlForm.Controls.Add(this.lblFormHeader);

        this.lblFormHeader.AutoSize = true;
        this.lblFormHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold);
        this.lblFormHeader.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
        this.lblFormHeader.Location = new System.Drawing.Point(20, 15);
        this.lblFormHeader.Text = "Thông tin độc giả";

        ConfigureFieldLabel(this.lblCard, "Số thẻ", 50);
        ConfigureFieldText(this.txtCard, 70);
        this.txtCard.MaxLength = 20;

        ConfigureFieldLabel(this.lblFullName, "Họ và tên", 105);
        ConfigureFieldText(this.txtFullName, 125);
        this.txtFullName.MaxLength = 100;

        ConfigureFieldLabel(this.lblDob, "Ngày sinh", 160);
        this.dtpDob.Format = DateTimePickerFormat.Short;
        this.dtpDob.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.dtpDob.Location = new System.Drawing.Point(20, 180);
        this.dtpDob.Size = new System.Drawing.Size(150, 25);
        this.dtpDob.MaxDate = DateTime.Today;
        this.dtpDob.Value = new DateTime(2000, 1, 1);

        ConfigureFieldLabel(this.lblGender, "Giới tính", 160);
        this.lblGender.Location = new System.Drawing.Point(190, 160);
        this.cboGender.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboGender.FlatStyle = FlatStyle.Flat;
        this.cboGender.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboGender.Location = new System.Drawing.Point(190, 180);
        this.cboGender.Size = new System.Drawing.Size(130, 25);

        ConfigureFieldLabel(this.lblAddress, "Địa chỉ", 215);
        ConfigureFieldText(this.txtAddress, 235);
        this.txtAddress.MaxLength = 200;

        ConfigureFieldLabel(this.lblPhone, "Số điện thoại", 270);
        ConfigureFieldText(this.txtPhone, 290);
        this.txtPhone.MaxLength = 15;

        ConfigureFieldLabel(this.lblEmail, "Email", 325);
        ConfigureFieldText(this.txtEmail, 345);
        this.txtEmail.MaxLength = 100;

        ConfigureFieldLabel(this.lblIssueDate, "Ngày cấp thẻ", 380);
        this.dtpIssueDate.Format = DateTimePickerFormat.Short;
        this.dtpIssueDate.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.dtpIssueDate.Location = new System.Drawing.Point(20, 400);
        this.dtpIssueDate.Size = new System.Drawing.Size(150, 25);
        this.dtpIssueDate.Value = DateTime.Today;

        ConfigureFieldLabel(this.lblExpireDate, "Hết hạn", 380);
        this.lblExpireDate.Location = new System.Drawing.Point(190, 380);
        this.dtpExpireDate.Format = DateTimePickerFormat.Short;
        this.dtpExpireDate.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.dtpExpireDate.Location = new System.Drawing.Point(190, 400);
        this.dtpExpireDate.Size = new System.Drawing.Size(130, 25);
        this.dtpExpireDate.Value = DateTime.Today.AddYears(2);

        ConfigureFieldLabel(this.lblStatus, "Trạng thái", 435);
        this.cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboStatus.FlatStyle = FlatStyle.Flat;
        this.cboStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboStatus.Location = new System.Drawing.Point(20, 455);
        this.cboStatus.Size = new System.Drawing.Size(300, 25);

        // ============================================================
        // pnlButtons
        // ============================================================
        this.pnlButtons.Controls.Add(this.btnCancel);
        this.pnlButtons.Controls.Add(this.btnDelete);
        this.pnlButtons.Controls.Add(this.btnRenew);
        this.pnlButtons.Controls.Add(this.btnUpdate);
        this.pnlButtons.Controls.Add(this.btnSave);
        this.pnlButtons.Controls.Add(this.btnNew);
        this.pnlButtons.Location = new System.Drawing.Point(20, 500);
        this.pnlButtons.Size = new System.Drawing.Size(300, 170);

        ConfigureActionButton(this.btnNew, "➕  Thêm mới", System.Drawing.Color.FromArgb(33, 64, 154), this.btnNew_Click);
        this.btnNew.Top = 0; this.btnNew.Left = 0; this.btnNew.Width = 300;

        ConfigureActionButton(this.btnSave, "💾  Lưu", System.Drawing.Color.FromArgb(16, 185, 129), this.btnSave_Click);
        this.btnSave.Top = 40; this.btnSave.Left = 0; this.btnSave.Width = 145;

        ConfigureActionButton(this.btnUpdate, "✏  Cập nhật", System.Drawing.Color.FromArgb(245, 158, 11), this.btnUpdate_Click);
        this.btnUpdate.Top = 40; this.btnUpdate.Left = 155; this.btnUpdate.Width = 145;

        ConfigureActionButton(this.btnRenew, "🔄  Gia hạn thẻ", System.Drawing.Color.FromArgb(99, 102, 241), this.btnRenew_Click);
        this.btnRenew.Top = 80; this.btnRenew.Left = 0; this.btnRenew.Width = 300;

        ConfigureActionButton(this.btnDelete, "🗑  Xoá", System.Drawing.Color.FromArgb(239, 68, 68), this.btnDelete_Click);
        this.btnDelete.Top = 120; this.btnDelete.Left = 0; this.btnDelete.Width = 145;

        ConfigureActionButton(this.btnCancel, "✖  Bỏ chọn", System.Drawing.Color.FromArgb(120, 120, 120), this.btnCancel_Click);
        this.btnCancel.Top = 120; this.btnCancel.Left = 155; this.btnCancel.Width = 145;

        // ============================================================
        // UserControl
        // ============================================================
        this.BackColor = System.Drawing.Color.FromArgb(245, 247, 251);
        this.Controls.Add(this.dgvReaders);
        this.Controls.Add(this.pnlForm);
        this.Controls.Add(this.pnlFilter);
        this.Name = "UcReaderList";
        this.Size = new System.Drawing.Size(1100, 700);

        ((System.ComponentModel.ISupportInitialize)(this.dgvReaders)).EndInit();
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
    private Label lblFilterStatus = null!;
    private ComboBox cboFilterStatus = null!;
    private Button btnSearch = null!;
    private Button btnReload = null!;

    private DataGridView dgvReaders = null!;

    private Panel pnlForm = null!;
    private Label lblFormHeader = null!;
    private Label lblCard = null!;
    private TextBox txtCard = null!;
    private Label lblFullName = null!;
    private TextBox txtFullName = null!;
    private Label lblDob = null!;
    private DateTimePicker dtpDob = null!;
    private Label lblGender = null!;
    private ComboBox cboGender = null!;
    private Label lblAddress = null!;
    private TextBox txtAddress = null!;
    private Label lblPhone = null!;
    private TextBox txtPhone = null!;
    private Label lblEmail = null!;
    private TextBox txtEmail = null!;
    private Label lblIssueDate = null!;
    private DateTimePicker dtpIssueDate = null!;
    private Label lblExpireDate = null!;
    private DateTimePicker dtpExpireDate = null!;
    private Label lblStatus = null!;
    private ComboBox cboStatus = null!;

    private Panel pnlButtons = null!;
    private Button btnNew = null!;
    private Button btnSave = null!;
    private Button btnUpdate = null!;
    private Button btnRenew = null!;
    private Button btnDelete = null!;
    private Button btnCancel = null!;

    private ErrorProvider errorProvider = null!;
}
