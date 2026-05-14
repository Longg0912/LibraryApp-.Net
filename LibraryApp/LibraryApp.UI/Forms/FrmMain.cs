using LibraryApp.UI.Common;
using LibraryApp.UI.UserControls;

namespace LibraryApp.UI.Forms;

/// <summary>
/// Form chính của ứng dụng sau khi đăng nhập thành công.
/// <para>
/// Layout: <c>MenuStrip</c> trên cùng, <c>Sidebar</c> bên trái, <c>StatusStrip</c>
/// dưới cùng, vùng giữa là <see cref="pnlContentHost"/> để load <see cref="UserControl"/>
/// (Dashboard, BookList, ReaderList, BorrowList, Reports...).
/// </para>
/// <para>
/// Mỗi mục menu / nút sidebar gọi <see cref="LoadView{T}"/> — phương thức
/// chung giúp tránh lặp code khi load nhiều màn hình.
/// </para>
/// </summary>
public partial class FrmMain : Form
{
    /// <summary>UserControl hiện đang hiển thị (để dispose khi chuyển trang).</summary>
    private UserControl? _currentView;

    /// <summary>Khởi tạo form.</summary>
    public FrmMain()
    {
        InitializeComponent();
        Load += FrmMain_Load;
    }

    // ================================================================
    // Form lifecycle
    // ================================================================

