using LibraryApp.DAL;
using LibraryApp.DAL.Common;
using LibraryApp.Models;
using LibraryApp.Models.Enums;
using LibraryApp.UI.Common;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace LibraryApp.UI.UserControls;

/// <summary>
/// UserControl tìm kiếm nâng cao đa thực thể.
/// <para>
/// Cho phép tìm kiếm trên một trong ba thực thể: Sách, Độc giả, Phiếu mượn,
/// với 2+ tiêu chí kết hợp cùng lúc. UI tự ẩn/hiện các field không liên quan
/// đến entity được chọn (ví dụ: ô "Tác giả" và "Năm XB" chỉ hiện khi tìm Sách).
/// </para>
/// </summary>
/// <remarks>
/// Truy vấn SQL được build động trong code-behind này và gửi qua ADO.NET
/// với <see cref="SqlParameter"/> để chống SQL injection. Các điều kiện
/// dùng <c>LIKE N'%' + @Param + N'%'</c> với collation
/// <c>Vietnamese_CI_AS</c> (đã set ở DB level) nên tìm kiếm tự động
/// <b>không phân biệt hoa/thường và dấu</b>.
/// </remarks>
public partial class UcAdvancedSearch : UserControl
{
    /// <summary>Entity đang được tìm kiếm.</summary>
    private enum SearchEntity { Books, Readers, BorrowReceipts }

    /// <summary>Khởi tạo UC.</summary>
    public UcAdvancedSearch()
    {
        InitializeComponent();
        Load += UcAdvancedSearch_Load;
    }

    // ================================================================
    // Lifecycle
    // ================================================================

