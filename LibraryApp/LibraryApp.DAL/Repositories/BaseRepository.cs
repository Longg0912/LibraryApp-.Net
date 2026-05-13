using System.Data;
using Microsoft.Data.SqlClient;
using LibraryApp.DAL.Common;

namespace LibraryApp.DAL.Repositories;

/// <summary>
/// Lớp cơ sở chứa các helper dùng chung cho mọi repository: mở connection,
/// gói exception <see cref="SqlException"/> thành <see cref="DalException"/>
/// với thông báo tiếng Việt, build <see cref="DataTable"/> từ reader.
/// </summary>
public abstract class BaseRepository
{
    /// <summary>
    /// Tạo và mở một <see cref="SqlConnection"/> mới qua
    /// <see cref="DatabaseConnection"/>.
    /// </summary>
    protected static SqlConnection OpenConnection() => DatabaseConnection.OpenConnection();

    /// <summary>
    /// Thực thi một hành động đọc/ghi database, bọc mọi <see cref="SqlException"/>
    /// và <see cref="InvalidOperationException"/> thành <see cref="DalException"/>
    /// có thông báo thân thiện kèm tên thao tác.
    /// </summary>
    /// <typeparam name="T">Kiểu trả về của hành động.</typeparam>
    /// <param name="operation">Tên thao tác (cho log/debug).</param>
    /// <param name="action">Hành động cần thực hiện.</param>
    protected static T Execute<T>(string operation, Func<T> action)
    {
        try
        {
            return action();
        }
        catch (DalException)
        {
            // Không bọc lại exception cùng loại — tránh stack message lồng nhau
            throw;
        }
        catch (DatabaseConnectionException ex)
        {
            // Lỗi kết nối ở tầng DatabaseConnection — chuyển thành DalException
            // để tầng UI chỉ cần bắt một loại exception duy nhất.
            throw new DalException(ex.Message, ex, null, operation);
        }
        catch (SqlException ex)
        {
            throw new DalException(BuildFriendlyMessage(ex, operation), ex, ex.Number, operation);
        }
        catch (InvalidOperationException ex)
        {
            throw new DalException($"Lỗi truy cập dữ liệu ({operation}): {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Phiên bản void của <see cref="Execute{T}"/>.
    /// </summary>
    protected static void Execute(string operation, Action action)
    {
        Execute<object?>(operation, () => { action(); return null; });
    }

    /// <summary>
    /// Đọc toàn bộ kết quả của một <see cref="SqlCommand"/> thành <see cref="List{T}"/>
    /// bằng hàm map được truyền vào.
    /// </summary>
    protected static List<T> ReadList<T>(SqlCommand cmd, Func<IDataReader, T> mapper)
    {
        using var reader = cmd.ExecuteReader();
        var list = new List<T>();
        while (reader.Read())
            list.Add(mapper(reader));
        return list;
    }

    /// <summary>
    /// Đọc kết quả của <see cref="SqlCommand"/> vào <see cref="DataTable"/>
    /// (phục vụ binding trực tiếp vào <c>DataGridView.DataSource</c>).
    /// </summary>
    protected static DataTable ReadDataTable(SqlCommand cmd)
    {
        using var reader = cmd.ExecuteReader();
        var table = new DataTable();
        table.Load(reader);
        return table;
    }

    /// <summary>
    /// Dịch mã lỗi SQL Server / lỗi SP nghiệp vụ (THROW 50xxx) sang thông báo tiếng Việt.
    /// </summary>
    private static string BuildFriendlyMessage(SqlException ex, string operation)
    {
        // Các mã RAISERROR/THROW nghiệp vụ trong stored procedure (xem script SQL)
        return ex.Number switch
        {
            // Book
            50010 => "Mã sách đã tồn tại trong hệ thống.",
            50011 => "Không tìm thấy sách hoặc đã bị xoá.",
            50012 => "Dữ liệu đã bị người khác thay đổi. Vui lòng tải lại và thử lại.",
            50013 => "Không thể giảm số lượng xuống thấp hơn số sách đang được mượn.",
            50014 => "Sách đang được mượn, không thể xoá.",
            50015 => "Bản ghi không tồn tại hoặc đã bị xoá trước đó.",

            // Borrow
            50020 => "Độc giả không hợp lệ hoặc thẻ thư viện đã hết hạn.",
            50021 => "Vượt quá số sách tối đa được phép mượn cùng lúc.",
            50022 => "Một hoặc nhiều sách không đủ tồn kho hoặc không khả dụng.",

            // Return
            50030 => "Phiếu mượn không tồn tại.",
            50031 => "Số lượng trả không hợp lệ hoặc dòng không thuộc phiếu mượn đã chọn.",

            // SQL Server chung
            2627 or 2601 => "Dữ liệu bị trùng (mã đã tồn tại). Vui lòng kiểm tra lại.",
            547 => "Không thể thực hiện do ràng buộc dữ liệu (khoá ngoại).",
            -2 => "Truy vấn database quá thời gian cho phép. Vui lòng thử lại.",

            _ => $"Lỗi truy cập dữ liệu khi {operation}: {ex.Message}"
        };
    }
}
