using System.Data;
using LibraryApp.Models;
using LibraryApp.UI.Common;

namespace LibraryApp.UI.UserControls;

/// <summary>
/// UserControl quản lý người dùng (chỉ Admin được sử dụng).
/// </summary>
/// <remarks>
/// Khác với CRUD độc giả/sách thông thường, quản lý user có 2 thao tác đặc thù:
/// <list type="bullet">
/// <item><b>Reset mật khẩu:</b> sinh mật khẩu tạm ngẫu nhiên, đặt
/// <c>MustChangePassword = 1</c> để user phải đổi khi đăng nhập kế tiếp.
/// Admin xem mật khẩu tạm trong dialog rồi thông báo cho user.</item>
/// <item><b>Khoá / mở khoá:</b> set <c>IsActive</c>. Khi khoá → user không
/// thể đăng nhập. Khi mở khoá → reset luôn <c>FailedLoginCount</c> và <c>LockoutUntil</c>.</item>
/// </list>
///
/// Bảo vệ: không cho phép admin tự xoá / khoá / reset chính tài khoản mình
/// để tránh trường hợp tự "khóa cửa" không vào được hệ thống.
/// </remarks>
public partial class UcUserList : UserControl
{
    private enum FormMode { Add, Edit }

    private FormMode _mode = FormMode.Add;
    private int _selectedUserId;
    private bool _suspendSelectionChanged;

    public UcUserList()
    {
        InitializeComponent();
        Load += UcUserList_Load;
    }

    // ================================================================
    // Lifecycle
    // ================================================================

    private void UcUserList_Load(object? sender, EventArgs e)
    {
        if (!CurrentSession.IsAdmin)
        {
            MessageBox.Show("Chức năng quản lý người dùng chỉ dành cho Quản trị viên.",
                "Không có quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        ErrorHandler.TryRun(() =>
        {
            BindRoleCombos();
            BindStatusCombo();
            SetMode(FormMode.Add);
            ReloadGrid();
        }, context: "Tải danh sách người dùng", owner: FindForm());
    }

    // ================================================================
    // ComboBox binding
    // ================================================================

    private void BindRoleCombos()
    {
        var roles = ServiceLocator.Users.GetAllRoles();

        // Filter combo: thêm "Tất cả"
        var filterItems = new List<Role>
        {
            new() { RoleId = 0, RoleCode = "", RoleName = "-- Tất cả --" }
        };
        filterItems.AddRange(roles);
        cboFilterRole.DataSource = filterItems;
        cboFilterRole.DisplayMember = nameof(Role.RoleName);
        cboFilterRole.ValueMember = nameof(Role.RoleId);
        cboFilterRole.SelectedIndex = 0;

        // Form combo: phải chọn 1 role cụ thể
        cboRole.DataSource = new List<Role>(roles);
        cboRole.DisplayMember = nameof(Role.RoleName);
        cboRole.ValueMember = nameof(Role.RoleId);
        if (roles.Count > 0) cboRole.SelectedIndex = 0;
    }

    private void BindStatusCombo()
    {
        var items = new List<StatusItem>
        {
            new(null,  "-- Tất cả --"),
            new(true,  "Đang hoạt động"),
            new(false, "Đã khoá")
        };
        cboFilterStatus.DataSource = items;
        cboFilterStatus.DisplayMember = nameof(StatusItem.Display);
        cboFilterStatus.ValueMember = nameof(StatusItem.Value);
        cboFilterStatus.SelectedIndex = 0;
    }

    private sealed record StatusItem(bool? Value, string Display);

    // ================================================================
    // Grid load + format
    // ================================================================

    private void ReloadGrid()
    {
        ErrorHandler.TryRun(() =>
        {
            UseWaitCursor = true;

            string? keyword = string.IsNullOrWhiteSpace(txtSearchKeyword.Text) ? null : txtSearchKeyword.Text.Trim();
            int? roleId = cboFilterRole.SelectedValue is int v && v > 0 ? v : null;
            bool? active = (cboFilterStatus.SelectedItem as StatusItem)?.Value;

            var table = ServiceLocator.Users.SearchAsDataTable(keyword, roleId, active);

            _suspendSelectionChanged = true;
            dgvUsers.DataSource = table;
            dgvUsers.ClearSelection();
            _suspendSelectionChanged = false;
        }, context: "Tải danh sách người dùng", owner: FindForm());

        UseWaitCursor = false;
    }

    /// <summary>Format cột IsActive sang "✓ Có" / "✗ Đã khoá".</summary>
    private void dgvUsers_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        var col = dgvUsers.Columns[e.ColumnIndex];
        if (col.Name == "IsActive" && e.Value is bool active)
        {
            e.Value = active ? "✓ Hoạt động" : "✗ Đã khoá";
            e.FormattingApplied = true;

            // Tô đỏ dòng đã khoá
            if (!active)
                dgvUsers.Rows[e.RowIndex].DefaultCellStyle.ForeColor =
                    System.Drawing.Color.FromArgb(190, 60, 60);
        }
    }

