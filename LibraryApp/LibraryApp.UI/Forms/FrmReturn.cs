using System.ComponentModel;
using LibraryApp.BLL.Common;
using LibraryApp.DAL.Common;
using LibraryApp.Models;
using LibraryApp.Models.Enums;
using LibraryApp.UI.Common;

namespace LibraryApp.UI.Forms;

/// <summary>
/// Dialog ghi nhận trả sách cho một phiếu mượn cụ thể.
/// Mở từ <see cref="UserControls.UcBorrowList"/> bằng cách chuyển vào <c>BorrowId</c>.
/// </summary>
/// <remarks>
/// UX:
/// <list type="bullet">
/// <item>Hiển thị mọi dòng chi tiết còn nợ của phiếu mượn.</item>
/// <item>Cho phép thủ thư nhập số lượng trả + tình trạng (Tốt/Hỏng/Mất) + tiền phạt.</item>
/// <item>Tiền phạt tự gợi ý theo công thức trong BLL (quá hạn × 5.000 VNĐ/cuốn/ngày).</item>
/// <item>Khi nhấn "Ghi nhận trả", gọi <c>BorrowService.CreateReturn</c> →
/// stored procedure <c>sp_Return_Create</c>.</item>
/// </list>
/// </remarks>
public partial class FrmReturn : Form
{
    private readonly int _borrowId;
    private BorrowReceipt? _receipt;
    private readonly BindingList<ReturnLine> _lines = new();

    /// <summary>
    /// Khởi tạo dialog cho một phiếu mượn cụ thể.
    /// </summary>
    /// <param name="borrowId">ID phiếu mượn cần ghi nhận trả.</param>
    public FrmReturn(int borrowId)
    {
        _borrowId = borrowId;
        InitializeComponent();
    }

    // ================================================================
    // Form lifecycle
    // ================================================================

