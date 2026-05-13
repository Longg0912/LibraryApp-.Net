namespace LibraryApp.DAL.Common;

/// <summary>
/// Exception riêng cho mọi sự cố xảy ra trong tầng Data Access Layer.
/// <para>
/// Mục đích: tách lỗi kỹ thuật phía dưới (<see cref="Microsoft.Data.SqlClient.SqlException"/>,
/// <see cref="InvalidOperationException"/>...) thành thông báo nghiệp vụ mà tầng UI/BLL
/// có thể hiển thị trực tiếp cho người dùng cuối, đồng thời giữ lại exception gốc
/// trong <see cref="Exception.InnerException"/> để phục vụ debug.
/// </para>
/// </summary>
public sealed class DalException : Exception
{
    /// <summary>
    /// Mã lỗi SQL gốc (nếu có). Hữu ích khi tầng BLL muốn phân nhánh xử lý
    /// theo lỗi nghiệp vụ tùy chỉnh (ví dụ: <c>THROW 50020</c> từ stored procedure).
    /// </summary>
    public int? SqlErrorNumber { get; }

    /// <summary>
    /// Tên thao tác đang thực hiện khi xảy ra lỗi (ví dụ: "GetById", "Insert").
    /// </summary>
    public string? Operation { get; }

    /// <summary>
    /// Khởi tạo exception với thông báo tiếng Việt.
    /// </summary>
    public DalException(string message) : base(message) { }

    /// <summary>
    /// Khởi tạo exception với thông báo và exception gốc.
    /// </summary>
    public DalException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Khởi tạo đầy đủ kèm mã lỗi SQL và tên thao tác.
    /// </summary>
    public DalException(string message, Exception innerException, int? sqlErrorNumber, string? operation)
        : base(message, innerException)
    {
        SqlErrorNumber = sqlErrorNumber;
        Operation = operation;
    }
}