    private void FrmMain_Load(object? sender, EventArgs e)
    {
        // Bảo vệ: ngăn truy cập trực tiếp khi chưa đăng nhập
        if (!CurrentSession.IsAuthenticated)
        {
            MessageBox.Show("Bạn chưa đăng nhập.", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
            return;
        }

        // Cập nhật status bar
        var user = CurrentSession.CurrentUser!;
        lblStatusUser.Text = $"👤 {user.FullName}";
        lblStatusRole.Text = CurrentSession.IsAdmin
            ? "Vai trò: Quản trị viên"
            : CurrentSession.IsLibrarian
                ? "Vai trò: Thủ thư"
                : $"Vai trò: {CurrentSession.CurrentRoleCode}";
        UpdateClock();
        timerClock.Start();

        // Áp dụng phân quyền lên menu (ẩn các mục viewer/librarian không được dùng)
        ApplyPermissionToMenu();

        // Mở Dashboard mặc định
        LoadView<UcDashboard>("Tổng quan", "Thông tin tổng hợp của hệ thống");
    }

    /// <summary>
    /// Khi đóng MainForm: xác nhận với người dùng, dọn dẹp UserControl đang mở,
    /// đăng xuất và clear ngữ cảnh.
    /// </summary>
    private void FrmMain_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // Chỉ hỏi xác nhận khi user trực tiếp đóng (X / Alt+F4), không hỏi khi gọi Close() từ Logout
        if (e.CloseReason == CloseReason.UserClosing)
        {
            var result = MessageBox.Show(
                "Bạn có chắc muốn thoát ứng dụng?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
        }

        timerClock.Stop();
        _currentView?.Dispose();

        if (CurrentSession.IsAuthenticated)
        {
            ServiceLocator.Auth.Logout(CurrentSession.CurrentUser!.UserId);
            CurrentSession.SignOut();
        }
    }

    // ================================================================
    // Quản lý hiển thị UserControl trong pnlContentHost
    // ================================================================

    /// <summary>
    /// Phương thức trung tâm: load một <see cref="UserControl"/> vào
    /// <see cref="pnlContentHost"/>. Dispose control cũ trước khi thêm control mới
    /// để tránh memory leak khi user chuyển qua lại nhiều màn hình.
    /// </summary>
    /// <typeparam name="T">Kiểu UserControl cần load (phải có constructor không tham số).</typeparam>
    /// <param name="title">Tiêu đề trang hiển thị ở header.</param>
    /// <param name="subtitle">Phụ đề mô tả ngắn.</param>
    private void LoadView<T>(string title, string subtitle) where T : UserControl, new()
    {
        // Tránh load lại cùng một view (tiết kiệm tài nguyên + trải nghiệm mượt hơn)
        if (_currentView is T)
        {
            UpdateHeader(title, subtitle);
            return;
        }

        UseWaitCursor = true;
        SuspendLayout();
        try
        {
            // Dọn view cũ
            if (_currentView is not null)
            {
                pnlContentHost.Controls.Remove(_currentView);
                _currentView.Dispose();
                _currentView = null;
            }

            // Load view mới
            var view = new T { Dock = DockStyle.Fill };
            pnlContentHost.Controls.Add(view);
            _currentView = view;

            UpdateHeader(title, subtitle);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể mở màn hình: {ex.Message}",
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            ResumeLayout();
            UseWaitCursor = false;
        }
    }

    /// <summary>Cập nhật tiêu đề header.</summary>
    private void UpdateHeader(string title, string subtitle)
    {
        lblPageTitle.Text = title;
        lblPageSubtitle.Text = subtitle;
    }

    // ================================================================
    // Phân quyền: ẩn các menu/button mà role không được phép dùng
    // ================================================================

    /// <summary>
    /// Áp dụng phân quyền lên menu/sidebar dựa theo <see cref="AppContext"/>.
    /// </summary>
    /// <remarks>
    /// Quy tắc đơn giản trong dự án:
    /// <list type="bullet">
    /// <item>ADMIN: thấy tất cả.</item>
    /// <item>LIBRARIAN: không thấy "Quản lý người dùng".</item>
    /// <item>VIEWER (nếu có): chỉ thấy Dashboard + Thống kê (chế độ chỉ xem).</item>
    /// </list>
    /// </remarks>
    private void ApplyPermissionToMenu()
    {
        bool isAdmin = CurrentSession.IsAdmin;
        bool isLibrarian = CurrentSession.IsLibrarian;
        bool isViewer = !isAdmin && !isLibrarian;

        // Menu Tài khoản → Quản lý người dùng: chỉ Admin
        menuAccountUsers.Visible = isAdmin;
        btnNavUsers.Visible = isAdmin;

        // Menu Mượn/Trả + Sách + Độc giả: ẩn với Viewer (chỉ Admin/Librarian thao tác)
        if (isViewer)
        {
            menuBook.Visible = false;
            menuReader.Visible = false;
            menuBorrow.Visible = false;
            btnNavBooks.Visible = false;
            btnNavCategories.Visible = false;
            btnNavReaders.Visible = false;
            btnNavBorrow.Visible = false;
        }
    }

    // ================================================================
    // MenuStrip event handlers
    // ================================================================

    private void menuFileExit_Click(object? sender, EventArgs e) => Close();

    private void menuBookList_Click(object? sender, EventArgs e)
        => LoadView<UcBookList>("Quản lý sách", "Danh sách, thêm, sửa, xoá, tìm kiếm sách");

    private void menuBookCategory_Click(object? sender, EventArgs e)
        => LoadView<UcCategoryList>("Danh mục sách", "Quản lý danh mục phân loại sách");

    private void menuReader_Click(object? sender, EventArgs e)
        => LoadView<UcReaderList>("Độc giả", "Quản lý hồ sơ độc giả và thẻ thư viện");

    private void menuBorrowCreate_Click(object? sender, EventArgs e)
        => LoadView<UcPlaceholder>("Lập phiếu mượn", "Tạo phiếu mượn sách mới");

    private void menuBorrowReturn_Click(object? sender, EventArgs e)
        => LoadView<UcPlaceholder>("Ghi nhận trả sách", "Tiếp nhận trả sách và tính tiền phạt");

    private void menuBorrowList_Click(object? sender, EventArgs e)
        => LoadView<UcPlaceholder>("Danh sách phiếu mượn", "Lịch sử mượn / trả toàn hệ thống");

    private void menuReportTop_Click(object? sender, EventArgs e)
        => LoadView<UcPlaceholder>("Top sách mượn nhiều", "Bảng xếp hạng sách được mượn nhiều nhất");

    private void menuReportOverdue_Click(object? sender, EventArgs e)
        => LoadView<UcPlaceholder>("Sách quá hạn", "Danh sách phiếu mượn đã quá hạn trả");

    private void menuReportFine_Click(object? sender, EventArgs e)
        => LoadView<UcPlaceholder>("Doanh thu tiền phạt", "Thống kê tiền phạt theo thời gian");

    private void menuAccountUsers_Click(object? sender, EventArgs e)
        => LoadView<UcPlaceholder>("Quản lý người dùng", "Tài khoản nhân viên hệ thống");

    private void menuAccountChangePwd_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("Tính năng đang được xây dựng.", "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void menuAccountLogout_Click(object? sender, EventArgs e) => PerformLogout();

    private void menuHelpAbout_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "Hệ thống Quản lý Thư viện\n" +
            "Phiên bản 1.0\n" +
            "Nền tảng: WinForms .NET 10 + SQL Server 2022\n\n" +
            "© Library Management",
            "Về phần mềm",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // ================================================================
    // Sidebar button handlers (gọi cùng action với menu tương ứng)
    // ================================================================

    private void btnNavDashboard_Click(object? sender, EventArgs e)
        => LoadView<UcDashboard>("Tổng quan", "Thông tin tổng hợp của hệ thống");

    private void btnNavBooks_Click(object? sender, EventArgs e)
        => menuBookList_Click(sender, e);

    private void btnNavCategories_Click(object? sender, EventArgs e)
        => menuBookCategory_Click(sender, e);

    private void btnNavReaders_Click(object? sender, EventArgs e)
        => menuReader_Click(sender, e);

    private void btnNavBorrow_Click(object? sender, EventArgs e)
        => menuBorrowList_Click(sender, e);

    private void btnNavReports_Click(object? sender, EventArgs e)
        => menuReportTop_Click(sender, e);

    private void btnNavUsers_Click(object? sender, EventArgs e)
        => menuAccountUsers_Click(sender, e);

    private void btnNavLogout_Click(object? sender, EventArgs e) => PerformLogout();

    // ================================================================
    // Status bar
    // ================================================================

    private void timerClock_Tick(object? sender, EventArgs e) => UpdateClock();

    private void UpdateClock()
    {
        lblStatusTime.Text = $"🕒 {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
    }

    // ================================================================
    // Logout
    // ================================================================

    /// <summary>
    /// Đăng xuất: xác nhận với người dùng, sau đó đóng MainForm để quay về FrmLogin.
    /// </summary>
    private void PerformLogout()
    {
        var result = MessageBox.Show(
            "Bạn có chắc muốn đăng xuất khỏi hệ thống?",
            "Xác nhận đăng xuất",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result != DialogResult.Yes) return;

        ServiceLocator.Auth.Logout(CurrentSession.CurrentUser!.UserId);
        CurrentSession.SignOut();

        // DialogResult = OK → bật cờ cho FrmLogin biết là logout (chứ không phải close ứng dụng)
        // Tuy nhiên FrmLogin của ta được mở qua Application.Run nên chỉ cần Close()
        DialogResult = DialogResult.OK;
        Close();
    }
}
