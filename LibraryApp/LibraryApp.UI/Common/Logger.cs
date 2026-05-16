using System.Text;

namespace LibraryApp.UI.Common;

/// <summary>
/// Logger đơn giản ghi log vào file theo ngày dưới thư mục <c>logs/</c>.
/// </summary>
/// <remarks>
/// Dùng cho mục đích chẩn đoán lỗi production. Không thay thế cho một logging
/// framework đầy đủ như Serilog, NLog — nhưng đủ cho bài tập lớn và môi trường
/// nội bộ. Có 4 mức log: Debug, Info, Warning, Error.
///
/// File log nằm cạnh exe: <c>{AppDir}\logs\YYYY-MM-DD.log</c>.
/// Mỗi dòng có format: <c>[HH:mm:ss] [LEVEL] message</c>.
///
/// Tất cả method <b>không bao giờ ném exception</b> — nếu không ghi được log,
/// silent fail. Việc log thất bại không được phép làm crash app.
/// </remarks>
public static class Logger
{
    /// <summary>Đường dẫn thư mục lưu log (cạnh exe).</summary>
    private static readonly string LogDirectory =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

    /// <summary>Lock object cho thread-safety khi nhiều thread cùng ghi log.</summary>
    private static readonly Lock _lock = new();

    /// <summary>Mức tối thiểu được ghi. Có thể đổi qua <see cref="SetMinLevel"/>.</summary>
    private static LogLevel _minLevel = LogLevel.Info;

    /// <summary>Đặt mức log tối thiểu. Log dưới mức này bị bỏ qua.</summary>
    public static void SetMinLevel(LogLevel level) => _minLevel = level;

    /// <summary>Ghi log mức Debug (chi tiết, chỉ cho dev).</summary>
    public static void Debug(string message) => Write(LogLevel.Debug, message, null);

    /// <summary>Ghi log mức Info (sự kiện bình thường).</summary>
    public static void Info(string message) => Write(LogLevel.Info, message, null);

    /// <summary>Ghi log mức Warning (vấn đề không nghiêm trọng).</summary>
    public static void Warning(string message) => Write(LogLevel.Warning, message, null);

    /// <summary>Ghi log mức Error có kèm exception (stack trace đầy đủ).</summary>
    public static void Error(string message, Exception? ex = null)
        => Write(LogLevel.Error, message, ex);

    // ----------------------------------------------------------------
    // Internal
    // ----------------------------------------------------------------

    private static void Write(LogLevel level, string message, Exception? ex)
    {
        if (level < _minLevel) return;

        try
        {
            // Build dòng log
            var sb = new StringBuilder();
            sb.Append($"[{DateTime.Now:HH:mm:ss}] ");
            sb.Append($"[{level.ToString().ToUpperInvariant(),-7}] ");
            sb.Append(message);

            if (ex is not null)
            {
                sb.AppendLine();
                sb.AppendLine($"  Exception: {ex.GetType().FullName}");
                sb.AppendLine($"  Message:   {ex.Message}");
                if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    sb.AppendLine("  Stack:");
                    foreach (var stackline in ex.StackTrace.Split('\n'))
                        sb.AppendLine($"    {stackline.TrimEnd()}");
                }

                // Inner exceptions
                var inner = ex.InnerException;
                int depth = 1;
                while (inner is not null && depth <= 3)
                {
                    sb.AppendLine($"  Inner #{depth}: {inner.GetType().Name}: {inner.Message}");
                    inner = inner.InnerException;
                    depth++;
                }
            }

            var line = sb.ToString();

            // Ghi file thread-safe
            lock (_lock)
            {
                if (!Directory.Exists(LogDirectory))
                    Directory.CreateDirectory(LogDirectory);

                var logFile = Path.Combine(LogDirectory, $"{DateTime.Now:yyyy-MM-dd}.log");
                File.AppendAllText(logFile, line + Environment.NewLine, Encoding.UTF8);
            }
        }
        catch
        {
            // Log thất bại → silent. Không được phép crash app vì lỗi log.
        }
    }
}

/// <summary>Mức ưu tiên log.</summary>
public enum LogLevel
{
    /// <summary>Thông tin debug chi tiết, chỉ dùng khi dev.</summary>
    Debug = 0,
    /// <summary>Sự kiện bình thường (login, save thành công...).</summary>
    Info = 1,
    /// <summary>Cảnh báo - không nghiêm trọng nhưng cần chú ý.</summary>
    Warning = 2,
    /// <summary>Lỗi nghiệp vụ hoặc kỹ thuật.</summary>
    Error = 3
}
