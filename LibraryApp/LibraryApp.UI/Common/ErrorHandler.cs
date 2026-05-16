using Microsoft.Data.SqlClient;
using LibraryApp.BLL.Common;
using LibraryApp.DAL.Common;

namespace LibraryApp.UI.Common;

/// <summary>
/// Bộ xử lý exception trung tâm cho toàn bộ UI. Tự phân loại exception
/// và hiển thị MessageBox với thông báo tiếng Việt phù hợp + ghi log.
/// </summary>
/// <remarks>
/// Mục đích: thay vì mỗi try-catch ở UI viết lại logic phân loại exception,
/// chỉ cần gọi <c>ErrorHandler.Handle(ex)</c> hoặc <c>ErrorHandler.TryRun(() =&gt; ...)</c>.
///
/// Ưu tiên thông báo:
/// <list type="number">
/// <item><see cref="BusinessException"/> — lỗi nghiệp vụ → Warning icon, hiện <c>Message</c>.</item>
/// <item><see cref="DalException"/> — lỗi DAL có <c>SqlErrorNumber</c> → message đã được dịch sẵn.</item>
/// <item><see cref="SqlException"/> — lỗi SQL Server thô → phân loại theo error number.</item>
/// <item><see cref="InvalidOperationException"/> với connection-string error → hướng dẫn sửa App.config.</item>
/// <item><see cref="UnauthorizedAccessException"/>, <see cref="IOException"/> → lỗi file/quyền.</item>
/// <item>Mặc định → Error icon, hiện <c>GetType + Message</c>.</item>
/// </list>
/// </remarks>
public static class ErrorHandler
{
    /// <summary>
    /// Phân loại exception và hiển thị MessageBox phù hợp. Cũng ghi log với
    /// stack trace đầy đủ. Method này <b>không ném lại exception</b>.
    /// </summary>
    /// <param name="ex">Exception cần xử lý.</param>
    /// <param name="context">
    /// Mô tả ngữ cảnh (ví dụ: "Khi đang lưu sách", "Lập phiếu mượn") để hiển thị
    /// trong MessageBox + ghi vào log. Có thể null.
    /// </param>
    /// <param name="owner">Form chủ để MessageBox hiện đúng vị trí.</param>
    public static void Handle(Exception ex, string? context = null, IWin32Window? owner = null)
    {
        ArgumentNullException.ThrowIfNull(ex);

        // Ghi log trước (ngay cả khi MessageBox bị suppress vì lý do nào đó)
        Logger.Error(string.IsNullOrEmpty(context)
            ? $"Unhandled exception: {ex.GetType().Name}"
            : $"{context}: {ex.GetType().Name}", ex);

        // Phân loại
        switch (ex)
        {
            case BusinessException be:
                ShowMessage(owner,
                    be.Message,
                    "Lỗi nghiệp vụ",
                    MessageBoxIcon.Warning);
                break;

            case DalException de:
                HandleDalException(de, context, owner);
                break;

            case SqlException sqlEx:
                HandleSqlException(sqlEx, context, owner);
                break;

            case InvalidOperationException ioe when IsConnectionStringError(ioe):
                ShowMessage(owner,
                    "Cấu hình kết nối CSDL không đúng.\n\n" +
                    $"Chi tiết: {ioe.Message}\n\n" +
                    "Vui lòng kiểm tra App.config — thẻ <connectionStrings name=\"LibraryDb\">.",
                    "Lỗi cấu hình kết nối",
                    MessageBoxIcon.Error);
                break;

            case TimeoutException:
                ShowMessage(owner,
                    "Truy vấn quá thời gian chờ.\n\n" +
                    "Mạng có thể bị chậm hoặc CSDL đang quá tải. Vui lòng thử lại.",
                    "Hết thời gian chờ",
                    MessageBoxIcon.Warning);
                break;

            case UnauthorizedAccessException uae:
                ShowMessage(owner,
                    $"Không có quyền truy cập:\n\n{uae.Message}",
                    "Lỗi quyền truy cập",
                    MessageBoxIcon.Warning);
                break;

            case IOException ioex:
                ShowMessage(owner,
                    $"Lỗi truy cập file:\n\n{ioex.Message}\n\n" +
                    "File có thể đang được mở bởi chương trình khác.",
                    "Lỗi file",
                    MessageBoxIcon.Warning);
                break;

            case ArgumentException ae:
                ShowMessage(owner,
                    $"Tham số không hợp lệ:\n\n{ae.Message}",
                    "Lỗi nhập liệu",
                    MessageBoxIcon.Warning);
                break;

            default:
                ShowMessage(owner,
                    BuildGenericMessage(ex, context),
                    "Lỗi hệ thống",
                    MessageBoxIcon.Error);
                break;
        }
    }

    /// <summary>
    /// Thực thi một action trong try-catch tự động, chuyển mọi exception
    /// về <see cref="Handle"/>. Trả về <c>true</c> nếu thành công.
    /// </summary>
    /// <example>
    /// <code>
    /// if (!ErrorHandler.TryRun(() =&gt; {
    ///     ServiceLocator.Books.Create(book);
    /// }, context: "Khi đang lưu sách", owner: this))
    /// {
    ///     return; // có lỗi, đã hiển thị MessageBox
    /// }
    /// </code>
    /// </example>
    public static bool TryRun(Action action, string? context = null, IWin32Window? owner = null)
    {
        ArgumentNullException.ThrowIfNull(action);
        try
        {
            action();
            return true;
        }
        catch (Exception ex)
        {
            Handle(ex, context, owner);
            return false;
        }
    }

