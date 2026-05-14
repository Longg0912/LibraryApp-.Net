namespace LibraryApp.UI.Forms;

partial class FrmLogin
{
    /// <summary>Required designer variable.</summary>
    private System.ComponentModel.IContainer? components = null;

    /// <summary>Clean up any resources being used.</summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support — do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        // ------------------------------------------------------------
        // Controls khai báo
        // ------------------------------------------------------------
        this.pnlLeft = new Panel();
        this.pnlRight = new Panel();
        this.lblTitle = new Label();
        this.lblSubtitle = new Label();
        this.lblFormTitle = new Label();
        this.lblFormSubtitle = new Label();
        this.lblUsername = new Label();
        this.txtUsername = new TextBox();
        this.lblPassword = new Label();
        this.txtPassword = new TextBox();
        this.chkShowPassword = new CheckBox();
        this.btnLogin = new Button();
        this.btnExit = new Button();
        this.lblFooter = new Label();
        this.errorProvider = new ErrorProvider(this.components ??= new System.ComponentModel.Container());

        this.pnlLeft.SuspendLayout();
        this.pnlRight.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
        this.SuspendLayout();

        // ------------------------------------------------------------
        // pnlLeft  (banner màu xanh phía trái)
        // ------------------------------------------------------------
        this.pnlLeft.BackColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.pnlLeft.Controls.Add(this.lblSubtitle);
        this.pnlLeft.Controls.Add(this.lblTitle);
        this.pnlLeft.Dock = DockStyle.Left;
        this.pnlLeft.Location = new System.Drawing.Point(0, 0);
        this.pnlLeft.Name = "pnlLeft";
        this.pnlLeft.Size = new System.Drawing.Size(360, 520);
        this.pnlLeft.TabIndex = 0;

