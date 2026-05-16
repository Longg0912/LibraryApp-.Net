using System.ComponentModel;
using LibraryApp.BLL.Common;
using LibraryApp.DAL.Common;
using LibraryApp.Models;
using LibraryApp.Models.Enums;
using LibraryApp.UI.Common;

namespace LibraryApp.UI.UserControls;

/// <summary>
/// Form lập phiếu mượn sách theo pattern "giỏ hàng":
/// (1) thủ thư tra cứu độc giả qua số thẻ →
/// (2) thêm từng sách vào giỏ với số lượng →
/// (3) bấm "Lập phiếu mượn" để gửi toàn bộ giỏ qua stored procedure
/// <c>sp_Borrow_Create</c> (transaction + race-condition-safe).
/// </summary>
/// <remarks>
/// Toàn bộ kiểm tra tồn kho, validate thẻ độc giả, trừ AvailableQty đều
/// nằm trong stored procedure. UI làm 2 việc:
/// - Pre-check để báo lỗi sớm (UX tốt: không cần round-trip DB mới biết hết sách)
/// - Hiển thị lỗi từ BusinessException (BLL bao bọc lỗi 5002x) cho người dùng
/// </remarks>
public partial class UcBorrowCreate : UserControl
{
    /// <summary>Độc giả đã chọn cho phiếu mượn này.</summary>
    private Reader? _selectedReader;

    /// <summary>
    /// Giỏ sách dạng <see cref="BindingList{T}"/> để DataGridView tự refresh
    /// khi add/remove. Mỗi <see cref="CartItem"/> đại diện một đầu sách trong phiếu.
    /// </summary>
    private readonly BindingList<CartItem> _cart = new();

    /// <summary>Cache danh sách sách có sẵn để filter combobox không cần round-trip DB.</summary>
    private List<Book> _availableBooks = [];

    public UcBorrowCreate()
    {
        InitializeComponent();
        Load += UcBorrowCreate_Load;
    }

    // ================================================================
    // Lifecycle
    // ================================================================

    private void UcBorrowCreate_Load(object? sender, EventArgs e)
    {
        try
        {
            // Load danh sách sách khả dụng cho combobox tự gợi ý
            _availableBooks = ServiceLocator.Books
                .Search(null, null, BookStatus.Available, null, null)
                .Where(b => b.AvailableQty > 0)
                .OrderBy(b => b.Title)
                .ToList();

            RebuildCandidateCombo();

            dgvCart.DataSource = _cart;
            _cart.ListChanged += (_, _) => UpdateTotal();

            UpdateReaderInfoUi(null);
            UpdateTotal();
            txtCardSearch.Focus();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    // ================================================================
    // Reader lookup
    // ================================================================

    private void txtCardSearch_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            LookupReader();
        }
    }

    private void btnLookupReader_Click(object? sender, EventArgs e) => LookupReader();

