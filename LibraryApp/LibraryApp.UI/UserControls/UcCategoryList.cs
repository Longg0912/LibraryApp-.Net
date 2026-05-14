using LibraryApp.BLL.Common;
using LibraryApp.Models;
using LibraryApp.UI.Common;

namespace LibraryApp.UI.UserControls;

/// <summary>
/// UserControl quản lý danh mục sách: tìm kiếm + CRUD đầy đủ.
/// Đồng bộ về layout và pattern với <see cref="UcBookList"/>.
/// </summary>
/// <remarks>
/// Khác biệt với <see cref="UcBookList"/>:
/// <list type="bullet">
/// <item>Có ít field hơn (chỉ Mã, Tên, Mô tả, IsActive).</item>
/// <item>Filter đơn giản: từ khoá + checkbox "chỉ hiển thị active".</item>
/// <item>Không cần ComboBox danh mục — chính danh mục là entity gốc.</item>
/// <item>Khi xoá: nếu còn sách tham chiếu, BLL sẽ ném BusinessException
/// (lỗi <c>50100</c> từ stored procedure).</item>
/// </list>
/// </remarks>
public partial class UcCategoryList : UserControl
{
    private enum FormMode { Add, Edit }

    private FormMode _mode = FormMode.Add;
    private int _selectedCategoryId;
    private bool _suspendSelectionChanged;

    /// <summary>Cache toàn bộ danh mục để filter client-side.</summary>
    private List<Category> _allCategories = [];

    /// <summary>Khởi tạo UserControl và đăng ký event Load.</summary>
    public UcCategoryList()
    {
        InitializeComponent();
        Load += UcCategoryList_Load;
    }

    // ================================================================
    // Lifecycle
    // ================================================================