    private void dgvUsers_SelectionChanged(object? sender, EventArgs e)
    {
        if (_suspendSelectionChanged) return;

        if (dgvUsers.CurrentRow is null)
        {
            SetMode(FormMode.Add);
            return;
        }

        ErrorHandler.TryRun(() =>
        {
            int userId = Convert.ToInt32(dgvUsers.CurrentRow.Cells["UserId"].Value);
            var user = ServiceLocator.Users.GetById(userId);
            if (user is null)
            {
                SetMode(FormMode.Add);
                return;
            }

            txtUsername.Text = user.Username;
            txtFullName.Text = user.FullName;
            txtEmail.Text = user.Email ?? "";
            txtPhone.Text = user.Phone ?? "";
            cboRole.SelectedValue = user.RoleId;
            chkIsActive.Checked = user.IsActive;
            txtTempPwd.Clear();

            // Meta info
            var meta = new System.Text.StringBuilder();
            meta.AppendLine($"Tạo lúc: {user.CreatedAt.ToLocalTime():dd/MM/yyyy HH:mm}");
            if (user.LastLoginAt.HasValue)
                meta.AppendLine($"Đăng nhập cuối: {user.LastLoginAt.Value.ToLocalTime():dd/MM/yyyy HH:mm}");
            if (user.IsCurrentlyLockedOut)
                meta.AppendLine($"⚠ Đang bị khoá đến: {user.LockoutUntil!.Value.ToLocalTime():HH:mm}");
            if (user.MustChangePassword)
                meta.AppendLine("⚠ Phải đổi mật khẩu khi đăng nhập");
            lblMeta.Text = meta.ToString().TrimEnd();

            _selectedUserId = userId;
            SetMode(FormMode.Edit);
        }, owner: FindForm());
    }

    // ================================================================
    // Form mode
    // ================================================================

    private void SetMode(FormMode mode)
    {
        _mode = mode;
        errorProvider.Clear();

        if (mode == FormMode.Add)
        {
            _selectedUserId = 0;
            ClearForm();
            txtUsername.ReadOnly = false;
            txtUsername.BackColor = System.Drawing.Color.White;
            txtTempPwd.Visible = true;
            lblTempPwd.Visible = true;
            lblMeta.Visible = false;

            btnSave.Visible = true;
            btnUpdate.Visible = false;
            btnResetPwd.Visible = false;
            btnToggleActive.Visible = false;
            btnDelete.Visible = false;
            lblFormHeader.Text = "Thêm người dùng mới";
        }
        else
        {
            txtUsername.ReadOnly = true;
            txtUsername.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            txtTempPwd.Clear();
            txtTempPwd.Visible = false;
            lblTempPwd.Visible = false;
            lblMeta.Visible = true;

            btnSave.Visible = false;
            btnUpdate.Visible = true;
            btnResetPwd.Visible = true;
            btnToggleActive.Visible = true;
            btnDelete.Visible = true;
            btnToggleActive.Text = chkIsActive.Checked ? "🔒  Khoá tài khoản" : "🔓  Mở khoá";
            lblFormHeader.Text = $"Sửa: {txtUsername.Text}";
        }
    }