    /// <summary>
    /// Tra cứu độc giả theo số thẻ. Hiển thị thông tin + cảnh báo
    /// nếu thẻ hết hạn / bị khoá / không tồn tại.
    /// </summary>
    private void LookupReader()
    {
        var card = txtCardSearch.Text.Trim();
        if (string.IsNullOrEmpty(card))
        {
            errorProvider.SetError(txtCardSearch, "Vui lòng nhập số thẻ.");
            txtCardSearch.Focus();
            return;
        }
        errorProvider.SetError(txtCardSearch, "");

        try
        {
            var reader = ServiceLocator.Readers.GetByCardNumber(card);
            if (reader is null)
            {
                MessageBox.Show($"Không tìm thấy độc giả với số thẻ '{card}'.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _selectedReader = null;
                UpdateReaderInfoUi(null);
                return;
            }

            // Pre-check thẻ còn hiệu lực không
            if (!reader.IsCardValid)
            {
                var reason = reader.Status switch
                {
                    ReaderStatus.Locked => "Thẻ độc giả đang bị khoá.",
                    ReaderStatus.Expired => "Thẻ độc giả đã hết hạn.",
                    _ => $"Thẻ độc giả không hợp lệ (hết hạn ngày {reader.CardExpireDate:dd/MM/yyyy})."
                };
                MessageBox.Show(reason + "\n\nVẫn có thể tiếp tục nhưng phiếu mượn sẽ bị từ chối khi lưu.",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            _selectedReader = reader;
            UpdateReaderInfoUi(reader);
            txtBookKeyword.Focus();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    /// <summary>Hiển thị thông tin độc giả lên panel + chuyển sang trạng thái đã chọn.</summary>
    private void UpdateReaderInfoUi(Reader? reader)
    {
        if (reader is null)
        {
            lblReaderName.Text = "Chưa chọn độc giả";
            lblReaderName.ForeColor = System.Drawing.Color.FromArgb(150, 150, 150);
            lblReaderCard.Text = "Số thẻ: -";
            lblReaderStatus.Text = "Trạng thái: -";
            lblReaderExpiry.Text = "Hết hạn: -";
            lblReaderActive.Text = "Đang mượn: -";
            return;
        }

        lblReaderName.Text = reader.FullName;
        lblReaderName.ForeColor = reader.IsCardValid
            ? System.Drawing.Color.FromArgb(33, 64, 154)
            : System.Drawing.Color.FromArgb(220, 53, 69);

        lblReaderCard.Text = $"Số thẻ: {reader.CardNumber}";
        lblReaderStatus.Text = $"Trạng thái: {GetReaderStatusDisplay(reader.Status)}";
        lblReaderExpiry.Text = $"Hết hạn: {reader.CardExpireDate:dd/MM/yyyy}";

        // Đếm số sách đang mượn
        try
        {
            var activeBorrows = ServiceLocator.Borrow.GetActiveByReader(reader.ReaderId);
            int activeCount = activeBorrows.Sum(b => b.Details?.Count ?? 0);
            lblReaderActive.Text = $"Đang mượn: {activeBorrows.Count} phiếu";
        }
        catch
        {
            lblReaderActive.Text = "Đang mượn: -";
        }
    }

    private static string GetReaderStatusDisplay(ReaderStatus s) => s switch
    {
        ReaderStatus.Active => "Đang hoạt động",
        ReaderStatus.Locked => "Đã bị khoá",
        ReaderStatus.Expired => "Hết hạn thẻ",
        _ => s.ToString()
    };

    // ================================================================
    // Book candidate combobox - tự lọc theo từ khoá nhập
    // ================================================================

    private void txtBookKeyword_TextChanged(object? sender, EventArgs e)
    {
        RebuildCandidateCombo();
    }

    /// <summary>
    /// Build lại danh sách combobox dựa trên từ khoá (filter tại client).
    /// Chỉ hiện sách <c>AvailableQty &gt; 0</c> để tránh thêm sách hết kho vào giỏ.
    /// </summary>
    private void RebuildCandidateCombo()
    {
        var keyword = txtBookKeyword.Text.Trim();

        IEnumerable<Book> candidates = _availableBooks;
        if (!string.IsNullOrEmpty(keyword))
        {
            candidates = candidates.Where(b =>
                b.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || b.BookCode.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || b.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        // Loại bỏ sách đã có trong giỏ
        var inCart = _cart.Select(c => c.BookId).ToHashSet();
        candidates = candidates.Where(b => !inCart.Contains(b.BookId));

        var list = candidates.Take(50).ToList();   // tối đa 50 gợi ý
        cboBookCandidates.DataSource = list;
        cboBookCandidates.DisplayMember = nameof(Book.ToString);   // dùng ToString của Book (Code - Title)
        cboBookCandidates.ValueMember = nameof(Book.BookId);

        // Reset số lượng khi đổi sách
        numQty.Value = 1;
    }

    // ================================================================
    // Cart management
    // ================================================================

    /// <summary>Thêm sách đang chọn vào giỏ với số lượng đã nhập.</summary>
    private void btnAddToCart_Click(object? sender, EventArgs e)
    {
        if (cboBookCandidates.SelectedItem is not Book book)
        {
            MessageBox.Show("Vui lòng chọn một sách từ danh sách.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int qty = (int)numQty.Value;

        // Pre-check tồn kho
        if (qty > book.AvailableQty)
        {
            MessageBox.Show(
                $"Sách '{book.Title}' chỉ còn {book.AvailableQty} bản, không đủ cho {qty} bản yêu cầu.",
                "Không đủ tồn kho", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Pre-check trạng thái
        if (book.Status != BookStatus.Available)
        {
            MessageBox.Show($"Sách '{book.Title}' không thể cho mượn ({book.Status}).",
                "Sách không khả dụng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _cart.Add(new CartItem
        {
            BookId = book.BookId,
            BookCode = book.BookCode,
            Title = book.Title,
            Author = book.Author,
            Quantity = qty,
            AvailableQty = book.AvailableQty
        });

        // Loại sách vừa thêm khỏi combobox (đã có trong giỏ)
        RebuildCandidateCombo();
        txtBookKeyword.Clear();
        txtBookKeyword.Focus();
    }

    /// <summary>Bỏ dòng đang chọn khỏi giỏ.</summary>
    private void btnRemoveItem_Click(object? sender, EventArgs e)
    {
        if (dgvCart.CurrentRow?.DataBoundItem is not CartItem item)
        {
            MessageBox.Show("Vui lòng chọn một dòng để bỏ khỏi giỏ.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        _cart.Remove(item);
        RebuildCandidateCombo();
    }

    /// <summary>Xoá toàn bộ giỏ (có xác nhận).</summary>
    private void btnClearCart_Click(object? sender, EventArgs e)
    {
        if (_cart.Count == 0) return;

        var confirm = MessageBox.Show(
            $"Xoá toàn bộ {_cart.Count} dòng khỏi giỏ?",
            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        _cart.Clear();
        RebuildCandidateCombo();
    }

    /// <summary>Cập nhật label tổng cuốn sách.</summary>
    private void UpdateTotal()
    {
        int total = _cart.Sum(c => c.Quantity);
        lblTotalQty.Text = $"Tổng: {total} cuốn / {_cart.Count} đầu sách";
        btnCreate.Enabled = _cart.Count > 0 && _selectedReader is not null;
    }

    // ================================================================
    // Date validation
    // ================================================================

    private void dtpBorrowDate_ValueChanged(object? sender, EventArgs e)
    {
        // Hạn trả tự đẩy lên thành BorrowDate + 14 ngày nếu trước đó user chưa chỉnh
        if (dtpDueDate.Value <= dtpBorrowDate.Value)
            dtpDueDate.Value = dtpBorrowDate.Value.AddDays(14);
    }

    // ================================================================
    // Create receipt
    // ================================================================

    /// <summary>
    /// Lập phiếu mượn: validate xong gọi <c>BorrowService.CreateBorrow</c>,
    /// stored procedure xử lý toàn bộ logic transaction.
    /// </summary>
    private void btnCreate_Click(object? sender, EventArgs e)
    {
        if (_selectedReader is null)
        {
            MessageBox.Show("Vui lòng tra cứu độc giả trước khi lập phiếu.",
                "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtCardSearch.Focus();
            return;
        }

        if (_cart.Count == 0)
        {
            MessageBox.Show("Giỏ sách đang trống. Vui lòng thêm ít nhất một quyển sách.",
                "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtBookKeyword.Focus();
            return;
        }

        var borrowDate = DateOnly.FromDateTime(dtpBorrowDate.Value);
        var dueDate = DateOnly.FromDateTime(dtpDueDate.Value);

        if (dueDate <= borrowDate)
        {
            MessageBox.Show("Hạn trả phải sau ngày mượn.",
                "Ngày không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            dtpDueDate.Focus();
            return;
        }

        // Xác nhận cuối
        var summary = string.Join("\n", _cart.Take(5).Select(c => $"  • {c.Title} × {c.Quantity}"));
        if (_cart.Count > 5) summary += $"\n  ...và {_cart.Count - 5} sách khác";

        var confirm = MessageBox.Show(
            $"Xác nhận lập phiếu mượn cho '{_selectedReader.FullName}'?\n\n" +
            $"Sách mượn:\n{summary}\n\n" +
            $"Tổng: {_cart.Sum(c => c.Quantity)} cuốn\n" +
            $"Hạn trả: {dueDate:dd/MM/yyyy}",
            "Xác nhận lập phiếu",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        // Gọi BorrowService
        try
        {
            UseWaitCursor = true;
            btnCreate.Enabled = false;

            var currentUser = CurrentSession.CurrentUser
                ?? throw new BusinessException("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.");

            var items = _cart.Select(c => (c.BookId, c.Quantity));

            var receipt = ServiceLocator.Borrow.CreateBorrow(
                readerId: _selectedReader.ReaderId,
                userId: currentUser.UserId,
                dueDate: dueDate,
                items: items,
                note: string.IsNullOrWhiteSpace(txtNote.Text) ? null : txtNote.Text.Trim());

            MessageBox.Show(
                $"✓ Đã lập phiếu mượn thành công.\n\n" +
                $"Mã phiếu: {receipt.ReceiptCode}\n" +
                $"Hạn trả: {receipt.DueDate:dd/MM/yyyy}",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Reset form
            ResetForm();
        }
        catch (BusinessException ex)
        {
            MessageBox.Show(ex.Message, "Không thể lập phiếu",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (DalException ex) when (ex.SqlErrorNumber is 50020 or 50021 or 50022)
        {
            // Phòng hờ nếu lỗi xuyên qua BLL (lý thuyết BLL đã bao bọc rồi)
            MessageBox.Show(ex.Message, "Không thể lập phiếu",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            UseWaitCursor = false;
            btnCreate.Enabled = _cart.Count > 0 && _selectedReader is not null;
        }
    }

    /// <summary>Đặt lại form về trạng thái rỗng sau khi lập phiếu xong.</summary>
    private void ResetForm()
    {
        _selectedReader = null;
        UpdateReaderInfoUi(null);
        txtCardSearch.Clear();
        txtBookKeyword.Clear();
        txtNote.Clear();
        _cart.Clear();
        dtpBorrowDate.Value = DateTime.Today;
        dtpDueDate.Value = DateTime.Today.AddDays(14);

        // Reload tồn kho (vì vừa trừ sau khi lập phiếu)
        try
        {
            _availableBooks = ServiceLocator.Books
                .Search(null, null, BookStatus.Available, null, null)
                .Where(b => b.AvailableQty > 0)
                .OrderBy(b => b.Title)
                .ToList();
            RebuildCandidateCombo();
        }
        catch { /* ignore reload error */ }

        txtCardSearch.Focus();
    }

    private static void ShowError(Exception ex)
    {
        MessageBox.Show(ex.Message, "Lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    // ================================================================
    // Nested type
    // ================================================================

    /// <summary>Một dòng trong giỏ sách mượn.</summary>
    public sealed class CartItem
    {
        public int BookId { get; set; }
        public string BookCode { get; set; } = "";
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public int Quantity { get; set; }
        public int AvailableQty { get; set; }
    }
}
