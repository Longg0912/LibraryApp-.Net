using System.Data;
using ClosedXML.Excel;

namespace LibraryApp.UI.Common;

/// <summary>
/// Helper xuất dữ liệu từ <see cref="DataGridView"/> hoặc <see cref="DataTable"/>
/// ra file Excel (.xlsx) sử dụng ClosedXML.
/// </summary>
/// <remarks>
/// Tất cả method đều hiện <c>SaveFileDialog</c> để user chọn nơi lưu rồi bắt
/// đầy đủ các loại exception thường gặp:
/// <list type="bullet">
/// <item><see cref="FileNotFoundException"/> / <see cref="TypeLoadException"/> —
/// khi ClosedXML chưa được restore (chưa <c>dotnet restore</c>).</item>
/// <item><see cref="IOException"/> — khi file đang được mở trong Excel hoặc thiếu quyền ghi.</item>
/// <item><see cref="UnauthorizedAccessException"/> — khi user chọn folder protected.</item>
/// </list>
///
/// Format file:
/// <list type="bullet">
/// <item>Header in đậm, nền xanh navy, chữ trắng — đồng bộ với theme app.</item>
/// <item>Tự auto-fit độ rộng cột.</item>
/// <item>Freeze pane hàng đầu (header) để scroll vẫn thấy header.</item>
/// <item>Banded rows (xen kẽ trắng / xám nhạt) cho dễ đọc.</item>
/// <item>Hàng tổng (footer) nếu được truyền.</item>
/// </list>
/// </remarks>
public static class ExcelExporter
{
    /// <summary>Màu nền header (xanh navy, đồng bộ theme app).</summary>
    private static readonly XLColor HeaderBackColor = XLColor.FromArgb(33, 64, 154);

    /// <summary>Màu nền hàng chẵn (xám rất nhạt).</summary>
    private static readonly XLColor BandedRowColor = XLColor.FromArgb(245, 247, 251);

