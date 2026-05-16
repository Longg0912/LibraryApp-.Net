using LibraryApp.UI.Common;

namespace LibraryApp.UI.Forms;

/// <summary>
/// Dialog đổi mật khẩu cho user đang đăng nhập. Gọi từ menu
/// <c>Tài khoản → Đổi mật khẩu</c> trong FrmMain.
/// </summary>
/// <remarks>
/// Validate đầy đủ:
/// <list type="bullet">
/// <item>Mật khẩu hiện tại không rỗng.</item>
/// <item>Mật khẩu mới &gt;= 6 ký tự, có chữ + số.</item>
/// <item>Xác nhận = mật khẩu mới.</item>
/// <item>Mật khẩu mới khác mật khẩu cũ.</item>
/// </list>
///
/// Logic verify mật khẩu cũ + hash mật khẩu mới được xử lý ở
/// <c>AuthService.ChangePassword</c> — dùng BCrypt cost factor 11.
/// </remarks>
public partial class FrmChangePassword : Form
{
    public FrmChangePassword()
    {
        InitializeComponent();
    }

    /// <summary>Toggle hiển thị/ẩn mật khẩu cho cả 3 ô.</summary>
    private void chkShowPassword_CheckedChanged(object? sender, EventArgs e)
    {
        char passwordChar = chkShowPassword.Checked ? '\0' : '●';
        txtOldPassword.PasswordChar = passwordChar;
        txtNewPassword.PasswordChar = passwordChar;
        txtConfirm.PasswordChar = passwordChar;
    }

    /// <summary>
    /// Validate đầu vào dùng <see cref="UiValidator"/>, sau đó gọi
    /// <c>AuthService.ChangePassword</c>. Mọi exception đi qua <see cref="ErrorHandler"/>.
    /// </summary>
    private void btnSave_Click(object? sender, EventArgs e)
    {
        if (!CurrentSession.IsAuthenticated)
        {
            MessageBox.Show("Bạn chưa đăng nhập.", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        // ---- 1. Validate bằng UiValidator ----
        var v = new UiValidator(errorProvider);
        v.RequireText(txtOldPassword, "Mật khẩu hiện tại")
         .RequireText(txtNewPassword, "Mật khẩu mới", minLength: 6)
         .RequireText(txtConfirm, "Xác nhận mật khẩu", minLength: 6)

         // Mật khẩu mới phải có chữ + số
         .Custom(txtNewPassword,
            check: () => !txtNewPassword.Text.Any(char.IsLetter)
                      || !txtNewPassword.Text.Any(char.IsDigit),
            message: "Mật khẩu mới phải chứa cả chữ cái và chữ số.")

         // Xác nhận phải khớp
         .Custom(txtConfirm,
            check: () => txtConfirm.Text != txtNewPassword.Text,
            message: "Mật khẩu xác nhận không khớp với mật khẩu mới.")

         // Mới khác cũ
         .Custom(txtNewPassword,
            check: () => txtOldPassword.Text == txtNewPassword.Text
                      && !string.IsNullOrEmpty(txtNewPassword.Text),
            message: "Mật khẩu mới phải khác mật khẩu hiện tại.");

        if (!v.IsValid)
        {
            v.FocusFirstError();
            return;
        }

        // ---- 2. Gọi AuthService qua ErrorHandler.TryRun ----
        UseWaitCursor = true;
        btnSave.Enabled = false;
        try
        {
            bool success = ErrorHandler.TryRun(() =>
            {
                ServiceLocator.Auth.ChangePassword(
                    userId: CurrentSession.CurrentUser!.UserId,
                    oldPassword: txtOldPassword.Text,
                    newPassword: txtNewPassword.Text);
            }, context: "Đổi mật khẩu", owner: this);

            if (!success) return;

            Logger.Info($"User {CurrentSession.CurrentUser!.Username} đã đổi mật khẩu thành công.");

            MessageBox.Show(
                "✓ Đã đổi mật khẩu thành công.\n\n" +
                "Vui lòng nhớ mật khẩu mới cho các lần đăng nhập sau.",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        finally
        {
            UseWaitCursor = false;
            btnSave.Enabled = true;
        }
    }
}