    private void ClearForm()
    {
        txtUsername.Clear();
        txtFullName.Clear();
        txtEmail.Clear();
        txtPhone.Clear();
        txtTempPwd.Clear();
        chkIsActive.Checked = true;
        if (cboRole.Items.Count > 0) cboRole.SelectedIndex = 0;
        lblMeta.Text = "";
    }

    // ================================================================
    // Button handlers
    // ================================================================

    private void btnNew_Click(object? sender, EventArgs e)
    {
        _suspendSelectionChanged = true;
        dgvUsers.ClearSelection();
        _suspendSelectionChanged = false;
        SetMode(FormMode.Add);
        txtUsername.Focus();
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        // Validate
        var v = new UiValidator(errorProvider);
        v.RequireText(txtUsername, "Tên đăng nhập", minLength: 3, maxLength: 50)
         .Custom(txtUsername,
            check: () => txtUsername.Text.Trim().Contains(' '),
            message: "Tên đăng nhập không được chứa khoảng trắng.")
         .RequireText(txtFullName, "Họ tên", minLength: 2, maxLength: 100)
         .ValidateEmail(txtEmail, allowEmpty: true)
         .ValidatePhone(txtPhone, allowEmpty: true)
         .RequireSelection(cboRole, "Vai trò")
         .RequireText(txtTempPwd, "Mật khẩu tạm", minLength: 6)
         .Custom(txtTempPwd,
            check: () => !txtTempPwd.Text.Any(char.IsLetter) || !txtTempPwd.Text.Any(char.IsDigit),
            message: "Mật khẩu tạm phải có cả chữ và số.");

        if (!v.IsValid) { v.FocusFirstError(); return; }

        bool success = ErrorHandler.TryRun(() =>
        {
            int newId = ServiceLocator.Users.Create(
                username: txtUsername.Text.Trim(),
                fullName: txtFullName.Text.Trim(),
                email: string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                phone: string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                roleId: (int)cboRole.SelectedValue!,
                tempPassword: txtTempPwd.Text);

            Logger.Info($"Admin {CurrentSession.CurrentUser?.Username} created user " +
                       $"'{txtUsername.Text}' (UserId={newId})");

            MessageBox.Show(
                $"✓ Đã tạo người dùng '{txtUsername.Text}' (ID: {newId}).\n\n" +
                $"Mật khẩu tạm: {txtTempPwd.Text}\n\n" +
                "User sẽ phải đổi mật khẩu khi đăng nhập lần đầu.",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }, context: "Tạo người dùng mới", owner: FindForm());

        if (success)
        {
            ReloadGrid();
            SetMode(FormMode.Add);
        }
    }

    private void btnUpdate_Click(object? sender, EventArgs e)
    {
        if (_selectedUserId <= 0)
        {
            MessageBox.Show("Vui lòng chọn người dùng cần sửa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var v = new UiValidator(errorProvider);
        v.RequireText(txtFullName, "Họ tên", minLength: 2, maxLength: 100)
         .ValidateEmail(txtEmail, allowEmpty: true)
         .ValidatePhone(txtPhone, allowEmpty: true)
         .RequireSelection(cboRole, "Vai trò");

        if (!v.IsValid) { v.FocusFirstError(); return; }

        bool success = ErrorHandler.TryRun(() =>
        {
            ServiceLocator.Users.Update(
                userId: _selectedUserId,
                fullName: txtFullName.Text.Trim(),
                email: string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                phone: string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                roleId: (int)cboRole.SelectedValue!);

            Logger.Info($"Admin {CurrentSession.CurrentUser?.Username} updated user UserId={_selectedUserId}");

            MessageBox.Show($"✓ Đã cập nhật '{txtUsername.Text}'.",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }, context: "Cập nhật người dùng", owner: FindForm());

        if (success)
        {
            ReloadGrid();
            SetMode(FormMode.Add);
        }
    }

    /// <summary>Reset mật khẩu - sinh tạm + hiển thị cho admin.</summary>
    private void btnResetPwd_Click(object? sender, EventArgs e)
    {
        if (_selectedUserId <= 0) return;

        if (_selectedUserId == CurrentSession.CurrentUser?.UserId)
        {
            MessageBox.Show(
                "Không thể reset mật khẩu của chính mình.\n\n" +
                "Vui lòng dùng menu 'Tài khoản → Đổi mật khẩu' để đổi mật khẩu cho tài khoản đang đăng nhập.",
                "Thao tác không hợp lệ",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            $"Reset mật khẩu cho '{txtUsername.Text}'?\n\n" +
            "Hệ thống sẽ sinh mật khẩu tạm và user phải đổi khi đăng nhập kế tiếp.",
            "Xác nhận reset mật khẩu",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        ErrorHandler.TryRun(() =>
        {
            string tempPwd = ServiceLocator.Users.ResetPassword(_selectedUserId);

            Logger.Warning(
                $"Admin {CurrentSession.CurrentUser?.Username} reset password for UserId={_selectedUserId}");

            // Hiển thị mật khẩu tạm trong dialog có thể copy
            ShowTempPasswordDialog(txtUsername.Text, tempPwd);
        }, context: "Reset mật khẩu", owner: FindForm());
    }

    /// <summary>Khoá / mở khoá tài khoản.</summary>
    private void btnToggleActive_Click(object? sender, EventArgs e)
    {
        if (_selectedUserId <= 0) return;

        if (_selectedUserId == CurrentSession.CurrentUser?.UserId)
        {
            MessageBox.Show(
                "Không thể khoá/mở khoá tài khoản của chính mình.",
                "Thao tác không hợp lệ",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        bool willActive = !chkIsActive.Checked;
        string action = willActive ? "mở khoá" : "khoá";

        var confirm = MessageBox.Show(
            $"Bạn có chắc muốn {action} tài khoản '{txtUsername.Text}'?",
            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        bool success = ErrorHandler.TryRun(() =>
        {
            ServiceLocator.Users.SetActive(_selectedUserId, willActive);

            Logger.Warning(
                $"Admin {CurrentSession.CurrentUser?.Username} {(willActive ? "unlocked" : "locked")} " +
                $"UserId={_selectedUserId}");

            MessageBox.Show($"✓ Đã {action} tài khoản '{txtUsername.Text}'.",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }, context: action + " tài khoản", owner: FindForm());

        if (success)
        {
            ReloadGrid();
            SetMode(FormMode.Add);
        }
    }

    private void btnDelete_Click(object? sender, EventArgs e)
    {
        if (_selectedUserId <= 0) return;

        if (_selectedUserId == CurrentSession.CurrentUser?.UserId)
        {
            MessageBox.Show("Không thể xoá tài khoản của chính mình.",
                "Thao tác không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            $"Bạn có chắc muốn xoá người dùng '{txtUsername.Text}'?\n\n" +
            "Lưu ý: thao tác không thể hoàn tác từ UI. " +
            "Nếu user đang có phiếu mượn mở, hệ thống sẽ từ chối xoá.",
            "Xác nhận xoá", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes) return;

        bool success = ErrorHandler.TryRun(() =>
        {
            ServiceLocator.Users.Delete(_selectedUserId);
            Logger.Warning($"Admin {CurrentSession.CurrentUser?.Username} deleted UserId={_selectedUserId}");

            MessageBox.Show("Đã xoá người dùng khỏi hệ thống.",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }, context: "Xoá người dùng", owner: FindForm());

        if (success)
        {
            ReloadGrid();
            SetMode(FormMode.Add);
        }
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        _suspendSelectionChanged = true;
        dgvUsers.ClearSelection();
        _suspendSelectionChanged = false;
        SetMode(FormMode.Add);
    }

    private void btnSearch_Click(object? sender, EventArgs e) => ReloadGrid();

    private void btnReload_Click(object? sender, EventArgs e)
    {
        txtSearchKeyword.Clear();
        if (cboFilterRole.Items.Count > 0) cboFilterRole.SelectedIndex = 0;
        if (cboFilterStatus.Items.Count > 0) cboFilterStatus.SelectedIndex = 0;
        ReloadGrid();
    }

    private void txtSearchKeyword_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            ReloadGrid();
        }
    }

    // ================================================================
    // Helper - hiện mật khẩu tạm trong dialog có thể copy
    // ================================================================

    private static void ShowTempPasswordDialog(string username, string tempPassword)
    {
        using var dlg = new Form
        {
            Text = "Mật khẩu tạm đã được tạo",
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            ClientSize = new System.Drawing.Size(420, 240),
            BackColor = System.Drawing.Color.White
        };

        var lblHeader = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Top,
            BackColor = System.Drawing.Color.FromArgb(254, 252, 232),
            Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold),
            ForeColor = System.Drawing.Color.FromArgb(146, 64, 14),
            Height = 50,
            Padding = new Padding(20, 0, 20, 0),
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            Text = $"🔑 Mật khẩu tạm cho '{username}'"
        };

        var lblHint = new Label
        {
            AutoSize = false,
            Font = new System.Drawing.Font("Segoe UI", 9F),
            ForeColor = System.Drawing.Color.FromArgb(80, 80, 80),
            Location = new System.Drawing.Point(20, 70),
            Size = new System.Drawing.Size(380, 50),
            Text = "Vui lòng sao chép mật khẩu sau và gửi cho user qua kênh an toàn. " +
                   "User sẽ phải đổi mật khẩu khi đăng nhập lần kế tiếp."
        };

        var txtPwd = new TextBox
        {
            BorderStyle = BorderStyle.FixedSingle,
            Font = new System.Drawing.Font("Consolas", 13F, System.Drawing.FontStyle.Bold),
            Location = new System.Drawing.Point(20, 130),
            Size = new System.Drawing.Size(280, 30),
            Text = tempPassword,
            ReadOnly = true,
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251),
            ForeColor = System.Drawing.Color.FromArgb(33, 64, 154),
            TextAlign = HorizontalAlignment.Center
        };

        var btnCopy = new Button
        {
            BackColor = System.Drawing.Color.FromArgb(33, 64, 154),
            FlatStyle = FlatStyle.Flat,
            Font = new System.Drawing.Font("Segoe UI Semibold", 9F),
            ForeColor = System.Drawing.Color.White,
            Location = new System.Drawing.Point(310, 130),
            Size = new System.Drawing.Size(90, 30),
            Text = "📋 Copy",
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        btnCopy.FlatAppearance.BorderSize = 0;
        btnCopy.Click += (_, _) =>
        {
            try
            {
                Clipboard.SetText(tempPassword);
                btnCopy.Text = "✓ Đã copy";
            }
            catch { /* ignore clipboard errors */ }
        };

        var btnOk = new Button
        {
            BackColor = System.Drawing.Color.FromArgb(16, 185, 129),
            FlatStyle = FlatStyle.Flat,
            Font = new System.Drawing.Font("Segoe UI Semibold", 10F),
            ForeColor = System.Drawing.Color.White,
            Location = new System.Drawing.Point(160, 185),
            Size = new System.Drawing.Size(120, 36),
            Text = "✓ Đã ghi nhớ",
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false,
            DialogResult = DialogResult.OK
        };
        btnOk.FlatAppearance.BorderSize = 0;

        dlg.Controls.AddRange(new Control[] { btnOk, btnCopy, txtPwd, lblHint, lblHeader });
        dlg.AcceptButton = btnOk;
        dlg.ShowDialog();
    }
}