    private void UcCategoryList_Load(object? sender, EventArgs e)
    {
        try
        {
            SetMode(FormMode.Add);
            ReloadGrid();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    // ================================================================
    // Grid load + filter
    // ================================================================

    /// <summary>
    /// Tải lại danh sách từ <c>CategoryService.GetAll()</c>, sau đó filter
    /// theo từ khoá + checkbox "Chỉ hoạt động".
    /// </summary>
    /// <remarks>
    /// Vì số lượng danh mục thường rất nhỏ (vài chục), tải hết về client
    /// rồi filter sẽ nhanh hơn nhiều so với mỗi lần tìm kiếm lại query DB.
    /// </remarks>
    private void ReloadGrid()
    {
        try
        {
            UseWaitCursor = true;
            _allCategories = ServiceLocator.Categories.GetAll();
            ApplyFilter();
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

    /// <summary>Lọc <see cref="_allCategories"/> theo điều kiện UI và bind vào grid.</summary>
    private void ApplyFilter()
    {
        IEnumerable<Category> filtered = _allCategories;

        // Filter từ khoá
        if (!string.IsNullOrWhiteSpace(txtSearchKeyword.Text))
        {
            var keyword = txtSearchKeyword.Text.Trim();
            filtered = filtered.Where(c =>
                c.CategoryCode.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || c.CategoryName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        // Filter active
        if (chkOnlyActive.Checked)
            filtered = filtered.Where(c => c.IsActive);

        // Bind
        _suspendSelectionChanged = true;
        dgvCategories.DataSource = filtered.ToList();
        dgvCategories.ClearSelection();
        _suspendSelectionChanged = false;
    }

    /// <summary>Format cột IsActive sang "Có"/"Không" cho UI.</summary>
    private void dgvCategories_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (dgvCategories.Columns[e.ColumnIndex].Name == "IsActive" && e.Value is bool b)
        {
            e.Value = b ? "✓ Có" : "✗ Không";
            e.FormattingApplied = true;
        }
    }

    /// <summary>Khi user click dòng → đổ data lên form, chuyển sang Edit.</summary>
    private void dgvCategories_SelectionChanged(object? sender, EventArgs e)
    {
        if (_suspendSelectionChanged) return;

        if (dgvCategories.CurrentRow?.DataBoundItem is not Category cat)
        {
            SetMode(FormMode.Add);
            return;
        }

        txtCode.Text = cat.CategoryCode;
        txtName.Text = cat.CategoryName;
        txtDescription.Text = cat.Description ?? string.Empty;
        chkIsActive.Checked = cat.IsActive;
        _selectedCategoryId = cat.CategoryId;

        SetMode(FormMode.Edit);
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
            _selectedCategoryId = 0;
            ClearFormInputs();
            txtCode.ReadOnly = false;
            txtCode.BackColor = System.Drawing.Color.White;

            btnSave.Visible = true;
            btnUpdate.Visible = false;
            btnDelete.Visible = false;
            lblFormHeader.Text = "Thêm danh mục mới";
        }
        else
        {
            txtCode.ReadOnly = true;
            txtCode.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);

            btnSave.Visible = false;
            btnUpdate.Visible = true;
            btnDelete.Visible = true;
            lblFormHeader.Text = $"Sửa: {txtCode.Text}";
        }
    }

    private void ClearFormInputs()
    {
        txtCode.Clear();
        txtName.Clear();
        txtDescription.Clear();
        chkIsActive.Checked = true;
    }

    // ================================================================
    // Button handlers
    // ================================================================

    private void btnNew_Click(object? sender, EventArgs e)
    {
        _suspendSelectionChanged = true;
        dgvCategories.ClearSelection();
        _suspendSelectionChanged = false;
        SetMode(FormMode.Add);
        txtCode.Focus();
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        if (!ValidateForm()) return;

        try
        {
            var cat = BuildCategoryFromForm();
            cat.CreatedBy = CurrentSession.CurrentUser?.UserId;

            int newId = ServiceLocator.Categories.Create(cat);
            MessageBox.Show($"Đã thêm danh mục '{cat.CategoryName}' (ID: {newId}).",
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
        if (_selectedCategoryId <= 0)
        {
            MessageBox.Show("Vui lòng chọn danh mục cần sửa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!ValidateForm(forUpdate: true)) return;

        try
        {
            var cat = BuildCategoryFromForm();
            cat.CategoryId = _selectedCategoryId;
            cat.CategoryCode = txtCode.Text.Trim();   // giữ nguyên mã
            cat.UpdatedBy = CurrentSession.CurrentUser?.UserId;

            ServiceLocator.Categories.Update(cat);
            MessageBox.Show($"Đã cập nhật danh mục '{cat.CategoryName}'.",
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

    private void btnDelete_Click(object? sender, EventArgs e)
    {
        if (_selectedCategoryId <= 0)
        {
            MessageBox.Show("Vui lòng chọn danh mục cần xoá.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show(
            $"Bạn có chắc muốn xoá danh mục '{txtName.Text}'?\n\n" +
            "Lưu ý: danh mục đang chứa sách sẽ không thể xoá.\n" +
            "Dữ liệu sẽ được xoá mềm để có thể khôi phục sau.",
            "Xác nhận xoá",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes) return;

        try
        {
            ServiceLocator.Categories.Delete(_selectedCategoryId);
            MessageBox.Show("Đã xoá danh mục khỏi hệ thống.",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ReloadGrid();
            SetMode(FormMode.Add);
        }
        catch (BusinessException ex)
        {
            // Trường hợp danh mục còn chứa sách: BLL bao bọc lỗi 50100 từ SP
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
        dgvCategories.ClearSelection();
        _suspendSelectionChanged = false;
        SetMode(FormMode.Add);
    }

    private void btnSearch_Click(object? sender, EventArgs e) => ApplyFilter();

    private void btnReload_Click(object? sender, EventArgs e)
    {
        txtSearchKeyword.Clear();
        chkOnlyActive.Checked = false;
        ReloadGrid();
    }

    private void chkOnlyActive_CheckedChanged(object? sender, EventArgs e)
    {
        ApplyFilter();
    }

    private void txtSearchKeyword_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            ApplyFilter();
        }
    }

    // ================================================================
    // Validate + helpers
    // ================================================================

    /// <summary>
    /// Validate ô nhập trên form. Tên danh mục bắt buộc; mã chỉ bắt buộc
    /// khi thêm mới (Edit không cho sửa mã).
    /// </summary>
    private bool ValidateForm(bool forUpdate = false)
    {
        errorProvider.Clear();

        if (!forUpdate)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                errorProvider.SetError(txtCode, "Mã danh mục không được để trống.");
                txtCode.Focus();
                return false;
            }
            if (txtCode.Text.Trim().Length < 2)
            {
                errorProvider.SetError(txtCode, "Mã danh mục phải có ít nhất 2 ký tự.");
                txtCode.Focus();
                return false;
            }
        }

        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            errorProvider.SetError(txtName, "Tên danh mục không được để trống.");
            txtName.Focus();
            return false;
        }

        if (txtName.Text.Trim().Length < 2)
        {
            errorProvider.SetError(txtName, "Tên danh mục phải có ít nhất 2 ký tự.");
            txtName.Focus();
            return false;
        }

        return true;
    }

    private Category BuildCategoryFromForm() => new()
    {
        CategoryCode = txtCode.Text.Trim(),
        CategoryName = txtName.Text.Trim(),
        Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim(),
        IsActive = chkIsActive.Checked
    };

    private void HighlightField(string? fieldName)
    {
        if (string.IsNullOrEmpty(fieldName)) return;

        Control? ctrl = fieldName switch
        {
            "Mã danh mục" => txtCode,
            "Tên danh mục" => txtName,
            "Mô tả" => txtDescription,
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
