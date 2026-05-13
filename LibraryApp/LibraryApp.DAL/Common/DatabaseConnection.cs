using System.Configuration;
using Microsoft.Data.SqlClient;

namespace LibraryApp.DAL;

/// <summary>
/// Lớp tiện ích quản lý kết nối tới SQL Server cho toàn bộ ứng dụng WinForms.
/// <para>
/// Cung cấp các phương thức tạo, mở, kiểm tra kết nối an toàn theo chuẩn ADO.NET
/// (<see cref="SqlConnection"/>) với chuỗi kết nối được lấy từ <c>App.config</c>
/// (mục <c>&lt;connectionStrings&gt;</c>). Tất cả các phương thức đều ném
/// <see cref="DatabaseConnectionException"/> với thông báo rõ ràng bằng tiếng Việt
/// khi gặp sự cố, giúp tầng UI hiển thị lỗi thân thiện cho người dùng.
/// </para>
/// <para>
/// Lớp này KHÔNG giữ kết nối singleton: mỗi lần gọi <see cref="CreateConnection"/>
/// hoặc <see cref="OpenConnection"/> sẽ tạo một <see cref="SqlConnection"/> mới và
/// trông cậy vào <i>Connection Pooling</i> tích hợp sẵn của ADO.NET để tái sử dụng
/// physical connection — đây là cách dùng đúng và an toàn nhất cho ứng dụng đa form.
/// </para>
/// </summary>
/// <remarks>
/// Cách dùng điển hình ở tầng repository:
/// <code>
/// using var conn = DatabaseConnection.OpenConnection();
/// using var cmd  = new SqlCommand("SELECT * FROM dbo.Books", conn);
/// using var rdr  = cmd.ExecuteReader();
/// while (rdr.Read()) { /* ... */ }
/// </code>
/// </remarks>
public static class DatabaseConnection
{
    /// <summary>
    /// Tên của connection string trong <c>App.config</c>. Có thể thay đổi tại đây
    /// nếu dự án dùng nhiều môi trường (DEV/UAT/PROD).
    /// </summary>
    private const string DefaultConnectionName = "LibraryDb";

    /// <summary>
    /// Cache chuỗi kết nối sau lần đọc đầu tiên để tránh truy xuất
    /// <see cref="ConfigurationManager"/> nhiều lần.
    /// </summary>
    private static string? _cachedConnectionString;

    /// <summary>
    /// Khoá đồng bộ cho thao tác đọc/ghi cache chuỗi kết nối, đảm bảo an toàn
    /// khi nhiều form khởi tạo song song lúc khởi động ứng dụng.
    /// </summary>
    private static readonly Lock _syncRoot = new();

    /// <summary>
    /// Lấy chuỗi kết nối đang sử dụng. Đọc từ <c>App.config</c> lần đầu rồi cache lại.
    /// </summary>
    /// <exception cref="DatabaseConnectionException">
    /// Khi không tìm thấy connection string trong cấu hình hoặc chuỗi rỗng.
    /// </exception>
    public static string ConnectionString
    {
        get
        {
            if (_cachedConnectionString is not null)
                return _cachedConnectionString;

            lock (_syncRoot)
            {
                if (_cachedConnectionString is not null)
                    return _cachedConnectionString;

                var setting = ConfigurationManager.ConnectionStrings[DefaultConnectionName];
                if (setting is null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                {
                    throw new DatabaseConnectionException(
                        $"Không tìm thấy chuỗi kết nối có tên '{DefaultConnectionName}' " +
                        "trong App.config. Vui lòng kiểm tra mục <connectionStrings>.");
                }

                _cachedConnectionString = setting.ConnectionString;
                return _cachedConnectionString;
            }
        }
    }

    /// <summary>
    /// Tạo một <see cref="SqlConnection"/> mới ở trạng thái CHƯA mở.
    /// Người gọi có trách nhiệm <c>Open()</c> và bọc trong <c>using</c>
    /// để đảm bảo connection được trả về pool.
    /// </summary>
    /// <returns>Một <see cref="SqlConnection"/> đã được khởi tạo với chuỗi kết nối.</returns>
    /// <exception cref="DatabaseConnectionException">
    /// Khi không đọc được chuỗi kết nối từ cấu hình.
    /// </exception>
    public static SqlConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }

    /// <summary>
    /// Tạo và mở một <see cref="SqlConnection"/> sẵn sàng để thực thi truy vấn.
    /// Người gọi vẫn phải đảm bảo gọi <c>Dispose()</c> (thường qua <c>using</c>).
    /// </summary>
    /// <returns>Một <see cref="SqlConnection"/> đã ở trạng thái <see cref="System.Data.ConnectionState.Open"/>.</returns>
    /// <exception cref="DatabaseConnectionException">
    /// Khi không thể mở kết nối — ví dụ sai server, sai mật khẩu, SQL Server không chạy,
    /// firewall chặn, hoặc database không tồn tại.
    /// </exception>
    public static SqlConnection OpenConnection()
    {
        var conn = CreateConnection();
        try
        {
            conn.Open();
            return conn;
        }
        catch (SqlException ex)
        {
            conn.Dispose();
            throw new DatabaseConnectionException(BuildFriendlyMessage(ex), ex);
        }
        catch (InvalidOperationException ex)
        {
            conn.Dispose();
            throw new DatabaseConnectionException(
                "Chuỗi kết nối không hợp lệ. Vui lòng kiểm tra lại cấu hình App.config.", ex);
        }
    }