    /// <summary>
    /// Tương tự <see cref="TryRun(Action, string?, IWin32Window?)"/> nhưng có giá trị trả về.
    /// </summary>
    public static bool TryRun<T>(Func<T> func, out T? result,
        string? context = null, IWin32Window? owner = null)
    {
        ArgumentNullException.ThrowIfNull(func);
        try
        {
            result = func();
            return true;
        }
        catch (Exception ex)
        {
            Handle(ex, context, owner);
            result = default;
            return false;
        }
    }

    // ================================================================
    // Internal handlers
    // ================================================================

    private static void HandleDalException(DalException ex, string? context, IWin32Window? owner)
    {
        // DAL đã dịch sẵn message — chỉ cần phân loại icon theo error number
        var icon = ex.SqlErrorNumber switch
        {
            // Errors trong range custom 5xxxx là lỗi nghiệp vụ ném từ stored procedure
            >= 50000 and < 60000 => MessageBoxIcon.Warning,
            _ => MessageBoxIcon.Error
        };

        var title = ex.SqlErrorNumber >= 50000 && ex.SqlErrorNumber < 60000
            ? "Vi phạm quy tắc dữ liệu"
            : "Lỗi truy cập dữ liệu";

        ShowMessage(owner, ex.Message, title, icon);
    }

    /// <summary>
    /// Xử lý <see cref="SqlException"/> thô. Phân loại theo error number của SQL Server
    /// để hiển thị thông báo tiếng Việt thân thiện (đặc biệt với lỗi kết nối thường gặp).
    /// </summary>
    private static void HandleSqlException(SqlException ex, string? context, IWin32Window? owner)
    {
        // Các error number quan trọng của SQL Server
        var (message, icon) = ex.Number switch
        {
            // Connection errors
            -2 or 11 or 121 or 258 => (
                "Không thể kết nối tới SQL Server (timeout).\n\n" +
                "Vui lòng kiểm tra:\n" +
                "• SQL Server service đang chạy\n" +
                "• Tường lửa cho phép port 1433\n" +
                "• Tên server trong App.config đúng",
                MessageBoxIcon.Error),

            17 or 53 or 67 or 233 or 6 => (
                "Không tìm thấy SQL Server hoặc không truy cập được.\n\n" +
                $"Chi tiết: {ex.Message}\n\n" +
                "Vui lòng kiểm tra tên server, instance name trong App.config.",
                MessageBoxIcon.Error),

            // Authentication
            18456 => (
                "Đăng nhập SQL Server thất bại.\n\n" +
                "Tên đăng nhập hoặc mật khẩu trong connection string không đúng,\n" +
                "hoặc tài khoản Windows không có quyền truy cập database.",
                MessageBoxIcon.Error),

            // Database not found
            4060 or 911 => (
                "Database không tồn tại hoặc không truy cập được.\n\n" +
                "Vui lòng kiểm tra:\n" +
                "• Database 'HUST_Library_DEV' đã được tạo chưa\n" +
                "• Đã chạy script HUST_Library_Production.sql chưa\n" +
                "• Tài khoản có quyền truy cập database này không",
                MessageBoxIcon.Error),

            // Permission denied
            229 or 230 or 297 => (
                "Tài khoản không có quyền thực hiện thao tác này.\n\n" +
                $"Chi tiết: {ex.Message}",
                MessageBoxIcon.Warning),

            // Deadlock
            1205 => (
                "Xung đột dữ liệu (deadlock) — thao tác đã bị huỷ.\n\n" +
                "Vui lòng thử lại sau giây lát.",
                MessageBoxIcon.Warning),

            // Constraint violation - duplicate key
            2627 or 2601 => (
                "Dữ liệu vi phạm ràng buộc duy nhất.\n\n" +
                "Có thể bạn đang thêm trùng mã/khoá đã tồn tại. Vui lòng kiểm tra lại.",
                MessageBoxIcon.Warning),

            // Foreign key violation
            547 => (
                "Dữ liệu vi phạm ràng buộc khoá ngoại.\n\n" +
                "Bản ghi này đang được tham chiếu bởi dữ liệu khác. " +
                "Vui lòng xoá các tham chiếu trước.",
                MessageBoxIcon.Warning),

            // Custom errors thrown from stored procedures
            >= 50000 and < 60000 => (
                ex.Message,  // Stored procedure đã trả message tiếng Việt
                MessageBoxIcon.Warning),

            // Default
            _ => (
                $"Lỗi SQL Server (mã {ex.Number}):\n\n{ex.Message}",
                MessageBoxIcon.Error)
        };

        ShowMessage(owner, message,
            icon == MessageBoxIcon.Warning ? "Cảnh báo" : "Lỗi kết nối CSDL",
            icon);
    }

    private static bool IsConnectionStringError(InvalidOperationException ex)
    {
        var msg = ex.Message.ToLowerInvariant();
        return msg.Contains("connection") || msg.Contains("connectionstring");
    }

    private static string BuildGenericMessage(Exception ex, string? context)
    {
        var sb = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(context))
            sb.AppendLine($"Ngữ cảnh: {context}\n");
        sb.AppendLine("Đã xảy ra lỗi không mong muốn.");
        sb.AppendLine();
        sb.AppendLine($"Loại lỗi: {ex.GetType().Name}");
        sb.AppendLine($"Chi tiết: {ex.Message}");
        sb.AppendLine();
        sb.AppendLine("Chi tiết kỹ thuật đã được ghi vào file log trong thư mục 'logs/'.");
        return sb.ToString();
    }

    private static void ShowMessage(IWin32Window? owner, string text, string title, MessageBoxIcon icon)
    {
        if (owner is not null)
            MessageBox.Show(owner, text, title, MessageBoxButtons.OK, icon);
        else
            MessageBox.Show(text, title, MessageBoxButtons.OK, icon);
    }
}
