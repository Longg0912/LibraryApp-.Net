using LibraryApp.UI.Forms;

namespace LibraryApp.UI;

/// <summary>
/// Entry point của ứng dụng. Khởi tạo Application context và mở
/// form đăng nhập đầu tiên.
/// </summary>
internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Bật visual styles theo chuẩn Windows hiện đại
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Global exception handlers (đảm bảo không crash silent)
        Application.ThreadException += (_, args) =>
            MessageBox.Show(
                $"Lỗi không xử lý được: {args.Exception.Message}",
                "Lỗi hệ thống",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
                MessageBox.Show(
                    $"Lỗi nghiêm trọng: {ex.Message}",
                    "Lỗi hệ thống",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        };

        // Vòng lặp Login → Main → (Logout → Login) → ... → Exit
        // Khi user đăng xuất, FrmMain.DialogResult = OK → quay lại FrmLogin.
        // Khi user đóng app từ Login hoặc đóng Main không qua Logout → thoát hẳn.
        while (true)
        {
            using var login = new FrmLogin();
            login.ShowDialog();

            if (!Common.CurrentSession.IsAuthenticated)
                break;   // user thoát từ FrmLogin

            using var main = new FrmMain();
            var mainResult = main.ShowDialog();

            // mainResult = OK nghĩa là user vừa Logout → quay lại Login
            // mainResult = Cancel hoặc bất kỳ giá trị nào khác → thoát app
            if (mainResult != DialogResult.OK)
                break;
        }
    }
}