        // lblTitle
        this.lblTitle.AutoSize = false;
        this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 22F, System.Drawing.FontStyle.Bold);
        this.lblTitle.ForeColor = System.Drawing.Color.White;
        this.lblTitle.Location = new System.Drawing.Point(30, 200);
        this.lblTitle.Name = "lblTitle";
        this.lblTitle.Size = new System.Drawing.Size(300, 90);
        this.lblTitle.Text = "HỆ THỐNG\r\nQUẢN LÝ THƯ VIỆN";

        // lblSubtitle
        this.lblSubtitle.AutoSize = false;
        this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 11F);
        this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(200, 215, 255);
        this.lblSubtitle.Location = new System.Drawing.Point(30, 295);
        this.lblSubtitle.Name = "lblSubtitle";
        this.lblSubtitle.Size = new System.Drawing.Size(300, 50);
        this.lblSubtitle.Text = "Nền tảng nội bộ dành cho cán bộ\r\nthư viện và quản trị viên.";

        // ------------------------------------------------------------
        // pnlRight  (vùng nhập đăng nhập)
        // ------------------------------------------------------------
        this.pnlRight.BackColor = System.Drawing.Color.White;
        this.pnlRight.Controls.Add(this.lblFooter);
        this.pnlRight.Controls.Add(this.btnExit);
        this.pnlRight.Controls.Add(this.btnLogin);
        this.pnlRight.Controls.Add(this.chkShowPassword);
        this.pnlRight.Controls.Add(this.txtPassword);
        this.pnlRight.Controls.Add(this.lblPassword);
        this.pnlRight.Controls.Add(this.txtUsername);
        this.pnlRight.Controls.Add(this.lblUsername);
        this.pnlRight.Controls.Add(this.lblFormSubtitle);
        this.pnlRight.Controls.Add(this.lblFormTitle);
        this.pnlRight.Dock = DockStyle.Fill;
        this.pnlRight.Location = new System.Drawing.Point(360, 0);
        this.pnlRight.Name = "pnlRight";
        this.pnlRight.Padding = new Padding(45, 60, 45, 25);
        this.pnlRight.Size = new System.Drawing.Size(540, 520);
        this.pnlRight.TabIndex = 1;

        // lblFormTitle
        this.lblFormTitle.AutoSize = true;
        this.lblFormTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold);
        this.lblFormTitle.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
        this.lblFormTitle.Location = new System.Drawing.Point(45, 60);
        this.lblFormTitle.Name = "lblFormTitle";
        this.lblFormTitle.Text = "Đăng nhập";

        // lblFormSubtitle
        this.lblFormSubtitle.AutoSize = true;
        this.lblFormSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
        this.lblFormSubtitle.ForeColor = System.Drawing.Color.FromArgb(110, 110, 110);
        this.lblFormSubtitle.Location = new System.Drawing.Point(46, 100);
        this.lblFormSubtitle.Name = "lblFormSubtitle";
        this.lblFormSubtitle.Text = "Vui lòng sử dụng tài khoản nhân viên do quản trị cấp.";

        // lblUsername
        this.lblUsername.AutoSize = true;
        this.lblUsername.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
        this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.lblUsername.Location = new System.Drawing.Point(46, 150);
        this.lblUsername.Name = "lblUsername";
        this.lblUsername.Text = "TÊN ĐĂNG NHẬP";

        // txtUsername
        this.txtUsername.BorderStyle = BorderStyle.FixedSingle;
        this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 11F);
        this.txtUsername.Location = new System.Drawing.Point(46, 173);
        this.txtUsername.MaxLength = 50;
        this.txtUsername.Name = "txtUsername";
        this.txtUsername.PlaceholderText = "Nhập tên đăng nhập";
        this.txtUsername.Size = new System.Drawing.Size(449, 32);
        this.txtUsername.TabIndex = 0;

        // lblPassword
        this.lblPassword.AutoSize = true;
        this.lblPassword.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
        this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.lblPassword.Location = new System.Drawing.Point(46, 222);
        this.lblPassword.Name = "lblPassword";
        this.lblPassword.Text = "MẬT KHẨU";

        // txtPassword
        this.txtPassword.BorderStyle = BorderStyle.FixedSingle;
        this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
        this.txtPassword.Location = new System.Drawing.Point(46, 245);
        this.txtPassword.MaxLength = 100;
        this.txtPassword.Name = "txtPassword";
        this.txtPassword.PasswordChar = '●';
        this.txtPassword.PlaceholderText = "Nhập mật khẩu";
        this.txtPassword.Size = new System.Drawing.Size(449, 32);
        this.txtPassword.TabIndex = 1;
        this.txtPassword.UseSystemPasswordChar = false;

        // chkShowPassword
        this.chkShowPassword.AutoSize = true;
        this.chkShowPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.chkShowPassword.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.chkShowPassword.Location = new System.Drawing.Point(46, 285);
        this.chkShowPassword.Name = "chkShowPassword";
        this.chkShowPassword.TabIndex = 2;
        this.chkShowPassword.Text = "Hiển thị mật khẩu";
        this.chkShowPassword.UseVisualStyleBackColor = true;
        this.chkShowPassword.CheckedChanged += new EventHandler(this.chkShowPassword_CheckedChanged);

        // btnLogin
        this.btnLogin.BackColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.btnLogin.FlatAppearance.BorderSize = 0;
        this.btnLogin.FlatStyle = FlatStyle.Flat;
        this.btnLogin.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
        this.btnLogin.ForeColor = System.Drawing.Color.White;
        this.btnLogin.Location = new System.Drawing.Point(46, 330);
        this.btnLogin.Name = "btnLogin";
        this.btnLogin.Size = new System.Drawing.Size(449, 44);
        this.btnLogin.TabIndex = 3;
        this.btnLogin.Text = "ĐĂNG NHẬP";
        this.btnLogin.UseVisualStyleBackColor = false;
        this.btnLogin.Cursor = Cursors.Hand;
        this.btnLogin.Click += new EventHandler(this.btnLogin_Click);

        // btnExit
        this.btnExit.BackColor = System.Drawing.Color.White;
        this.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 220, 220);
        this.btnExit.FlatAppearance.BorderSize = 1;
        this.btnExit.FlatStyle = FlatStyle.Flat;
        this.btnExit.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
        this.btnExit.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.btnExit.Location = new System.Drawing.Point(46, 385);
        this.btnExit.Name = "btnExit";
        this.btnExit.Size = new System.Drawing.Size(449, 38);
        this.btnExit.TabIndex = 4;
        this.btnExit.Text = "THOÁT";
        this.btnExit.UseVisualStyleBackColor = false;
        this.btnExit.Cursor = Cursors.Hand;
        this.btnExit.Click += new EventHandler(this.btnExit_Click);

        // lblFooter
        this.lblFooter.AutoSize = false;
        this.lblFooter.Font = new System.Drawing.Font("Segoe UI", 8.25F);
        this.lblFooter.ForeColor = System.Drawing.Color.FromArgb(160, 160, 160);
        this.lblFooter.Location = new System.Drawing.Point(46, 470);
        this.lblFooter.Name = "lblFooter";
        this.lblFooter.Size = new System.Drawing.Size(449, 20);
        this.lblFooter.Text = "© Library Management • WinForms .NET 10";
        this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        // errorProvider
        this.errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        this.errorProvider.ContainerControl = this;

        // ------------------------------------------------------------
        // FrmLogin
        // ------------------------------------------------------------
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.AcceptButton = this.btnLogin;
        this.CancelButton = this.btnExit;
        this.BackColor = System.Drawing.Color.White;
        this.ClientSize = new System.Drawing.Size(900, 520);
        this.Controls.Add(this.pnlRight);
        this.Controls.Add(this.pnlLeft);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Name = "FrmLogin";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Đăng nhập - Hệ thống Quản lý Thư viện";
        this.Load += new EventHandler(this.FrmLogin_Load);

        this.pnlLeft.ResumeLayout(false);
        this.pnlLeft.PerformLayout();
        this.pnlRight.ResumeLayout(false);
        this.pnlRight.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
        this.ResumeLayout(false);
    }

    #endregion

    // ----------- Control fields -----------
    private Panel pnlLeft = null!;
    private Panel pnlRight = null!;
    private Label lblTitle = null!;
    private Label lblSubtitle = null!;
    private Label lblFormTitle = null!;
    private Label lblFormSubtitle = null!;
    private Label lblUsername = null!;
    private TextBox txtUsername = null!;
    private Label lblPassword = null!;
    private TextBox txtPassword = null!;
    private CheckBox chkShowPassword = null!;
    private Button btnLogin = null!;
    private Button btnExit = null!;
    private Label lblFooter = null!;
    private ErrorProvider errorProvider = null!;
}
