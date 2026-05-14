using LibraryApp.BLL.Common;
using LibraryApp.DAL.Common;
using LibraryApp.Models;
using LibraryApp.Models.Enums;
using LibraryApp.UI.Common;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibraryApp.UI.UserControls;

/// <summary>
/// UserControl quản lý sách: tìm kiếm, thêm, sửa, xoá, làm mới.
/// </summary>
/// <remarks>
/// Sơ đồ tương tác:
/// <list type="bullet">
/// <item>Load → bind ComboBox danh mục + trạng thái → load danh sách lần đầu.</item>
/// <item>User chọn dòng trên grid → đổ dữ liệu lên form bên phải.</item>
/// <item>Nút Thêm → clear form, chuyển sang chế độ "Add".</item>
/// <item>Nút Lưu → validate qua <c>BookService.Create</c>.</item>
/// <item>Nút Cập nhật → validate qua <c>BookService.Update</c>.</item>
/// <item>Nút Xoá → confirm + <c>BookService.Delete</c>.</item>
/// </list>
/// </remarks>
public partial class UcBookList : UserControl
{
    /// <summary>Chế độ form hiện tại.</summary>
    private enum FormMode { Add, Edit }

    /// <summary>Chế độ hiện hành: Thêm mới hoặc Sửa.</summary>
    private FormMode _mode = FormMode.Add;

    /// <summary>BookId của dòng đang chọn (dùng khi Update/Delete).</summary>
    private int _selectedBookId;

    /// <summary>Cờ chống vòng lặp khi cập nhật form từ event SelectionChanged.</summary>
    private bool _suspendSelectionChanged;

    /// <summary>Khởi tạo và đăng ký event Load.</summary>
    public UcBookList()
    {
        InitializeComponent();
        Load += UcBookList_Load;
    }

    // ================================================================
    // Lifecycle
    // ================================================================