    /// <summary>
    /// Xuất dữ liệu trong <see cref="DataGridView"/> ra Excel.
    /// </summary>
    /// <param name="grid">Grid nguồn (bỏ qua các cột <c>Visible = false</c>).</param>
    /// <param name="defaultFileName">
    /// Tên file gợi ý không có extension và timestamp. Method tự thêm
    /// <c>-yyyyMMdd-HHmmss.xlsx</c>.
    /// </param>
    /// <param name="sheetName">Tên sheet trong workbook (Excel giới hạn 31 ký tự).</param>
    /// <param name="title">
    /// Tiêu đề hiển thị ở dòng đầu file (in to, merge cells, optional).
    /// Truyền <c>null</c> để bỏ qua.
    /// </param>
    /// <param name="owner">Form chủ để hiện SaveFileDialog đúng vị trí.</param>
    /// <returns><c>true</c> nếu xuất thành công, <c>false</c> nếu user huỷ hoặc lỗi.</returns>
    public static bool ExportDataGridView(
        DataGridView grid,
        string defaultFileName,
        string sheetName = "Sheet1",
        string? title = null,
        IWin32Window? owner = null)
    {
        if (grid is null)
            throw new ArgumentNullException(nameof(grid));

        if (grid.Rows.Count == 0)
        {
            MessageBox.Show("Chưa có dữ liệu để xuất.\nVui lòng tìm kiếm/load dữ liệu trước.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        // Chuyển grid thành DataTable đơn giản — chỉ giữ cột Visible
        var dt = GridToDataTable(grid);
        return ExportDataTable(dt, defaultFileName, sheetName, title, owner);
    }

    /// <summary>
    /// Xuất một <see cref="DataTable"/> ra Excel.
    /// </summary>
    /// <param name="table">Bảng dữ liệu.</param>
    /// <param name="defaultFileName">Tên file gợi ý không có timestamp/extension.</param>
    /// <param name="sheetName">Tên sheet.</param>
    /// <param name="title">Tiêu đề ở dòng đầu (optional).</param>
    /// <param name="owner">Form chủ để hiển thị dialog.</param>
    /// <returns><c>true</c> nếu xuất thành công.</returns>
    public static bool ExportDataTable(
        DataTable table,
        string defaultFileName,
        string sheetName = "Sheet1",
        string? title = null,
        IWin32Window? owner = null)
    {
        if (table is null)
            throw new ArgumentNullException(nameof(table));

        if (table.Rows.Count == 0)
        {
            MessageBox.Show("Chưa có dữ liệu để xuất.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        // 1. Chọn nơi lưu
        string fileNameWithTimestamp = $"{defaultFileName}-{DateTime.Now:yyyyMMdd-HHmmss}.xlsx";
        using var dlg = new SaveFileDialog
        {
            Title = "Lưu file Excel",
            Filter = "Excel Workbook (*.xlsx)|*.xlsx|All files (*.*)|*.*",
            FileName = fileNameWithTimestamp,
            DefaultExt = "xlsx",
            AddExtension = true,
            OverwritePrompt = true
        };

        var dialogResult = owner is not null ? dlg.ShowDialog(owner) : dlg.ShowDialog();
        if (dialogResult != DialogResult.OK) return false;

        // 2. Tạo workbook + ghi file
        try
        {
            WriteWorkbook(table, dlg.FileName, sheetName, title);

            // 3. Thông báo + đề xuất mở file
            var openIt = MessageBox.Show(
                $"✓ Đã xuất {table.Rows.Count:N0} dòng ra:\n{dlg.FileName}\n\nBạn có muốn mở file ngay?",
                "Xuất Excel thành công",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (openIt == DialogResult.Yes)
                TryOpenFile(dlg.FileName);

            return true;
        }
        catch (FileNotFoundException ex) when (ex.FileName?.Contains("ClosedXML", StringComparison.OrdinalIgnoreCase) == true)
        {
            // Trường hợp DLL ClosedXML không có trong output (chưa restore)
            ShowLibraryMissingError();
            return false;
        }
        catch (TypeLoadException)
        {
            // ClosedXML compile được nhưng không load được runtime
            ShowLibraryMissingError();
            return false;
        }
        catch (IOException ex)
        {
            MessageBox.Show(
                $"Không thể ghi file vì file đang được mở hoặc không có quyền:\n\n{ex.Message}\n\n" +
                "Vui lòng đóng file trong Excel rồi thử lại.",
                "Lỗi ghi file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        catch (UnauthorizedAccessException ex)
        {
            MessageBox.Show(
                $"Không có quyền ghi file ở vị trí này:\n\n{ex.Message}\n\n" +
                "Vui lòng chọn thư mục khác (ví dụ Desktop hoặc Documents).",
                "Lỗi quyền truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Không thể xuất file Excel:\n\n{ex.GetType().Name}: {ex.Message}",
                "Lỗi xuất Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    // ================================================================
    // Internal - ghi workbook bằng ClosedXML
    // ================================================================

    /// <summary>
    /// Ghi workbook ra file. Tách hàm để dễ test và để bắt riêng các
    /// exception load assembly từ ClosedXML.
    /// </summary>
    private static void WriteWorkbook(DataTable table, string filePath,
        string sheetName, string? title)
    {
        // Sheet name có giới hạn 31 ký tự + không có ký tự đặc biệt
        if (sheetName.Length > 31) sheetName = sheetName[..31];
        sheetName = sheetName.Replace('/', '-').Replace('\\', '-')
                             .Replace('*', '-').Replace('?', '-')
                             .Replace(':', '-').Replace('[', '(').Replace(']', ')');

        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet(sheetName);

        int headerRow = 1;

        // ---- Title row ----
        if (!string.IsNullOrEmpty(title))
        {
            var titleCell = sheet.Cell(1, 1);
            titleCell.Value = title;
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontSize = 14;
            titleCell.Style.Font.FontColor = XLColor.FromArgb(33, 33, 33);
            titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            // Merge từ cột 1 đến cột cuối
            sheet.Range(1, 1, 1, table.Columns.Count).Merge();
            sheet.Row(1).Height = 28;

            // Hàng phụ: ngày xuất
            var subCell = sheet.Cell(2, 1);
            subCell.Value = $"Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            subCell.Style.Font.Italic = true;
            subCell.Style.Font.FontSize = 9;
            subCell.Style.Font.FontColor = XLColor.FromArgb(120, 120, 120);
            sheet.Range(2, 1, 2, table.Columns.Count).Merge();

            headerRow = 4;   // chừa hàng 3 trống
        }

        // ---- Header row ----
        for (int c = 0; c < table.Columns.Count; c++)
        {
            var col = table.Columns[c];
            var cell = sheet.Cell(headerRow, c + 1);
            cell.Value = col.ColumnName;
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = HeaderBackColor;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.BottomBorderColor = XLColor.White;
        }
        sheet.Row(headerRow).Height = 24;

        // ---- Data rows ----
        for (int r = 0; r < table.Rows.Count; r++)
        {
            var row = table.Rows[r];
            int sheetRow = headerRow + 1 + r;

            for (int c = 0; c < table.Columns.Count; c++)
            {
                var cell = sheet.Cell(sheetRow, c + 1);
                SetCellValue(cell, row[c], table.Columns[c].DataType);
            }

            // Banded rows (hàng chẵn có nền nhạt)
            if (r % 2 == 1)
            {
                sheet.Range(sheetRow, 1, sheetRow, table.Columns.Count)
                     .Style.Fill.BackgroundColor = BandedRowColor;
            }

            // Border dưới hàng
            sheet.Range(sheetRow, 1, sheetRow, table.Columns.Count)
                 .Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            sheet.Range(sheetRow, 1, sheetRow, table.Columns.Count)
                 .Style.Border.BottomBorderColor = XLColor.FromArgb(232, 234, 240);
        }

        // ---- Auto-fit + freeze pane ----
        sheet.Columns().AdjustToContents();

        // Giới hạn độ rộng cột (tránh cột text quá dài)
        foreach (var col in sheet.ColumnsUsed())
        {
            if (col.Width > 50) col.Width = 50;
            if (col.Width < 8) col.Width = 8;
        }

        sheet.SheetView.FreezeRows(headerRow);

        // Auto filter cho header
        var dataRange = sheet.Range(headerRow, 1,
            headerRow + table.Rows.Count, table.Columns.Count);
        dataRange.SetAutoFilter();

        // ---- Save ----
        workbook.SaveAs(filePath);
    }

    /// <summary>
    /// Set giá trị cell theo kiểu dữ liệu để Excel hiển thị đúng
    /// (số, ngày, decimal hiển thị format hơn là string thô).
    /// </summary>
    private static void SetCellValue(IXLCell cell, object? value, Type dataType)
    {
        if (value is null || value == DBNull.Value)
        {
            cell.Value = "";
            return;
        }

        // Map theo kiểu CLR
        switch (Type.GetTypeCode(dataType))
        {
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                cell.Value = Convert.ToInt64(value);
                cell.Style.NumberFormat.Format = "#,##0";
                break;

            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                cell.Value = Convert.ToDecimal(value);
                cell.Style.NumberFormat.Format = "#,##0.##";
                break;

            case TypeCode.DateTime:
                var dt = Convert.ToDateTime(value);
                cell.Value = dt;
                // Nếu giờ-phút = 00:00 → format date-only, ngược lại format datetime
                cell.Style.NumberFormat.Format = (dt.TimeOfDay == TimeSpan.Zero)
                    ? "dd/mm/yyyy" : "dd/mm/yyyy hh:mm";
                break;

            case TypeCode.Boolean:
                cell.Value = (bool)value ? "✓ Có" : "✗ Không";
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                break;

            default:
                // DateOnly là kiểu .NET 6+, không trả type code DateTime
                if (value is DateOnly dateOnly)
                {
                    cell.Value = dateOnly.ToDateTime(TimeOnly.MinValue);
                    cell.Style.NumberFormat.Format = "dd/mm/yyyy";
                }
                else if (value is TimeOnly timeOnly)
                {
                    cell.Value = timeOnly.ToTimeSpan();
                    cell.Style.NumberFormat.Format = "hh:mm:ss";
                }
                else
                {
                    cell.Value = value.ToString() ?? "";
                }
                break;
        }
    }

    // ================================================================
    // Helpers
    // ================================================================

    /// <summary>
    /// Chuyển <see cref="DataGridView"/> thành <see cref="DataTable"/>, chỉ
    /// giữ các cột <c>Visible = true</c> với tiêu đề (<c>HeaderText</c>).
    /// </summary>
    private static DataTable GridToDataTable(DataGridView grid)
    {
        var dt = new DataTable();

        // Chọn các cột visible, theo đúng thứ tự DisplayIndex
        var visibleCols = grid.Columns
            .Cast<DataGridViewColumn>()
            .Where(c => c.Visible)
            .OrderBy(c => c.DisplayIndex)
            .ToList();

        foreach (var col in visibleCols)
        {
            var displayName = string.IsNullOrEmpty(col.HeaderText) ? col.Name : col.HeaderText;
            // Tránh trùng tên cột (DataTable không cho phép)
            string uniqueName = displayName;
            int suffix = 1;
            while (dt.Columns.Contains(uniqueName))
                uniqueName = $"{displayName}_{++suffix}";

            // Kiểu dữ liệu: lấy từ ValueType của cột nếu có, fallback string
            var type = col.ValueType ?? typeof(string);
            // DataGridView đôi khi trả về Nullable<T>, nhưng DataTable cần T thật
            type = Nullable.GetUnderlyingType(type) ?? type;

            dt.Columns.Add(uniqueName, type);
        }

        foreach (DataGridViewRow row in grid.Rows)
        {
            if (row.IsNewRow) continue;
            var values = visibleCols
                .Select(c => row.Cells[c.Index].Value ?? DBNull.Value)
                .ToArray();

            // Convert sang đúng kiểu — tránh InvalidCastException khi cell có
            // value khác kiểu khai báo (vd: int? trả về null)
            var coercedValues = new object[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                var v = values[i];
                if (v is null or DBNull) { coercedValues[i] = DBNull.Value; continue; }

                try
                {
                    var targetType = dt.Columns[i].DataType;
                    coercedValues[i] = targetType == v.GetType()
                        ? v
                        : Convert.ChangeType(v, targetType);
                }
                catch
                {
                    // Fallback: lưu dưới dạng string
                    coercedValues[i] = v.ToString() ?? "";
                }
            }
            dt.Rows.Add(coercedValues);
        }

        return dt;
    }

    /// <summary>Hiển thị thông báo lỗi khi ClosedXML chưa được restore.</summary>
    private static void ShowLibraryMissingError()
    {
        MessageBox.Show(
            "Không tìm thấy thư viện ClosedXML.\n\n" +
            "Chức năng xuất Excel yêu cầu package 'ClosedXML' phải được cài đặt.\n" +
            "Vui lòng chạy lệnh sau trong terminal:\n\n" +
            "    dotnet restore\n\n" +
            "Hoặc trong Visual Studio: Right-click project → Restore NuGet Packages.",
            "Thiếu thư viện", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    /// <summary>
    /// Mở file vừa xuất bằng app mặc định (thường là Excel).
    /// Bắt lỗi nếu không có app nào được register cho .xlsx.
    /// </summary>
    private static void TryOpenFile(string filePath)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Không thể mở file tự động. Vui lòng mở thủ công.\n\n{ex.Message}",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
