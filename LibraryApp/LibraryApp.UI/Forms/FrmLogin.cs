using LibraryApp.BLL.Common;
using LibraryApp.BLL.Dtos;
using LibraryApp.DAL;
using LibraryApp.DAL.Common;
using LibraryApp.UI.Common;

namespace LibraryApp.UI.Forms;

/// <summary>
/// Form đăng nhập - điểm vào của ứng dụng.
/// <para>
/// Luồng xử lý:
/// </para>
/// <list type="number">
/// <item>Kiểm tra kết nối database khi form load. Nếu fail thì báo lỗi và đóng app.</item>
/// <item>Validate ô nhập (không rỗng) trước khi gọi service.</item>
/// <item>Gọi <c>AuthService.Login</c> trên background thread để không treo UI.</item>
/// <item>Nếu thành công: lưu user vào <see cref="CurrentSession"/>, ẩn form này và mở <c>FrmMain</c>.</item>
/// <item>Nếu thất bại: hiển thị MessageBox + ErrorProvider, focus lại field cần sửa.</item>
/// </list>
/// </summary>
public partial class FrmLogin : Form
{
    /// <summary>Khởi tạo form và các control.</summary>
    public FrmLogin()
    {
        InitializeComponent();
    }

    // ----------------------------------------------------------------
    // Form events
    // ----------------------------------------------------------------