    private void UcAdvancedSearch_Load(object? sender, EventArgs e)
    {
        try
        {
            BindEntityCombo();
            BindCategoryCombo();
            cboEntity.SelectedIndex = 0;   // mặc định tìm Sách
            UpdateUiByEntity();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    // ================================================================
    // Binding ComboBox
    // ================================================================

    private void BindEntityCombo()
    {
        var entities = new List<EntityItem>
        {
            new(SearchEntity.Books,          "📚 Sách"),
            new(SearchEntity.Readers,        "👥 Độc giả"),
            new(SearchEntity.BorrowReceipts, "🔄 Phiếu mượn")
        };
        cboEntity.DataSource = entities;
        cboEntity.DisplayMember = nameof(EntityItem.Display);
        cboEntity.ValueMember = nameof(EntityItem.Value);
    }

    private void BindCategoryCombo()
    {
        try
        {
            var categories = ServiceLocator.Categories.GetActive();
            var items = new List<Category>
            {
                new() { CategoryId = 0, CategoryCode = "", CategoryName = "-- Tất cả --" }
            };
            items.AddRange(categories);

            cboCategory.DataSource = items;
            cboCategory.DisplayMember = nameof(Category.CategoryName);
            cboCategory.ValueMember = nameof(Category.CategoryId);
            cboCategory.SelectedIndex = 0;
        }
        catch
        {
            // Không fail toàn form chỉ vì combo danh mục không tải được
        }
    }

    private void BindBookStatusCombo()
    {
        var items = new List<StatusItem>
        {
            new(null, "-- Tất cả --"),
            new(BookStatus.Available,  "Đang sẵn có"),
            new(BookStatus.OutOfStock, "Hết tồn kho"),
            new(BookStatus.Lost,       "Đã mất"),
            new(BookStatus.Damaged,    "Bị hỏng"),
            new(BookStatus.Retired,    "Ngừng lưu thông")
        };
        cboStatus.DataSource = items;
        cboStatus.DisplayMember = nameof(StatusItem.Display);
        cboStatus.ValueMember = nameof(StatusItem.Value);
        cboStatus.SelectedIndex = 0;
    }

    private void BindReaderStatusCombo()
    {
        var items = new List<StatusItem>
        {
            new(null, "-- Tất cả --"),
            new(ReaderStatus.Active,  "Đang hoạt động"),
            new(ReaderStatus.Locked,  "Bị khoá"),
            new(ReaderStatus.Expired, "Hết hạn")
        };
        cboStatus.DataSource = items;
        cboStatus.DisplayMember = nameof(StatusItem.Display);
        cboStatus.ValueMember = nameof(StatusItem.Value);
        cboStatus.SelectedIndex = 0;
    }

    private void BindBorrowStatusCombo()
    {
        var items = new List<StatusItem>
        {
            new(null, "-- Tất cả --"),
            new(BorrowStatus.Borrowing,         "Đang mượn"),
            new(BorrowStatus.PartiallyReturned, "Trả 1 phần"),
            new(BorrowStatus.Returned,          "Đã trả hết"),
            new(BorrowStatus.Overdue,           "Quá hạn"),
            new(BorrowStatus.Cancelled,         "Đã huỷ")
        };
        cboStatus.DataSource = items;
        cboStatus.DisplayMember = nameof(StatusItem.Display);
        cboStatus.ValueMember = nameof(StatusItem.Value);
        cboStatus.SelectedIndex = 0;
    }

    private sealed record EntityItem(SearchEntity Value, string Display);
    private sealed record StatusItem(object? Value, string Display);

    // ================================================================
    // UI mode switching
    // ================================================================

    /// <summary>
    /// Ẩn/hiện các ô tiêu chí tuỳ entity được chọn:
    /// - Books: hiện Category, Author, Year range
    /// - Readers: chỉ Keyword + Status (đủ rồi)
    /// - BorrowReceipts: hiện Date range
    /// </summary>
    private void UpdateUiByEntity()
    {
        var entity = (cboEntity.SelectedItem as EntityItem)?.Value ?? SearchEntity.Books;

        switch (entity)
        {
            case SearchEntity.Books:
                BindBookStatusCombo();
                lblKeywordHint.Text = "Tìm theo Tên / Mã sách / Tác giả";

                // Row 2 - hiện
                lblCategory.Visible = true;
                cboCategory.Visible = true;
                lblAuthor.Visible = true;
                txtAuthor.Visible = true;

                // Row 3 - hiện (year range)
                lblYearRange.Visible = true;
                numYearFrom.Visible = true;
                lblYearSeparator.Visible = true;
                numYearTo.Visible = true;

                // Row 4 - ẩn (date range)
                lblDateRange.Visible = false;
                dtpDateFrom.Visible = false;
                lblDateSeparator.Visible = false;
                dtpDateTo.Visible = false;
                chkAllDates.Visible = false;
                break;

            case SearchEntity.Readers:
                BindReaderStatusCombo();
                lblKeywordHint.Text = "Tìm theo Họ tên / Số thẻ / Số điện thoại / Email";

                // Ẩn các tiêu chí không liên quan
                lblCategory.Visible = false;
                cboCategory.Visible = false;
                lblAuthor.Visible = false;
                txtAuthor.Visible = false;
                lblYearRange.Visible = false;
                numYearFrom.Visible = false;
                lblYearSeparator.Visible = false;
                numYearTo.Visible = false;
                lblDateRange.Visible = false;
                dtpDateFrom.Visible = false;
                lblDateSeparator.Visible = false;
                dtpDateTo.Visible = false;
                chkAllDates.Visible = false;
                break;

            case SearchEntity.BorrowReceipts:
                BindBorrowStatusCombo();
                lblKeywordHint.Text = "Tìm theo Mã phiếu / Số thẻ độc giả / Họ tên độc giả";

                // Ẩn category / author / year
                lblCategory.Visible = false;
                cboCategory.Visible = false;
                lblAuthor.Visible = false;
                txtAuthor.Visible = false;
                lblYearRange.Visible = false;
                numYearFrom.Visible = false;
                lblYearSeparator.Visible = false;
                numYearTo.Visible = false;

                // Hiện date range
                lblDateRange.Visible = true;
                dtpDateFrom.Visible = true;
                lblDateSeparator.Visible = true;
                dtpDateTo.Visible = true;
                chkAllDates.Visible = true;
                break;
        }

        // Xoá kết quả + đặt count về 0 khi đổi entity
        dgvResults.DataSource = null;
        lblResultCount.Text = "";
    }

    private void cboEntity_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateUiByEntity();
    }

    private void chkAllDates_CheckedChanged(object? sender, EventArgs e)
    {
        dtpDateFrom.Enabled = !chkAllDates.Checked;
        dtpDateTo.Enabled = !chkAllDates.Checked;
    }

    // ================================================================
    // Search execution - SQL build động
    // ================================================================

    private void btnSearch_Click(object? sender, EventArgs e) => DoSearch();

    private void txtKeyword_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            DoSearch();
        }
    }

    /// <summary>
    /// Thực thi tìm kiếm dựa trên entity và các tiêu chí đã chọn.
    /// Build SQL động + dùng SqlParameter cho mọi giá trị.
    /// </summary>
    private void DoSearch()
    {
        try
        {
            UseWaitCursor = true;
            var entity = (cboEntity.SelectedItem as EntityItem)?.Value ?? SearchEntity.Books;

            DataTable result = entity switch
            {
                SearchEntity.Books => SearchBooks(),
                SearchEntity.Readers => SearchReaders(),
                SearchEntity.BorrowReceipts => SearchBorrows(),
                _ => new DataTable()
            };

            dgvResults.DataSource = result;
            lblResultCount.Text = $"Tìm thấy {result.Rows.Count:N0} kết quả.";
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
    /// Tìm kiếm sách với 5 tiêu chí: keyword, category, status, author, year range.
    /// Build điều kiện WHERE động — chỉ thêm clause khi tiêu chí có giá trị.
    /// </summary>
    private DataTable SearchBooks()
    {
        var sql = new StringBuilder("""
            SELECT b.BookCode,
                   b.Title,
                   b.Author,
                   c.CategoryName,
                   b.Publisher,
                   b.PublishYear,
                   b.Quantity,
                   b.AvailableQty,
                   b.Status
            FROM dbo.Books b
            JOIN dbo.Categories c ON c.CategoryId = b.CategoryId
            WHERE b.IsDeleted = 0
            """);

        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand { Connection = conn };

        // -- Keyword (Title / BookCode / Author) --
        if (!string.IsNullOrWhiteSpace(txtKeyword.Text))
        {
            sql.AppendLine("""
                  AND (b.Title    LIKE N'%' + @Keyword + N'%'
                    OR b.BookCode LIKE       @Keyword + '%'
                    OR b.Author   LIKE N'%' + @Keyword + N'%')
                """);
            cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 200).Value = txtKeyword.Text.Trim();
        }

        // -- Category --
        if (cboCategory.SelectedValue is int catId && catId > 0)
        {
            sql.AppendLine("  AND b.CategoryId = @CategoryId");
            cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = catId;
        }

        // -- Status --
        if (cboStatus.SelectedItem is StatusItem si && si.Value is BookStatus bookStatus)
        {
            sql.AppendLine("  AND b.Status = @Status");
            cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = bookStatus.ToString();
        }

        // -- Author (search bổ sung ngoài keyword) --
        if (!string.IsNullOrWhiteSpace(txtAuthor.Text))
        {
            sql.AppendLine("  AND b.Author LIKE N'%' + @Author + N'%'");
            cmd.Parameters.Add("@Author", SqlDbType.NVarChar, 150).Value = txtAuthor.Text.Trim();
        }

        // -- Year range (chỉ áp dụng nếu user thay đổi khỏi default 1500-2100) --
        if (numYearFrom.Value > numYearFrom.Minimum)
        {
            sql.AppendLine("  AND b.PublishYear >= @YearFrom");
            cmd.Parameters.Add("@YearFrom", SqlDbType.Int).Value = (int)numYearFrom.Value;
        }
        if (numYearTo.Value < numYearTo.Maximum)
        {
            sql.AppendLine("  AND b.PublishYear <= @YearTo");
            cmd.Parameters.Add("@YearTo", SqlDbType.Int).Value = (int)numYearTo.Value;
        }

        sql.AppendLine("ORDER BY b.Title;");
        cmd.CommandText = sql.ToString();
        return Fill(cmd);
    }

    /// <summary>
    /// Tìm kiếm độc giả với 2 tiêu chí: keyword + status.
    /// Keyword tìm trên 4 trường: FullName, CardNumber, Phone, Email.
    /// </summary>
    private DataTable SearchReaders()
    {
        var sql = new StringBuilder("""
            SELECT r.CardNumber,
                   r.FullName,
                   r.DateOfBirth,
                   r.Gender,
                   r.Phone,
                   r.Email,
                   r.CardIssueDate,
                   r.CardExpireDate,
                   r.Status
            FROM dbo.Readers r
            WHERE r.IsDeleted = 0
            """);

        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand { Connection = conn };

        if (!string.IsNullOrWhiteSpace(txtKeyword.Text))
        {
            sql.AppendLine("""
                  AND (r.FullName   LIKE N'%' + @Keyword + N'%'
                    OR r.CardNumber LIKE       @Keyword + '%'
                    OR r.Phone      LIKE       @Keyword + '%'
                    OR r.Email      LIKE       @Keyword + '%')
                """);
            cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 100).Value = txtKeyword.Text.Trim();
        }

        if (cboStatus.SelectedItem is StatusItem si && si.Value is ReaderStatus readerStatus)
        {
            sql.AppendLine("  AND r.Status = @Status");
            cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = readerStatus.ToString();
        }

        sql.AppendLine("ORDER BY r.FullName;");
        cmd.CommandText = sql.ToString();
        return Fill(cmd);
    }

    /// <summary>
    /// Tìm kiếm phiếu mượn theo: keyword (mã phiếu/số thẻ/họ tên độc giả),
    /// status, khoảng ngày mượn.
    /// </summary>
    private DataTable SearchBorrows()
    {
        var sql = new StringBuilder("""
            SELECT br.ReceiptCode,
                   r.CardNumber,
                   r.FullName AS ReaderName,
                   u.FullName AS LibrarianName,
                   br.BorrowDate,
                   br.DueDate,
                   br.Status,
                   br.TotalFine
            FROM dbo.BorrowReceipts br
            JOIN dbo.Readers r ON r.ReaderId = br.ReaderId
            JOIN dbo.Users   u ON u.UserId   = br.UserId
            WHERE br.IsDeleted = 0
            """);

        using var conn = DatabaseConnection.OpenConnection();
        using var cmd = new SqlCommand { Connection = conn };

        if (!string.IsNullOrWhiteSpace(txtKeyword.Text))
        {
            sql.AppendLine("""
                  AND (br.ReceiptCode LIKE       @Keyword + '%'
                    OR r.CardNumber   LIKE       @Keyword + '%'
                    OR r.FullName     LIKE N'%' + @Keyword + N'%')
                """);
            cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 100).Value = txtKeyword.Text.Trim();
        }

        if (cboStatus.SelectedItem is StatusItem si && si.Value is BorrowStatus borrowStatus)
        {
            sql.AppendLine("  AND br.Status = @Status");
            cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = borrowStatus.ToString();
        }

        if (!chkAllDates.Checked)
        {
            sql.AppendLine("  AND br.BorrowDate >= @From AND br.BorrowDate <= @To");
            cmd.Parameters.Add("@From", SqlDbType.Date).Value = dtpDateFrom.Value.Date;
            cmd.Parameters.Add("@To", SqlDbType.Date).Value = dtpDateTo.Value.Date;
        }

        sql.AppendLine("ORDER BY br.BorrowDate DESC;");
        cmd.CommandText = sql.ToString();
        return Fill(cmd);
    }

    private static DataTable Fill(SqlCommand cmd)
    {
        using var reader = cmd.ExecuteReader();
        var dt = new DataTable();
        dt.Load(reader);
        return dt;
    }

    // ================================================================
    // Reset + Export
    // ================================================================

    private void btnReset_Click(object? sender, EventArgs e)
    {
        txtKeyword.Clear();
        txtAuthor.Clear();
        if (cboCategory.Items.Count > 0) cboCategory.SelectedIndex = 0;
        if (cboStatus.Items.Count > 0) cboStatus.SelectedIndex = 0;
        numYearFrom.Value = numYearFrom.Minimum;
        numYearTo.Value = numYearTo.Maximum;
        dtpDateFrom.Value = DateTime.Today.AddMonths(-1);
        dtpDateTo.Value = DateTime.Today;
        chkAllDates.Checked = false;

        dgvResults.DataSource = null;
        lblResultCount.Text = "";
    }

    /// <summary>
    /// Xuất kết quả hiện tại ra file CSV. Đây là chức năng tiện ích cho báo cáo
    /// (gửi qua email, mở bằng Excel, v.v.).
    /// </summary>
    private void btnExport_Click(object? sender, EventArgs e)
    {
        if (dgvResults.DataSource is not DataTable dt || dt.Rows.Count == 0)
        {
            MessageBox.Show("Chưa có dữ liệu để xuất. Vui lòng tìm kiếm trước.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new SaveFileDialog
        {
            Filter = "CSV file (*.csv)|*.csv",
            FileName = $"search-result-{DateTime.Now:yyyyMMdd-HHmmss}.csv"
        };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        try
        {
            using var writer = new StreamWriter(dlg.FileName, false, Encoding.UTF8);
            // BOM cho Excel hiển thị tiếng Việt đúng
            writer.Write('\uFEFF');

            // Header
            var headers = dt.Columns.Cast<DataColumn>().Select(c => Escape(c.ColumnName));
            writer.WriteLine(string.Join(",", headers));

            // Rows
            foreach (DataRow row in dt.Rows)
            {
                var values = row.ItemArray.Select(v => Escape(v?.ToString() ?? ""));
                writer.WriteLine(string.Join(",", values));
            }

            MessageBox.Show($"Đã xuất {dt.Rows.Count:N0} dòng ra:\n{dlg.FileName}",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }

        static string Escape(string s)
        {
            if (s.Contains(',') || s.Contains('"') || s.Contains('\n'))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }
    }

    private static void ShowError(Exception ex)
    {
        MessageBox.Show(ex.Message, "Lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
