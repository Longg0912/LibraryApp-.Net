namespace LibraryApp.UI.Forms;

partial class FrmMain
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        // ============================================================
        // Controls
        // ============================================================
        this.menuStrip = new MenuStrip();
        this.menuFile = new ToolStripMenuItem();
        this.menuFileExit = new ToolStripMenuItem();
        this.menuBook = new ToolStripMenuItem();
        this.menuBookList = new ToolStripMenuItem();
        this.menuBookCategory = new ToolStripMenuItem();
        this.menuReader = new ToolStripMenuItem();
        this.menuBorrow = new ToolStripMenuItem();
        this.menuBorrowCreate = new ToolStripMenuItem();
        this.menuBorrowReturn = new ToolStripMenuItem();
        this.menuBorrowList = new ToolStripMenuItem();
        this.menuReport = new ToolStripMenuItem();
        this.menuReportTop = new ToolStripMenuItem();
        this.menuReportOverdue = new ToolStripMenuItem();
        this.menuReportFine = new ToolStripMenuItem();
        this.menuAccount = new ToolStripMenuItem();
        this.menuAccountUsers = new ToolStripMenuItem();
        this.menuAccountChangePwd = new ToolStripMenuItem();
        this.menuAccountLogout = new ToolStripMenuItem();
        this.menuHelp = new ToolStripMenuItem();
        this.menuHelpAbout = new ToolStripMenuItem();

        this.pnlSidebar = new Panel();
        this.btnNavDashboard = new Button();
        this.btnNavBooks = new Button();
        this.btnNavCategories = new Button();
        this.btnNavReaders = new Button();
        this.btnNavBorrow = new Button();
        this.btnNavReports = new Button();
        this.btnNavUsers = new Button();
        this.btnNavLogout = new Button();

        this.pnlHeader = new Panel();
        this.lblPageTitle = new Label();
        this.lblPageSubtitle = new Label();

        this.pnlContent = new Panel();
        this.pnlContentHost = new Panel();   // nơi load UserControl

        this.statusStrip = new StatusStrip();
        this.lblStatusUser = new ToolStripStatusLabel();
        this.lblStatusRole = new ToolStripStatusLabel();
        this.lblStatusFiller = new ToolStripStatusLabel();
        this.lblStatusTime = new ToolStripStatusLabel();
        this.timerClock = new System.Windows.Forms.Timer(this.components ??= new System.ComponentModel.Container());

        this.menuStrip.SuspendLayout();
        this.pnlSidebar.SuspendLayout();
        this.pnlHeader.SuspendLayout();
        this.pnlContent.SuspendLayout();
        this.statusStrip.SuspendLayout();
        this.SuspendLayout();

        // ============================================================
        // menuStrip - menu chính trên cùng
        // ============================================================
        this.menuStrip.BackColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.menuStrip.ForeColor = System.Drawing.Color.White;
        this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
        this.menuStrip.Items.AddRange(new ToolStripItem[] {
            this.menuFile, this.menuBook, this.menuReader,
            this.menuBorrow, this.menuReport, this.menuAccount, this.menuHelp });
        this.menuStrip.Location = new System.Drawing.Point(0, 0);
        this.menuStrip.Name = "menuStrip";
        this.menuStrip.Padding = new Padding(8, 6, 0, 6);
        this.menuStrip.Size = new System.Drawing.Size(1200, 32);
        this.menuStrip.RenderMode = ToolStripRenderMode.Professional;

        // -- menuFile --
        this.menuFile.Text = "&Hệ thống";
        this.menuFile.ForeColor = System.Drawing.Color.White;
        this.menuFile.DropDownItems.AddRange(new ToolStripItem[] { this.menuFileExit });

        this.menuFileExit.Text = "&Thoát";
        this.menuFileExit.ShortcutKeys = Keys.Alt | Keys.F4;
        this.menuFileExit.Click += new EventHandler(this.menuFileExit_Click);

        // -- menuBook --
        this.menuBook.Text = "&Sách";
        this.menuBook.ForeColor = System.Drawing.Color.White;
        this.menuBook.DropDownItems.AddRange(new ToolStripItem[] {
            this.menuBookList, this.menuBookCategory });

        this.menuBookList.Text = "&Danh sách sách";
        this.menuBookList.ShortcutKeys = Keys.Control | Keys.B;
        this.menuBookList.Click += new EventHandler(this.menuBookList_Click);

        this.menuBookCategory.Text = "Danh &mục sách";
        this.menuBookCategory.ShortcutKeys = Keys.Control | Keys.M;
        this.menuBookCategory.Click += new EventHandler(this.menuBookCategory_Click);

        // -- menuReader --
        this.menuReader.Text = "Độc &giả";
        this.menuReader.ForeColor = System.Drawing.Color.White;
        this.menuReader.ShortcutKeys = Keys.Control | Keys.D;
        this.menuReader.Click += new EventHandler(this.menuReader_Click);

        // -- menuBorrow --
        this.menuBorrow.Text = "&Mượn / Trả";
        this.menuBorrow.ForeColor = System.Drawing.Color.White;
        this.menuBorrow.DropDownItems.AddRange(new ToolStripItem[] {
            this.menuBorrowCreate, this.menuBorrowReturn, this.menuBorrowList });

        this.menuBorrowCreate.Text = "&Lập phiếu mượn";
        this.menuBorrowCreate.ShortcutKeys = Keys.Control | Keys.N;
        this.menuBorrowCreate.Click += new EventHandler(this.menuBorrowCreate_Click);

        this.menuBorrowReturn.Text = "&Ghi nhận trả sách";
        this.menuBorrowReturn.ShortcutKeys = Keys.Control | Keys.R;
        this.menuBorrowReturn.Click += new EventHandler(this.menuBorrowReturn_Click);

        this.menuBorrowList.Text = "&Danh sách phiếu mượn";
        this.menuBorrowList.Click += new EventHandler(this.menuBorrowList_Click);

        // -- menuReport --
        this.menuReport.Text = "&Thống kê";
        this.menuReport.ForeColor = System.Drawing.Color.White;
        this.menuReport.DropDownItems.AddRange(new ToolStripItem[] {
            this.menuReportTop, this.menuReportOverdue, this.menuReportFine });

        this.menuReportTop.Text = "Top sách &mượn nhiều";
        this.menuReportTop.Click += new EventHandler(this.menuReportTop_Click);

        this.menuReportOverdue.Text = "Sách &quá hạn";
        this.menuReportOverdue.Click += new EventHandler(this.menuReportOverdue_Click);

        this.menuReportFine.Text = "Doanh thu tiền &phạt";
        this.menuReportFine.Click += new EventHandler(this.menuReportFine_Click);

        // -- menuAccount --
        this.menuAccount.Text = "Tài &khoản";
        this.menuAccount.ForeColor = System.Drawing.Color.White;
        this.menuAccount.DropDownItems.AddRange(new ToolStripItem[] {
            this.menuAccountUsers, this.menuAccountChangePwd,
            new ToolStripSeparator(), this.menuAccountLogout });

        this.menuAccountUsers.Text = "Quản lý &người dùng";
        this.menuAccountUsers.Click += new EventHandler(this.menuAccountUsers_Click);

        this.menuAccountChangePwd.Text = "&Đổi mật khẩu";
        this.menuAccountChangePwd.Click += new EventHandler(this.menuAccountChangePwd_Click);

        this.menuAccountLogout.Text = "Đăng &xuất";
        this.menuAccountLogout.ShortcutKeys = Keys.Control | Keys.L;
        this.menuAccountLogout.Click += new EventHandler(this.menuAccountLogout_Click);

        // -- menuHelp --
        this.menuHelp.Text = "&Trợ giúp";
        this.menuHelp.ForeColor = System.Drawing.Color.White;
        this.menuHelp.DropDownItems.AddRange(new ToolStripItem[] { this.menuHelpAbout });

        this.menuHelpAbout.Text = "&Về phần mềm";
        this.menuHelpAbout.Click += new EventHandler(this.menuHelpAbout_Click);

        // ============================================================
        // pnlSidebar - sidebar trái với navigation buttons
        // ============================================================
        this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(28, 53, 130);
        this.pnlSidebar.Controls.Add(this.btnNavLogout);
        this.pnlSidebar.Controls.Add(this.btnNavUsers);
        this.pnlSidebar.Controls.Add(this.btnNavReports);
        this.pnlSidebar.Controls.Add(this.btnNavBorrow);
        this.pnlSidebar.Controls.Add(this.btnNavReaders);
        this.pnlSidebar.Controls.Add(this.btnNavCategories);
        this.pnlSidebar.Controls.Add(this.btnNavBooks);
        this.pnlSidebar.Controls.Add(this.btnNavDashboard);
        this.pnlSidebar.Dock = DockStyle.Left;
        this.pnlSidebar.Padding = new Padding(0, 16, 0, 0);
        this.pnlSidebar.Size = new System.Drawing.Size(220, 0);

        // Cấu hình các nút sidebar đồng nhất qua helper bên dưới
        ConfigureSidebarButton(this.btnNavDashboard, "🏠   Tổng quan", 0, this.btnNavDashboard_Click);
        ConfigureSidebarButton(this.btnNavBooks, "📚   Quản lý sách", 50, this.btnNavBooks_Click);
        ConfigureSidebarButton(this.btnNavCategories, "🏷️   Danh mục", 100, this.btnNavCategories_Click);
        ConfigureSidebarButton(this.btnNavReaders, "👥   Độc giả", 150, this.btnNavReaders_Click);
        ConfigureSidebarButton(this.btnNavBorrow, "🔄   Mượn / Trả", 200, this.btnNavBorrow_Click);
        ConfigureSidebarButton(this.btnNavReports, "📊   Thống kê", 250, this.btnNavReports_Click);
        ConfigureSidebarButton(this.btnNavUsers, "👤   Người dùng", 300, this.btnNavUsers_Click);

        // Nút Đăng xuất dock-bottom (style đỏ nhẹ)
        this.btnNavLogout.BackColor = System.Drawing.Color.FromArgb(28, 53, 130);
        this.btnNavLogout.Dock = DockStyle.Bottom;
        this.btnNavLogout.FlatAppearance.BorderSize = 0;
        this.btnNavLogout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(180, 50, 60);
        this.btnNavLogout.FlatStyle = FlatStyle.Flat;
        this.btnNavLogout.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F);
        this.btnNavLogout.ForeColor = System.Drawing.Color.FromArgb(255, 200, 200);
        this.btnNavLogout.Height = 48;
        this.btnNavLogout.Text = "🚪   Đăng xuất";
        this.btnNavLogout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.btnNavLogout.Padding = new Padding(20, 0, 0, 0);
        this.btnNavLogout.UseVisualStyleBackColor = false;
        this.btnNavLogout.Cursor = Cursors.Hand;
        this.btnNavLogout.Click += new EventHandler(this.btnNavLogout_Click);

        // ============================================================
        // pnlHeader - thanh tiêu đề trang
        // ============================================================
        this.pnlHeader.BackColor = System.Drawing.Color.White;
        this.pnlHeader.Controls.Add(this.lblPageSubtitle);
        this.pnlHeader.Controls.Add(this.lblPageTitle);
        this.pnlHeader.Dock = DockStyle.Top;
        this.pnlHeader.Padding = new Padding(28, 18, 28, 12);
        this.pnlHeader.Size = new System.Drawing.Size(0, 80);

        this.lblPageTitle.AutoSize = true;
        this.lblPageTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold);
        this.lblPageTitle.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
        this.lblPageTitle.Location = new System.Drawing.Point(28, 18);
        this.lblPageTitle.Text = "Tổng quan";

        this.lblPageSubtitle.AutoSize = true;
        this.lblPageSubtitle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.lblPageSubtitle.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
        this.lblPageSubtitle.Location = new System.Drawing.Point(30, 50);
        this.lblPageSubtitle.Text = "Thông tin tổng hợp của hệ thống";

        // ============================================================
        // pnlContent - vùng nội dung chính
        // ============================================================
        this.pnlContent.BackColor = System.Drawing.Color.FromArgb(245, 247, 251);
        this.pnlContent.Controls.Add(this.pnlContentHost);
        this.pnlContent.Controls.Add(this.pnlHeader);
        this.pnlContent.Dock = DockStyle.Fill;
        this.pnlContent.Padding = new Padding(0);

        // pnlContentHost - nơi load UserControl, fill toàn bộ phần dưới header
        this.pnlContentHost.BackColor = System.Drawing.Color.FromArgb(245, 247, 251);
        this.pnlContentHost.Dock = DockStyle.Fill;
        this.pnlContentHost.Padding = new Padding(20);

        // ============================================================
        // statusStrip - thanh trạng thái dưới cùng
        // ============================================================
        this.statusStrip.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
        this.statusStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.statusStrip.Items.AddRange(new ToolStripItem[] {
            this.lblStatusUser, this.lblStatusRole,
            this.lblStatusFiller, this.lblStatusTime });
        this.statusStrip.SizingGrip = false;

        this.lblStatusUser.Text = "👤 -";
        this.lblStatusUser.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblStatusUser.Padding = new Padding(8, 0, 16, 0);

        this.lblStatusRole.Text = "Vai trò: -";
        this.lblStatusRole.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblStatusRole.BorderSides = ToolStripStatusLabelBorderSides.Left;
        this.lblStatusRole.BorderStyle = Border3DStyle.Etched;
        this.lblStatusRole.Padding = new Padding(8, 0, 16, 0);

        this.lblStatusFiller.Spring = true;

        this.lblStatusTime.Text = "🕒 -";
        this.lblStatusTime.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblStatusTime.Padding = new Padding(8, 0, 8, 0);

        // timer cập nhật đồng hồ trên status bar
        this.timerClock.Interval = 1000;
        this.timerClock.Tick += new EventHandler(this.timerClock_Tick);

        // ============================================================
        // FrmMain
        // ============================================================
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.White;
        this.ClientSize = new System.Drawing.Size(1200, 720);
        this.Controls.Add(this.pnlContent);
        this.Controls.Add(this.pnlSidebar);
        this.Controls.Add(this.statusStrip);
        this.Controls.Add(this.menuStrip);
        this.MainMenuStrip = this.menuStrip;
        this.MinimumSize = new System.Drawing.Size(1024, 640);
        this.Name = "FrmMain";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Hệ thống Quản lý Thư viện";
        this.WindowState = FormWindowState.Maximized;
        this.FormClosing += new FormClosingEventHandler(this.FrmMain_FormClosing);

        this.menuStrip.ResumeLayout(false);
        this.menuStrip.PerformLayout();
        this.pnlSidebar.ResumeLayout(false);
        this.pnlHeader.ResumeLayout(false);
        this.pnlHeader.PerformLayout();
        this.pnlContent.ResumeLayout(false);
        this.statusStrip.ResumeLayout(false);
        this.statusStrip.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    /// <summary>
    /// Helper cấu hình các nút sidebar theo cùng style (font, màu, padding...).
    /// </summary>
    private void ConfigureSidebarButton(Button btn, string text, int top, EventHandler onClick)
    {
        btn.BackColor = System.Drawing.Color.FromArgb(28, 53, 130);
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(45, 75, 165);
        btn.FlatStyle = FlatStyle.Flat;
        btn.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F);
        btn.ForeColor = System.Drawing.Color.White;
        btn.Dock = DockStyle.Top;
        btn.Height = 48;
        btn.Padding = new Padding(20, 0, 0, 0);
        btn.Text = text;
        btn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        btn.UseVisualStyleBackColor = false;
        btn.Cursor = Cursors.Hand;
        btn.Click += onClick;
    }

    #endregion

    // ============================================================
    // Control fields
    // ============================================================
    private MenuStrip menuStrip = null!;
    private ToolStripMenuItem menuFile = null!;
    private ToolStripMenuItem menuFileExit = null!;
    private ToolStripMenuItem menuBook = null!;
    private ToolStripMenuItem menuBookList = null!;
    private ToolStripMenuItem menuBookCategory = null!;
    private ToolStripMenuItem menuReader = null!;
    private ToolStripMenuItem menuBorrow = null!;
    private ToolStripMenuItem menuBorrowCreate = null!;
    private ToolStripMenuItem menuBorrowReturn = null!;
    private ToolStripMenuItem menuBorrowList = null!;
    private ToolStripMenuItem menuReport = null!;
    private ToolStripMenuItem menuReportTop = null!;
    private ToolStripMenuItem menuReportOverdue = null!;
    private ToolStripMenuItem menuReportFine = null!;
    private ToolStripMenuItem menuAccount = null!;
    private ToolStripMenuItem menuAccountUsers = null!;
    private ToolStripMenuItem menuAccountChangePwd = null!;
    private ToolStripMenuItem menuAccountLogout = null!;
    private ToolStripMenuItem menuHelp = null!;
    private ToolStripMenuItem menuHelpAbout = null!;

    private Panel pnlSidebar = null!;
    private Button btnNavDashboard = null!;
    private Button btnNavBooks = null!;
    private Button btnNavCategories = null!;
    private Button btnNavReaders = null!;
    private Button btnNavBorrow = null!;
    private Button btnNavReports = null!;
    private Button btnNavUsers = null!;
    private Button btnNavLogout = null!;

    private Panel pnlHeader = null!;
    private Label lblPageTitle = null!;
    private Label lblPageSubtitle = null!;

    private Panel pnlContent = null!;
    private Panel pnlContentHost = null!;

    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel lblStatusUser = null!;
    private ToolStripStatusLabel lblStatusRole = null!;
    private ToolStripStatusLabel lblStatusFiller = null!;
    private ToolStripStatusLabel lblStatusTime = null!;
    private System.Windows.Forms.Timer timerClock = null!;
}