    /// <summary>
    /// Phiên bản bất đồng bộ của <see cref="OpenConnection"/>. Khuyến nghị dùng
    /// trong các handler <c>async</c> của WinForms để tránh treo UI thread.
    /// </summary>
    /// <param name="cancellationToken">Token huỷ thao tác mở kết nối.</param>
    /// <returns>Một <see cref="SqlConnection"/> đã mở thành công.</returns>
    /// <exception cref="DatabaseConnectionException">
    /// Khi không thể mở kết nối.
    /// </exception>
    public static async Task<SqlConnection> OpenConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        var conn = CreateConnection();
        try
        {
            await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
            return conn;
        }
        catch (SqlException ex)
        {
            await conn.DisposeAsync().ConfigureAwait(false);
            throw new DatabaseConnectionException(BuildFriendlyMessage(ex), ex);
        }
        catch (InvalidOperationException ex)
        {
            await conn.DisposeAsync().ConfigureAwait(false);
            throw new DatabaseConnectionException(
                "Chuỗi kết nối không hợp lệ. Vui lòng kiểm tra lại cấu hình App.config.", ex);
        }
    }

    /// <summary>
    /// Kiểm tra nhanh xem có kết nối được tới database hay không.
    /// Thường được gọi ở form đăng nhập hoặc <c>FrmMain.Load</c> để báo sớm cho người dùng
    /// thay vì đợi đến khi mở DataGridView mới phát hiện.
    /// </summary>
    /// <returns><c>true</c> nếu kết nối thành công; <c>false</c> nếu thất bại.</returns>
    public static bool TestConnection()
    {
        try
        {
            using var conn = OpenConnection();
            return conn.State == System.Data.ConnectionState.Open;
        }
        catch (DatabaseConnectionException)
        {
            return false;
        }
    }

    /// <summary>
    /// Đóng và giải phóng một <see cref="SqlConnection"/> một cách an toàn,
    /// nuốt mọi exception phát sinh trong quá trình đóng (để không che mất
    /// exception gốc của thao tác chính). Thường dùng trong khối <c>finally</c>
    /// khi không thể bọc <c>using</c>.
    /// </summary>
    /// <param name="connection">Kết nối cần đóng. Có thể là <c>null</c>.</param>
    public static void SafeClose(SqlConnection? connection)
    {
        if (connection is null) return;

        try
        {
            if (connection.State != System.Data.ConnectionState.Closed)
                connection.Close();
        }
        catch
        {
            // Cố ý nuốt: tránh che lấp exception gốc trong khối finally.
        }
        finally
        {
            connection.Dispose();
        }
    }

    /// <summary>
    /// Reset cache chuỗi kết nối. Hữu ích khi cấu hình thay đổi lúc runtime
    /// (ví dụ: form Settings cho phép đổi server và lưu lại App.config).
    /// </summary>
    public static void ResetConnectionStringCache()
    {
        lock (_syncRoot)
        {
            _cachedConnectionString = null;
            ConfigurationManager.RefreshSection("connectionStrings");
        }
    }

    /// <summary>
    /// Dịch các <see cref="SqlException"/> phổ biến thành thông báo tiếng Việt
    /// thân thiện. Các mã lỗi tham khảo từ tài liệu Microsoft Docs.
    /// </summary>
    private static string BuildFriendlyMessage(SqlException ex)
    {
        return ex.Number switch
        {
            // Network / server không phản hồi
            -2 => "Kết nối tới SQL Server bị quá thời gian. Vui lòng kiểm tra mạng hoặc địa chỉ server.",
            53 => "Không tìm thấy SQL Server trên mạng. Vui lòng kiểm tra tên server trong App.config.",
            17 => "SQL Server không tồn tại hoặc đã từ chối kết nối. Kiểm tra dịch vụ SQL Server đã chạy chưa.",
            // Xác thực
            18456 => "Sai tên đăng nhập hoặc mật khẩu SQL Server. Vui lòng kiểm tra thông tin xác thực.",
            // Database
            4060 => "Không thể mở database được chỉ định. Vui lòng kiểm tra tên database trong App.config.",
            // Permission
            229 => "Tài khoản hiện tại không có quyền truy cập đối tượng yêu cầu.",
            // Fallback
            _ => $"Lỗi SQL Server ({ex.Number}): {ex.Message}"
        };
    }
}

/// <summary>
/// Exception riêng cho mọi sự cố liên quan tới kết nối database trong ứng dụng.
/// Tầng UI chỉ cần bắt loại exception này để hiển thị thông báo phù hợp,
/// không cần xử lý trực tiếp <see cref="SqlException"/>.
/// </summary>
public sealed class DatabaseConnectionException : Exception
{
    /// <summary>
    /// Khởi tạo exception với thông báo lỗi.
    /// </summary>
    public DatabaseConnectionException(string message) : base(message) { }

    /// <summary>
    /// Khởi tạo exception với thông báo lỗi và exception gốc.
    /// </summary>
    public DatabaseConnectionException(string message, Exception innerException)
        : base(message, innerException) { }
}