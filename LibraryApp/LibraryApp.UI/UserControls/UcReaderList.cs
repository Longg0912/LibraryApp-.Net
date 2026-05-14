using System.Text.RegularExpressions;
using LibraryApp.BLL.Common;
using LibraryApp.Models;
using LibraryApp.Models.Enums;
using LibraryApp.UI.Common;

namespace LibraryApp.UI.UserControls;

/// <summary>
/// UserControl quản lý độc giả: tìm kiếm + CRUD đầy đủ + chức năng riêng "Gia hạn thẻ".
/// Đồng bộ về layout và pattern với <see cref="UcBookList"/>, <see cref="UcCategoryList"/>.
/// </summary>
/// <remarks>
/// Đặc thù của form độc giả so với 2 form trước:
/// <list type="bullet">
/// <item>Có nhiều field hơn (10 field, gồm 2 DateTimePicker + 2 ComboBox).</item>
/// <item>Có validate email và số điện thoại bằng regex (ngay tại UI để báo lỗi sớm).</item>
/// <item>Có nút "🔄 Gia hạn thẻ" gọi <c>ReaderService.RenewCard</c> — tách khỏi luồng Update
/// để có UX rõ ràng và logic riêng (giới hạn 3 năm/lần, mở khoá thẻ nếu đang Expired).</item>
/// <item>Khi xoá: nếu độc giả còn sách chưa trả, BLL ném BusinessException (lỗi
/// <c>50101</c> từ stored procedure).</item>
/// </list>
/// </remarks>
public partial class UcReaderList : UserControl
{
    private enum FormMode { Add, Edit }

    private FormMode _mode = FormMode.Add;
    private int _selectedReaderId;
    private bool _suspendSelectionChanged;

    // Regex validate phía UI (regex chuẩn cũng được dùng trong BLL Validator)
    private static readonly Regex _emailRegex = new(
        @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
        RegexOptions.Compiled);
    private static readonly Regex _phoneRegex = new(
        @"^[0-9+]{8,15}$",
        RegexOptions.Compiled);

    public UcReaderList()
    {
        InitializeComponent();
        Load += UcReaderList_Load;
    }

    // ================================================================
    // Lifecycle
    // ================================================================