    /// <summary>
    /// Khi form load: kiểm tra kết nối database trước khi cho user nhập.
    /// Tránh trường hợp user gõ xong mới phát hiện không có kết nối.
    /// </summary>
    private void FrmLogin_Load(object? sender, EventArgs e)
    {
        // Test connection ở background để không treo UI khi mạng chậm
        UseWaitCursor = true;
        Task.Run(DatabaseConnection.TestConnection).ContinueWith(task =>
        {
            UseWaitCursor = false;

            if (!task.Result)
            {
                MessageBox.Show(
                    "Không thể kết nối tới cơ sở dữ liệu.\n" +
                    "Vui lòng kiểm tra App.config và đảm bảo SQL Server đang chạy.",
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
                return;
            }

            txtUsername.Focus();
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    /// <summary>Toggle hiển thị/ẩn mật khẩu.</summary>
    private void chkShowPassword_CheckedChanged(object? sender, EventArgs e)
    {
        txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
    }

    /// <summary>Nút Thoát - đóng form đăng nhập (kết thúc ứng dụng).</summary>
    private void btnExit_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    /// <summary>
    /// Nút Đăng nhập - validate, gọi AuthService, xử lý kết quả.
    /// Dùng <c>async</c> để hash mật khẩu BCrypt trên background thread
    /// (BCrypt cost factor 11 mất ~100ms, đủ để gây giật UI nếu chạy đồng bộ).
    /// </summary>
    private async void btnLogin_Click(object? sender, EventArgs e)
    {
        errorProvider.Clear();

        // ---- 1. Validate đầu vào tại UI ----
        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            errorProvider.SetError(txtUsername, "Vui lòng nhập tên đăng nhập.");
            txtUsername.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            errorProvider.SetError(txtPassword, "Vui lòng nhập mật khẩu.");
            txtPassword.Focus();
            return;
        }

        // ---- 2. Gọi AuthService trên background thread ----
        SetBusy(true);
        try
        {
            var result = await Task.Run(() =>
                ServiceLocator.Auth.Login(
                    txtUsername.Text.Trim(),
                    txtPassword.Text,
                    ipAddress: GetLocalIpAddress()));

            HandleLoginResult(result);
        }
        catch (BusinessException ex)
        {
            MessageBox.Show(ex.Message, "Lỗi nhập liệu",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (DalException ex)
        {
            MessageBox.Show(ex.Message, "Lỗi truy cập dữ liệu",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            // Fallback cho mọi exception không lường trước
            MessageBox.Show(
                $"Đã xảy ra lỗi không xác định: {ex.Message}",
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    // ----------------------------------------------------------------
    // Helpers
    // ----------------------------------------------------------------

    /// <summary>
    /// Xử lý <see cref="LoginResult"/>: thành công thì mở MainForm,
    /// thất bại thì hiện MessageBox phù hợp.
    /// </summary>
    private void HandleLoginResult(LoginResult result)
    {
        if (!result.Success)
        {
            var icon = result.LockoutMinutesRemaining.HasValue
                ? MessageBoxIcon.Warning
                : MessageBoxIcon.Information;

            MessageBox.Show(result.Message, "Đăng nhập thất bại",
                MessageBoxButtons.OK, icon);

            txtPassword.Clear();
            txtPassword.Focus();
            return;
        }

        // ---- Đăng nhập thành công ----
        var user = result.User!;

        // Lấy mã vai trò để đặt vào ngữ cảnh ứng dụng
        // (chỉ cần RoleCode "ADMIN"/"LIBRARIAN"/"VIEWER", lookup nhanh từ DB)
        var roleCode = ResolveRoleCode(user.RoleId);
        CurrentSession.SignIn(user, roleCode);

        // Cảnh báo nếu cần đổi mật khẩu (nhưng vẫn cho phép vào)
        if (result.MustChangePassword)
        {
            MessageBox.Show(
                "Bạn cần đổi mật khẩu trong lần đăng nhập đầu tiên.",
                "Yêu cầu đổi mật khẩu",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            // TODO: mở FrmChangePassword trước khi mở MainForm khi có form đó
        }

        OpenMainForm();
    }

    /// <summary>
    /// Đóng FrmLogin với <see cref="DialogResult.OK"/> để báo cho
    /// <c>Program.Main</c> biết login thành công và tiếp tục mở MainForm.
    /// </summary>
    private void OpenMainForm()
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    /// <summary>
    /// Lấy mã vai trò ("ADMIN"/"LIBRARIAN"/...) theo RoleId.
    /// Thực hiện một truy vấn nhanh tới bảng Roles — kết quả được cache static
    /// vì danh sách Role rất nhỏ và ít thay đổi.
    /// </summary>
    private static readonly Dictionary<int, string> _roleCodeCache = new();
    private static readonly Lock _roleCacheLock = new();

    private static string ResolveRoleCode(int roleId)
    {
        lock (_roleCacheLock)
        {
            if (_roleCodeCache.TryGetValue(roleId, out var cached))
                return cached;
        }

        const string sql = "SELECT RoleCode FROM dbo.Roles WHERE RoleId = @Id;";
        try
        {
            using var conn = DatabaseConnection.OpenConnection();
            using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", roleId);
            var result = cmd.ExecuteScalar()?.ToString() ?? "UNKNOWN";

            lock (_roleCacheLock)
                _roleCodeCache[roleId] = result;

            return result;
        }
        catch
        {
            return "UNKNOWN";
        }
    }

    /// <summary>
    /// Lấy IP nội bộ của máy đang đăng nhập, để gửi vào audit log.
    /// </summary>
    private static string? GetLocalIpAddress()
    {
        try
        {
            var hostName = System.Net.Dns.GetHostName();
            var addresses = System.Net.Dns.GetHostAddresses(hostName);
            foreach (var ip in addresses)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return ip.ToString();
            }
        }
        catch
        {
            // Bỏ qua nếu không lấy được IP
        }
        return null;
    }

    /// <summary>Bật/tắt trạng thái "đang xử lý" trên UI.</summary>
    private void SetBusy(bool busy)
    {
        UseWaitCursor = busy;
        btnLogin.Enabled = !busy;
        btnExit.Enabled = !busy;
        txtUsername.Enabled = !busy;
        txtPassword.Enabled = !busy;
        chkShowPassword.Enabled = !busy;
        btnLogin.Text = busy ? "ĐANG ĐĂNG NHẬP..." : "ĐĂNG NHẬP";
    }
}