    private void UcBookList_Load(object? sender, EventArgs e)
    {
        try
        {
            BindCategoryCombos();
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

    /// <summary>
    /// Bind ComboBox danh mục từ <c>CategoryService.GetActive()</c> cho cả
    /// filter ở trên và combobox trên form nhập liệu.
    /// </summary>
    private void BindCategoryCombos()
    {
        var categories = ServiceLocator.Categories.GetActive();

        // ComboBox lọc (có thêm mục "Tất cả")
        var withAll = new List<Category>
        {
            new() { CategoryId = 0, CategoryCode = "", CategoryName = "-- Tất cả --" }
        };
        withAll.AddRange(categories);
        cboFilterCategory.DataSource = withAll;
        cboFilterCategory.DisplayMember = nameof(Category.CategoryName);
        cboFilterCategory.ValueMember = nameof(Category.CategoryId);
        cboFilterCategory.SelectedIndex = 0;

        // ComboBox trên form (không có "Tất cả" — phải chọn 1 danh mục cụ thể)
        cboCategory.DataSource = new List<Category>(categories);   // clone để không share datasource
        cboCategory.DisplayMember = nameof(Category.CategoryName);
        cboCategory.ValueMember = nameof(Category.CategoryId);
        if (categories.Count > 0) cboCategory.SelectedIndex = 0;
    }

    /// <summary>Bind ComboBox trạng thái sách (enum <see cref="BookStatus"/>).</summary>
    private void BindStatusCombos()
    {
        // Trên filter: có thêm "Tất cả"
        var filterItems = new List<StatusItem>
        {
            new(null, "-- Tất cả --")
        };
        foreach (BookStatus s in Enum.GetValues<BookStatus>())
            filterItems.Add(new StatusItem(s, GetStatusDisplayName(s)));

        cboFilterStatus.DataSource = filterItems;
        cboFilterStatus.DisplayMember = nameof(StatusItem.Display);
        cboFilterStatus.ValueMember = nameof(StatusItem.Value);
        cboFilterStatus.SelectedIndex = 0;

        // Trên form: liệt kê toàn bộ enum
        var statusItems = new List<StatusItem>();
        foreach (BookStatus s in Enum.GetValues<BookStatus>())
            statusItems.Add(new StatusItem(s, GetStatusDisplayName(s)));

        cboStatus.DataSource = statusItems;
        cboStatus.DisplayMember = nameof(StatusItem.Display);
        cboStatus.ValueMember = nameof(StatusItem.Value);
        cboStatus.SelectedIndex = 0;
    }

    /// <summary>Chuyển enum BookStatus sang tên tiếng Việt cho UI.</summary>
    private static string GetStatusDisplayName(BookStatus status) => status switch
    {
        BookStatus.Available => "Đang sẵn có",
        BookStatus.OutOfStock => "Hết tồn kho",
        BookStatus.Lost => "Đã mất",
        BookStatus.Damaged => "Bị hỏng",
        BookStatus.Retired => "Ngừng lưu thông",
        _ => status.ToString()
    };

    /// <summary>Helper item cho ComboBox enum.</summary>
    private sealed record StatusItem(BookStatus? Value, string Display);

    // ================================================================
    // Grid binding + selection
    // ================================================================

    /// <summary>Tải lại lưới theo điều kiện filter hiện tại.</summary>
    private void ReloadGrid()
    {
        try
        {
            UseWaitCursor = true;

            // Lấy filter
            string? keyword = string.IsNullOrWhiteSpace(txtSearchKeyword.Text) ? null : txtSearchKeyword.Text.Trim();
            int? categoryId = cboFilterCategory.SelectedValue is int catId && catId > 0 ? catId : null;
            BookStatus? status = (cboFilterStatus.SelectedItem as StatusItem)?.Value;

            // BLL trả về DataTable đã JOIN sẵn CategoryName → bind thẳng vào DataGridView
            var table = ServiceLocator.Books.SearchAsDataTable(keyword, categoryId, status, null, null);

            // Tránh trigger SelectionChanged khi đang reload
            _suspendSelectionChanged = true;
            dgvBooks.DataSource = table;
            dgvBooks.ClearSelection();
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

    /// <summary>
    /// Format giá trị enum <c>Status</c> hiển thị thân thiện (Available → "Đang sẵn có").
    /// </summary>
    private void dgvBooks_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (dgvBooks.Columns[e.ColumnIndex].Name == "Status" && e.Value is string s
            && Enum.TryParse<BookStatus>(s, out var status))
        {
            e.Value = GetStatusDisplayName(status);
            e.FormattingApplied = true;
        }
    }

    /// <summary>
    /// Khi user chọn dòng → đổ dữ liệu lên form và chuyển sang chế độ Edit.
    /// </summary>
    private void dgvBooks_SelectionChanged(object? sender, EventArgs e)
    {
        if (_suspendSelectionChanged) return;

        if (dgvBooks.CurrentRow is null)
        {
            SetMode(FormMode.Add);
            return;
        }

        try
        {
            int bookId = Convert.ToInt32(dgvBooks.CurrentRow.Cells["BookId"].Value);
            var book = ServiceLocator.Books.GetById(bookId);
            if (book is null)
            {
                SetMode(FormMode.Add);
                return;
            }

            // Đổ dữ liệu lên form
            txtCode.Text = book.BookCode;
            txtTitle.Text = book.Title;
            txtAuthor.Text = book.Author;
            txtPublisher.Text = book.Publisher ?? string.Empty;
            numYear.Value = book.PublishYear ?? DateTime.Now.Year;
            cboCategory.SelectedValue = book.CategoryId;
            numQuantity.Value = book.Quantity;
            numPrice.Value = book.Price;

            // Chọn đúng status trong combobox
            for (int i = 0; i < cboStatus.Items.Count; i++)
            {
                if (cboStatus.Items[i] is StatusItem item && item.Value == book.Status)
                {
                    cboStatus.SelectedIndex = i;
                    break;
                }
            }

            _selectedBookId = book.BookId;
            SetMode(FormMode.Edit);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    // ================================================================
    // Mode handling
    // ================================================================

    /// <summary>Chuyển form sang chế độ Thêm mới hoặc Sửa.</summary>
    private void SetMode(FormMode mode)
    {
        _mode = mode;
        errorProvider.Clear();

        if (mode == FormMode.Add)
        {
            _selectedBookId = 0;
            ClearFormInputs();
            txtCode.ReadOnly = false;   // cho phép nhập mã sách khi thêm mới
            txtCode.BackColor = System.Drawing.Color.White;

            btnSave.Visible = true;
            btnUpdate.Visible = false;
            btnDelete.Visible = false;
            lblFormHeader.Text = "Thêm sách mới";
        }
        else
        {
            txtCode.ReadOnly = true;    // không cho sửa mã sách
            txtCode.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);

            btnSave.Visible = false;
            btnUpdate.Visible = true;
            btnDelete.Visible = true;
            lblFormHeader.Text = $"Sửa: {txtCode.Text}";
        }
    }

    /// <summary>Xoá nội dung các ô nhập liệu (giữ lại danh mục, status mặc định).</summary>
    private void ClearFormInputs()
    {
        txtCode.Clear();
        txtTitle.Clear();
        txtAuthor.Clear();
        txtPublisher.Clear();
        numYear.Value = DateTime.Now.Year;
        numQuantity.Value = 0;
        numPrice.Value = 0;
        if (cboCategory.Items.Count > 0) cboCategory.SelectedIndex = 0;
        if (cboStatus.Items.Count > 0) cboStatus.SelectedIndex = 0;
    }

    // ================================================================
    // Button handlers
    // ================================================================

    private void btnNew_Click(object? sender, EventArgs e)
    {
        _suspendSelectionChanged = true;
        dgvBooks.ClearSelection();
        _suspendSelectionChanged = false;
        SetMode(FormMode.Add);
        txtCode.Focus();
    }

    /// <summary>Lưu sách mới.</summary>
    private void btnSave_Click(object? sender, EventArgs e)
    {
        if (!ValidateForm()) return;

        try
        {
            var book = BuildBookFromForm();
            book.CreatedBy = CurrentSession.CurrentUser?.UserId;

            int newId = ServiceLocator.Books.Create(book);
            MessageBox.Show($"Đã thêm sách '{book.Title}' (ID: {newId}).",
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

    /// <summary>Cập nhật sách đã chọn.</summary>
    private void btnUpdate_Click(object? sender, EventArgs e)
    {
        if (_selectedBookId <= 0)
        {
            MessageBox.Show("Vui lòng chọn sách cần sửa từ danh sách.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!ValidateForm(forUpdate: true)) return;

        try
        {
            // Lấy RowVersion từ DB để check concurrency
            var current = ServiceLocator.Books.GetById(_selectedBookId)
                ?? throw new BusinessException("Sách không còn tồn tại hoặc đã bị xoá.");

            var book = BuildBookFromForm();
            book.BookId = _selectedBookId;
            book.BookCode = current.BookCode;   // giữ nguyên mã
            book.RowVersion = current.RowVersion;
            book.UpdatedBy = CurrentSession.CurrentUser?.UserId;

            ServiceLocator.Books.Update(book);
            MessageBox.Show($"Đã cập nhật sách '{book.Title}'.",
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
        catch (DalException ex) when (ex.SqlErrorNumber == 50012)
        {
            // Concurrency conflict - khuyến nghị reload
            MessageBox.Show("Dữ liệu đã bị người khác thay đổi. Vui lòng tải lại và thử lại.",
                "Xung đột dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ReloadGrid();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    /// <summary>Xoá mềm sách đã chọn (xác nhận trước).</summary>
    private void btnDelete_Click(object? sender, EventArgs e)
    {
        if (_selectedBookId <= 0)
        {
            MessageBox.Show("Vui lòng chọn sách cần xoá từ danh sách.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show(
            $"Bạn có chắc muốn xoá sách '{txtTitle.Text}' (mã: {txtCode.Text})?\n\n" +
            "Lưu ý: sách đang được mượn sẽ không thể xoá. Dữ liệu sẽ được xoá mềm để có thể khôi phục.",
            "Xác nhận xoá",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes) return;

        try
        {
            ServiceLocator.Books.Delete(_selectedBookId);
            MessageBox.Show("Đã xoá sách khỏi hệ thống.",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ReloadGrid();
            SetMode(FormMode.Add);
        }
        catch (BusinessException ex)
        {
            MessageBox.Show(ex.Message, "Không thể xoá",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    /// <summary>Bỏ chọn dòng và chuyển về chế độ Thêm mới.</summary>
    private void btnCancel_Click(object? sender, EventArgs e)
    {
        _suspendSelectionChanged = true;
        dgvBooks.ClearSelection();
        _suspendSelectionChanged = false;
        SetMode(FormMode.Add);
    }

    private void btnSearch_Click(object? sender, EventArgs e) => ReloadGrid();

    private void btnReload_Click(object? sender, EventArgs e)
    {
        // Reset filter rồi reload
        txtSearchKeyword.Clear();
        if (cboFilterCategory.Items.Count > 0) cboFilterCategory.SelectedIndex = 0;
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
    // Validate + helpers
    // ================================================================

    /// <summary>
    /// Validate các ô nhập trên form. BLL cũng validate nhưng làm ở đây
    /// để báo lỗi ngay lập tức cho UX tốt hơn (không phải đi vòng qua DB).
    /// </summary>
    private bool ValidateForm(bool forUpdate = false)
    {
        errorProvider.Clear();

        if (!forUpdate && string.IsNullOrWhiteSpace(txtCode.Text))
        {
            errorProvider.SetError(txtCode, "Mã sách không được để trống.");
            txtCode.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtTitle.Text))
        {
            errorProvider.SetError(txtTitle, "Tên sách không được để trống.");
            txtTitle.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtAuthor.Text))
        {
            errorProvider.SetError(txtAuthor, "Tác giả không được để trống.");
            txtAuthor.Focus();
            return false;
        }

        if (cboCategory.SelectedValue is null)
        {
            errorProvider.SetError(cboCategory, "Vui lòng chọn danh mục.");
            cboCategory.Focus();
            return false;
        }

        if (numQuantity.Value < 0)
        {
            errorProvider.SetError(numQuantity, "Số lượng không được âm.");
            numQuantity.Focus();
            return false;
        }

        return true;
    }

    /// <summary>Tạo entity <see cref="Book"/> từ giá trị các ô nhập liệu.</summary>
    private Book BuildBookFromForm()
    {
        var statusItem = (StatusItem)cboStatus.SelectedItem!;

        return new Book
        {
            BookCode = txtCode.Text.Trim(),
            Title = txtTitle.Text.Trim(),
            Author = txtAuthor.Text.Trim(),
            Publisher = string.IsNullOrWhiteSpace(txtPublisher.Text) ? null : txtPublisher.Text.Trim(),
            PublishYear = (int)numYear.Value,
            CategoryId = (int)cboCategory.SelectedValue!,
            Quantity = (int)numQuantity.Value,
            Price = numPrice.Value,
            Status = statusItem.Value ?? BookStatus.Available
        };
    }

    /// <summary>Highlight ô nhập tương ứng với <paramref name="fieldName"/> nếu có.</summary>
    private void HighlightField(string? fieldName)
    {
        if (string.IsNullOrEmpty(fieldName)) return;

        Control? ctrl = fieldName switch
        {
            "Mã sách" => txtCode,
            "Tên sách" => txtTitle,
            "Tác giả" => txtAuthor,
            "Nhà xuất bản" => txtPublisher,
            "Năm xuất bản" => numYear,
            "Danh mục" => cboCategory,
            "Số lượng" => numQuantity,
            "Giá" => numPrice,
            _ => null
        };

        if (ctrl is not null)
        {
            errorProvider.SetError(ctrl, fieldName);
            ctrl.Focus();
        }
    }

    /// <summary>Hiển thị MessageBox lỗi chuẩn.</summary>
    private static void ShowError(Exception ex)
    {
        MessageBox.Show(ex.Message, "Lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