    private void UcReaderList_Load(object? sender, EventArgs e)
    {
        try
        {
            BindGenderCombo();
            BindStatusCombos();
            SetMode(FormMode.Add);
            ReloadGrid();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    // ================================================================
    // Binding ComboBox
    // ================================================================

    private void BindGenderCombo()
    {
        var items = new List<GenderItem>
        {
            new(null, "-- Không xác định --"),
            new(Gender.Male,   "Nam"),
            new(Gender.Female, "Nữ"),
            new(Gender.Other,  "Khác")
        };
        cboGender.DataSource = items;
        cboGender.DisplayMember = nameof(GenderItem.Display);
        cboGender.ValueMember = nameof(GenderItem.Value);
        cboGender.SelectedIndex = 0;
    }

    private void BindStatusCombos()
    {
        // Filter combobox: thêm "Tất cả"
        var filterItems = new List<StatusItem>
        {
            new(null, "-- Tất cả --"),
            new(ReaderStatus.Active,  "Đang hoạt động"),
            new(ReaderStatus.Locked,  "Đã bị khoá"),
            new(ReaderStatus.Expired, "Hết hạn thẻ")
        };
        cboFilterStatus.DataSource = filterItems;
        cboFilterStatus.DisplayMember = nameof(StatusItem.Display);
        cboFilterStatus.ValueMember = nameof(StatusItem.Value);
        cboFilterStatus.SelectedIndex = 0;

        // Form combobox: liệt kê tất cả enum
        var statusItems = new List<StatusItem>
        {
            new(ReaderStatus.Active,  "Đang hoạt động"),
            new(ReaderStatus.Locked,  "Đã bị khoá"),
            new(ReaderStatus.Expired, "Hết hạn thẻ")
        };
        cboStatus.DataSource = statusItems;
        cboStatus.DisplayMember = nameof(StatusItem.Display);
        cboStatus.ValueMember = nameof(StatusItem.Value);
        cboStatus.SelectedIndex = 0;
    }

    private sealed record GenderItem(Gender? Value, string Display);
    private sealed record StatusItem(ReaderStatus? Value, string Display);

    private static string GetStatusDisplayName(ReaderStatus s) => s switch
    {
        ReaderStatus.Active => "Đang hoạt động",
        ReaderStatus.Locked => "Đã bị khoá",
        ReaderStatus.Expired => "Hết hạn thẻ",
        _ => s.ToString()
    };

    // ================================================================
    // Grid load
    // ================================================================

    private void ReloadGrid()
    {
        try
        {
            UseWaitCursor = true;
            string? keyword = string.IsNullOrWhiteSpace(txtSearchKeyword.Text) ? null : txtSearchKeyword.Text.Trim();
            ReaderStatus? status = (cboFilterStatus.SelectedItem as StatusItem)?.Value;

            var table = ServiceLocator.Readers.SearchAsDataTable(keyword, status);

            _suspendSelectionChanged = true;
            dgvReaders.DataSource = table;
            dgvReaders.ClearSelection();
            _suspendSelectionChanged = false;
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            UseWaitCursor = false;
        }
    }

    private void dgvReaders_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (dgvReaders.Columns[e.ColumnIndex].Name == "Status"
            && e.Value is string s && Enum.TryParse<ReaderStatus>(s, out var status))
        {
            e.Value = GetStatusDisplayName(status);
            e.FormattingApplied = true;
        }
    }

    private void dgvReaders_SelectionChanged(object? sender, EventArgs e)
    {
        if (_suspendSelectionChanged) return;

        if (dgvReaders.CurrentRow is null)
        {
            SetMode(FormMode.Add);
            return;
        }

        try
        {
            int readerId = Convert.ToInt32(dgvReaders.CurrentRow.Cells["ReaderId"].Value);
            var reader = ServiceLocator.Readers.GetById(readerId);
            if (reader is null)
            {
                SetMode(FormMode.Add);
                return;
            }

            // Đổ data lên form
            txtCard.Text = reader.CardNumber;
            txtFullName.Text = reader.FullName;
            dtpDob.Value = reader.DateOfBirth.HasValue
                                 ? reader.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue)
                                 : new DateTime(2000, 1, 1);
            SelectGender(reader.Gender);
            txtAddress.Text = reader.Address ?? string.Empty;
            txtPhone.Text = reader.Phone ?? string.Empty;
            txtEmail.Text = reader.Email ?? string.Empty;
            dtpIssueDate.Value = reader.CardIssueDate.ToDateTime(TimeOnly.MinValue);
            dtpExpireDate.Value = reader.CardExpireDate.ToDateTime(TimeOnly.MinValue);
            SelectStatus(reader.Status);

            _selectedReaderId = reader.ReaderId;
            SetMode(FormMode.Edit);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void SelectGender(Gender? gender)
    {
        for (int i = 0; i < cboGender.Items.Count; i++)
        {
            if (cboGender.Items[i] is GenderItem item && item.Value == gender)
            {
                cboGender.SelectedIndex = i;
                return;
            }
        }
        cboGender.SelectedIndex = 0;
    }

    private void SelectStatus(ReaderStatus status)
    {
        for (int i = 0; i < cboStatus.Items.Count; i++)
        {
            if (cboStatus.Items[i] is StatusItem item && item.Value == status)
            {
                cboStatus.SelectedIndex = i;
                return;
            }
        }
        cboStatus.SelectedIndex = 0;
    }

    // ================================================================
    // Mode handling
    // ================================================================

    private void SetMode(FormMode mode)
    {
        _mode = mode;
        errorProvider.Clear();

        if (mode == FormMode.Add)
        {
            _selectedReaderId = 0;
            ClearFormInputs();
            txtCard.ReadOnly = false;
            txtCard.BackColor = System.Drawing.Color.White;

            btnSave.Visible = true;
            btnUpdate.Visible = false;
            btnRenew.Visible = false;
            btnDelete.Visible = false;
            lblFormHeader.Text = "Thêm độc giả mới";
        }
        else
        {
            txtCard.ReadOnly = true;
            txtCard.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);

            btnSave.Visible = false;
            btnUpdate.Visible = true;
            btnRenew.Visible = true;
            btnDelete.Visible = true;
            lblFormHeader.Text = $"Sửa: {txtCard.Text}";
        }
    }

    private void ClearFormInputs()
    {
        txtCard.Clear();
        txtFullName.Clear();
        dtpDob.Value = new DateTime(2000, 1, 1);
        cboGender.SelectedIndex = 0;
        txtAddress.Clear();
        txtPhone.Clear();
        txtEmail.Clear();
        dtpIssueDate.Value = DateTime.Today;
        dtpExpireDate.Value = DateTime.Today.AddYears(2);
        cboStatus.SelectedIndex = 0;
    }

    // ================================================================
    // Button handlers
    // ================================================================

    private void btnNew_Click(object? sender, EventArgs e)
    {
        _suspendSelectionChanged = true;
        dgvReaders.ClearSelection();
        _suspendSelectionChanged = false;
        SetMode(FormMode.Add);
        txtCard.Focus();
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        if (!ValidateForm()) return;

        try
        {
            var reader = BuildReaderFromForm();
            reader.CreatedBy = CurrentSession.CurrentUser?.UserId;

            int newId = ServiceLocator.Readers.Create(reader);
            MessageBox.Show($"Đã thêm độc giả '{reader.FullName}' (Số thẻ: {reader.CardNumber}).",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ReloadGrid();
            SetMode(FormMode.Add);
        }
        catch (BusinessException ex)
        {
            HighlightField(ex.FieldName);
            MessageBox.Show(ex.Message, "Dữ liệu chưa hợp lệ",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void btnUpdate_Click(object? sender, EventArgs e)
    {
        if (_selectedReaderId <= 0)
        {
            MessageBox.Show("Vui lòng chọn độc giả cần sửa.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!ValidateForm(forUpdate: true)) return;

        try
        {
            var reader = BuildReaderFromForm();
            reader.ReaderId = _selectedReaderId;
            reader.CardNumber = txtCard.Text.Trim();  // giữ nguyên số thẻ
            reader.UpdatedBy = CurrentSession.CurrentUser?.UserId;

            ServiceLocator.Readers.Update(reader);
            MessageBox.Show($"Đã cập nhật thông tin của '{reader.FullName}'.",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ReloadGrid();
            SetMode(FormMode.Add);
        }
        catch (BusinessException ex)
        {
            HighlightField(ex.FieldName);
            MessageBox.Show(ex.Message, "Lỗi nghiệp vụ",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    /// <summary>
    /// Gia hạn thẻ: mở một dialog nhỏ cho user chọn ngày hết hạn mới
    /// (hoặc dùng ngày trong dtpExpireDate hiện tại). Gọi ReaderService.RenewCard
    /// để xử lý nghiệp vụ (giới hạn 3 năm/lần, mở khoá thẻ Expired...).
    /// </summary>
    private void btnRenew_Click(object? sender, EventArgs e)
    {
        if (_selectedReaderId <= 0)
        {
            MessageBox.Show("Vui lòng chọn độc giả cần gia hạn thẻ.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var newExpire = DateOnly.FromDateTime(dtpExpireDate.Value);
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (newExpire <= today)
        {
            MessageBox.Show("Ngày hết hạn mới phải sau hôm nay. Vui lòng chọn ngày trong tương lai.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            dtpExpireDate.Focus();
            return;
        }

        var confirm = MessageBox.Show(
            $"Gia hạn thẻ của '{txtFullName.Text}' đến ngày {newExpire:dd/MM/yyyy}?\n\n" +
            "Nếu thẻ đang ở trạng thái 'Hết hạn', hệ thống sẽ tự kích hoạt lại.",
            "Xác nhận gia hạn",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        try
        {
            ServiceLocator.Readers.RenewCard(_selectedReaderId, newExpire);
            MessageBox.Show($"Đã gia hạn thẻ đến {newExpire:dd/MM/yyyy}.",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ReloadGrid();
            SetMode(FormMode.Add);
        }
        catch (BusinessException ex)
        {
            MessageBox.Show(ex.Message, "Không thể gia hạn",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void btnDelete_Click(object? sender, EventArgs e)
    {
        if (_selectedReaderId <= 0)
        {
            MessageBox.Show("Vui lòng chọn độc giả cần xoá.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show(
            $"Bạn có chắc muốn xoá độc giả '{txtFullName.Text}' (số thẻ: {txtCard.Text})?\n\n" +
            "Lưu ý: độc giả đang có sách chưa trả sẽ không thể xoá.",
            "Xác nhận xoá",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes) return;

        try
        {
            ServiceLocator.Readers.Delete(_selectedReaderId);
            MessageBox.Show("Đã xoá độc giả khỏi hệ thống.",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ReloadGrid();
            SetMode(FormMode.Add);
        }
        catch (BusinessException ex)
        {
            // Bao gồm lỗi 50101 từ stored procedure (còn sách chưa trả)
            MessageBox.Show(ex.Message, "Không thể xoá",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        _suspendSelectionChanged = true;
        dgvReaders.ClearSelection();
        _suspendSelectionChanged = false;
        SetMode(FormMode.Add);
    }

    private void btnSearch_Click(object? sender, EventArgs e) => ReloadGrid();

    private void btnReload_Click(object? sender, EventArgs e)
    {
        txtSearchKeyword.Clear();
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
    // Validate
    // ================================================================

    /// <summary>
    /// Validate đầy đủ trên UI, bao gồm format email và số điện thoại.
    /// </summary>
    private bool ValidateForm(bool forUpdate = false)
    {
        errorProvider.Clear();

        // 1. Số thẻ (chỉ check khi thêm mới)
        if (!forUpdate)
        {
            if (string.IsNullOrWhiteSpace(txtCard.Text))
            {
                errorProvider.SetError(txtCard, "Số thẻ không được để trống.");
                txtCard.Focus();
                return false;
            }
            if (txtCard.Text.Trim().Length < 2)
            {
                errorProvider.SetError(txtCard, "Số thẻ phải có ít nhất 2 ký tự.");
                txtCard.Focus();
                return false;
            }
        }

        // 2. Họ tên
        if (string.IsNullOrWhiteSpace(txtFullName.Text))
        {
            errorProvider.SetError(txtFullName, "Họ tên không được để trống.");
            txtFullName.Focus();
            return false;
        }

        // 3. Ngày sinh không sau hôm nay
        if (dtpDob.Value.Date > DateTime.Today)
        {
            errorProvider.SetError(dtpDob, "Ngày sinh không được sau hôm nay.");
            dtpDob.Focus();
            return false;
        }

        // 4. Số điện thoại (nếu có nhập)
        if (!string.IsNullOrWhiteSpace(txtPhone.Text)
            && !_phoneRegex.IsMatch(txtPhone.Text.Trim()))
        {
            errorProvider.SetError(txtPhone,
                "Số điện thoại chỉ được chứa chữ số và dấu +, dài 8-15 ký tự.");
            txtPhone.Focus();
            return false;
        }

        // 5. Email (nếu có nhập)
        if (!string.IsNullOrWhiteSpace(txtEmail.Text)
            && !_emailRegex.IsMatch(txtEmail.Text.Trim()))
        {
            errorProvider.SetError(txtEmail, "Email không đúng định dạng (vd: name@domain.com).");
            txtEmail.Focus();
            return false;
        }

        // 6. Ngày hết hạn phải sau ngày cấp
        if (dtpExpireDate.Value.Date <= dtpIssueDate.Value.Date)
        {
            errorProvider.SetError(dtpExpireDate, "Ngày hết hạn phải sau ngày cấp thẻ.");
            dtpExpireDate.Focus();
            return false;
        }

        // 7. Thời hạn thẻ tối đa 5 năm
        if ((dtpExpireDate.Value.Date - dtpIssueDate.Value.Date).TotalDays > 365 * 5)
        {
            errorProvider.SetError(dtpExpireDate, "Thời hạn thẻ tối đa 5 năm tính từ ngày cấp.");
            dtpExpireDate.Focus();
            return false;
        }

        return true;
    }

    private Reader BuildReaderFromForm()
    {
        var genderItem = (GenderItem)cboGender.SelectedItem!;
        var statusItem = (StatusItem)cboStatus.SelectedItem!;

        return new Reader
        {
            CardNumber = txtCard.Text.Trim(),
            FullName = txtFullName.Text.Trim(),
            DateOfBirth = DateOnly.FromDateTime(dtpDob.Value),
            Gender = genderItem.Value,
            Address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim(),
            Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
            Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
            CardIssueDate = DateOnly.FromDateTime(dtpIssueDate.Value),
            CardExpireDate = DateOnly.FromDateTime(dtpExpireDate.Value),
            Status = statusItem.Value ?? ReaderStatus.Active
        };
    }

    private void HighlightField(string? fieldName)
    {
        if (string.IsNullOrEmpty(fieldName)) return;

        Control? ctrl = fieldName switch
        {
            "Số thẻ" => txtCard,
            "Họ tên" => txtFullName,
            "Địa chỉ" => txtAddress,
            "Số điện thoại" => txtPhone,
            "Email" => txtEmail,
            "Ngày cấp thẻ" or "Ngày hết hạn" => dtpExpireDate,
            _ => null
        };

        if (ctrl is not null)
        {
            errorProvider.SetError(ctrl, fieldName);
            ctrl.Focus();
        }
    }

    private static void ShowError(Exception ex)
    {
        MessageBox.Show(ex.Message, "Lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