    /// <summary>
    /// Khi form load: tải phiếu mượn, validate trạng thái, đổ dữ liệu các dòng còn nợ.
    /// Nếu phiếu đã đóng → đóng form ngay với <see cref="DialogResult.Cancel"/>.
    /// </summary>
    private void FrmReturn_Load(object? sender, EventArgs e)
    {
        try
        {
            _receipt = ServiceLocator.Borrow.GetDetail(_borrowId);
            if (_receipt is null)
            {
                MessageBox.Show("Không tìm thấy phiếu mượn.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            if (_receipt.Status is BorrowStatus.Returned or BorrowStatus.Cancelled)
            {
                MessageBox.Show(
                    $"Phiếu này đã ở trạng thái '{_receipt.Status}', không thể ghi nhận trả thêm.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            // Cập nhật header
            lblReceiptInfo.Text =
                $"Mã phiếu: {_receipt.ReceiptCode} • " +
                $"Ngày mượn: {_receipt.BorrowDate:dd/MM/yyyy} • " +
                $"Hạn trả: {_receipt.DueDate:dd/MM/yyyy}";

            var reader = ServiceLocator.Readers.GetById(_receipt.ReaderId);
            lblReaderInfo.Text = reader is not null
                ? $"Độc giả: {reader.FullName} (Số thẻ: {reader.CardNumber})"
                : "Độc giả: -";

            // Cảnh báo quá hạn
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (_receipt.DueDate < today)
            {
                int days = today.DayNumber - _receipt.DueDate.DayNumber;
                lblWarning.Visible = true;
                lblWarning.Text = $"⚠ Phiếu đã quá hạn {days} ngày — tiền phạt sẽ được tính tự động.";
            }

            // Load các dòng chưa trả hết
            _lines.Clear();
            foreach (var detail in _receipt.Details.Where(d => d.Quantity > d.ReturnedQty))
            {
                var book = ServiceLocator.Books.GetById(detail.BookId);
                _lines.Add(new ReturnLine
                {
                    BorrowDetailId = detail.BorrowDetailId,
                    BookId = detail.BookId,
                    BookPrice = book?.Price ?? 0m,
                    Title = book?.Title ?? $"BookId {detail.BookId}",
                    BorrowedQty = detail.Quantity,
                    ReturnedQty = detail.ReturnedQty,
                    RemainingQty = detail.Quantity - detail.ReturnedQty,
                    ReturnNow = detail.Quantity - detail.ReturnedQty,   // mặc định trả hết
                    ConditionDisplay = "Tốt",
                    Fine = 0m
                });
            }

            grid.DataSource = _lines;
            _lines.ListChanged += (_, _) => UpdateTotalFineLabel();

            RecalculateAllFines();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    // ================================================================
    // Grid events - tự tính phạt realtime khi user đổi qty / condition
    // ================================================================

    /// <summary>
    /// Commit value ngay khi dirty để event <see cref="grid_CellValueChanged"/>
    /// bắn ra liền (mặc định ComboBoxColumn chỉ commit khi user click ra ngoài).
    /// </summary>
    private void grid_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (grid.IsCurrentCellDirty)
            grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    /// <summary>Bỏ qua DataError không quan trọng (ví dụ user gõ chữ vào cột số).</summary>
    private void grid_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        e.ThrowException = false;
    }

    /// <summary>
    /// Khi user đổi giá trị cell: validate qty (cap về [0, RemainingQty]) và tính lại phạt.
    /// </summary>
    private void grid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _lines.Count) return;

        var line = _lines[e.RowIndex];
        var colName = grid.Columns[e.ColumnIndex].Name;

        if (colName == "ReturnNow")
        {
            if (line.ReturnNow < 0)
                line.ReturnNow = 0;
            else if (line.ReturnNow > line.RemainingQty)
                line.ReturnNow = line.RemainingQty;
            RecalculateLineFine(line);
        }
        else if (colName == "ConditionDisplay")
        {
            RecalculateLineFine(line);
        }

        UpdateTotalFineLabel();
        grid.InvalidateRow(e.RowIndex);
    }

    /// <summary>Cập nhật phạt khi user đổi ngày trả.</summary>
    private void dtpReturnDate_ValueChanged(object? sender, EventArgs e)
        => RecalculateAllFines();

    /// <summary>
    /// Tính lại phạt cho một dòng dựa trên qty + condition + ngày trả.
    /// Quá hạn dùng công thức BLL <c>BorrowService.CalculateOverdueFine</c>.
    /// </summary>
    private void RecalculateLineFine(ReturnLine line)
    {
        if (_receipt is null || line.ReturnNow <= 0)
        {
            line.Fine = 0m;
            return;
        }

        var returnDate = DateOnly.FromDateTime(dtpReturnDate.Value);

        // 1. Phạt quá hạn
        decimal overdueFine = ServiceLocator.Borrow.CalculateOverdueFine(
            _receipt, line.ReturnNow, returnDate);

        // 2. Phạt hỏng / mất
        decimal damageFine = line.ConditionDisplay switch
        {
            "Hỏng" => line.BookPrice * line.ReturnNow * 0.3m,
            "Mất" => line.BookPrice * line.ReturnNow * 1.0m,
            _ => 0m
        };

        line.Fine = overdueFine + damageFine;
    }

    private void RecalculateAllFines()
    {
        foreach (var line in _lines)
            RecalculateLineFine(line);

        grid.Refresh();
        UpdateTotalFineLabel();
    }

    private void UpdateTotalFineLabel()
    {
        decimal total = _lines.Sum(l => l.Fine);
        lblTotalFine.Text = $"Tổng tiền phạt: {total:N0} VNĐ";
    }

    // ================================================================
    // Confirm return
    // ================================================================

    /// <summary>
    /// Gọi <c>BorrowService.CreateReturn</c> để ghi nhận trả qua stored procedure.
    /// SP xử lý transaction, cộng lại AvailableQty (trừ sách bị Mất), ghi PenaltyHistory.
    /// </summary>
    private void btnConfirm_Click(object? sender, EventArgs e)
    {
        var toReturn = _lines.Where(l => l.ReturnNow > 0).ToList();
        if (toReturn.Count == 0)
        {
            MessageBox.Show("Vui lòng nhập số lượng trả cho ít nhất một dòng.",
                "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        foreach (var line in toReturn)
        {
            if (line.ReturnNow > line.RemainingQty)
            {
                MessageBox.Show(
                    $"Số lượng trả của '{line.Title}' ({line.ReturnNow}) " +
                    $"vượt quá số còn nợ ({line.RemainingQty}).",
                    "Dữ liệu sai", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        var total = toReturn.Sum(l => l.Fine);
        var confirm = MessageBox.Show(
            $"Xác nhận ghi nhận trả {toReturn.Sum(l => l.ReturnNow)} cuốn?\n\n" +
            $"Tổng tiền phạt: {total:N0} VNĐ",
            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        try
        {
            UseWaitCursor = true;
            btnConfirm.Enabled = false;

            var currentUser = CurrentSession.CurrentUser!;
            var returnDate = DateOnly.FromDateTime(dtpReturnDate.Value);

            var items = toReturn.Select(l => (
                l.BorrowDetailId,
                l.ReturnNow,
                MapCondition(l.ConditionDisplay),
                (decimal?)l.Fine
            ));

            var receipt = ServiceLocator.Borrow.CreateReturn(
                borrowId: _borrowId,
                userId: currentUser.UserId,
                returnDate: returnDate,
                items: items,
                note: string.IsNullOrWhiteSpace(txtNote.Text) ? null : txtNote.Text.Trim());

            MessageBox.Show(
                $"✓ Đã ghi nhận trả sách.\n\n" +
                $"Mã phiếu trả: {receipt.ReturnCode}\n" +
                $"Tổng phạt: {receipt.TotalFine:N0} VNĐ",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (BusinessException ex)
        {
            MessageBox.Show(ex.Message, "Không thể ghi nhận trả",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (DalException ex) when (ex.SqlErrorNumber is 50030 or 50031)
        {
            MessageBox.Show(ex.Message, "Không thể ghi nhận trả",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            UseWaitCursor = false;
            btnConfirm.Enabled = true;
        }
    }

    private static BookCondition MapCondition(string display) => display switch
    {
        "Hỏng" => BookCondition.Damaged,
        "Mất" => BookCondition.Lost,
        _ => BookCondition.Good
    };

    // ================================================================
    // Nested type
    // ================================================================

    /// <summary>Dòng dữ liệu cho DataGridView trả sách.</summary>
    public sealed class ReturnLine
    {
        /// <summary>ID dòng chi tiết phiếu mượn gốc.</summary>
        public int BorrowDetailId { get; set; }
        /// <summary>ID sách.</summary>
        public int BookId { get; set; }
        /// <summary>Giá sách (cho công thức tính phạt hỏng/mất).</summary>
        public decimal BookPrice { get; set; }
        /// <summary>Tên sách hiển thị.</summary>
        public string Title { get; set; } = "";
        /// <summary>Đã mượn tổng cộng.</summary>
        public int BorrowedQty { get; set; }
        /// <summary>Đã trả trước đó.</summary>
        public int ReturnedQty { get; set; }
        /// <summary>Còn nợ = BorrowedQty − ReturnedQty.</summary>
        public int RemainingQty { get; set; }
        /// <summary>Số lượng trả lần này (user nhập).</summary>
        public int ReturnNow { get; set; }
        /// <summary>Tình trạng hiển thị (Tốt/Hỏng/Mất).</summary>
        public string ConditionDisplay { get; set; } = "Tốt";
        /// <summary>Tiền phạt tự tính.</summary>
        public decimal Fine { get; set; }
    }
}
