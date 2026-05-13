namespace LibraryApp.Models;

/// <summary>
/// Lớp cơ sở chứa các thuộc tính audit chung cho tất cả các entity nghiệp vụ
/// (sách, độc giả, người dùng, phiếu mượn, phiếu trả...).
/// <para>
/// Mỗi entity kế thừa lớp này sẽ tự động có đủ thông tin để theo vết:
/// ai tạo, ai sửa, ai xoá, khi nào, và một dấu thời gian (<see cref="RowVersion"/>)
/// phục vụ cơ chế <i>optimistic concurrency</i> với cột <c>ROWVERSION</c> trong SQL Server.
/// </para>
/// <para>
/// Lưu ý: lớp này KHÔNG khai báo cột <c>Id</c> chung, vì mỗi bảng trong database
/// dùng tên khoá chính riêng (<c>BookId</c>, <c>UserId</c>...) để Dapper map trực tiếp
/// theo tên cột mà không cần attribute bổ sung.
/// </para>
/// </summary>
public abstract class BaseAuditableEntity
{
    /// <summary>
    /// Thời điểm bản ghi được tạo (giờ UTC).
    /// Mặc định được cấu hình bởi <c>DEFAULT SYSUTCDATETIME()</c> phía SQL Server.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Mã người dùng tạo bản ghi. Có thể <c>null</c> nếu bản ghi được tạo bởi
    /// hệ thống (ví dụ: dữ liệu seed ban đầu).
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// Thời điểm cập nhật gần nhất (giờ UTC). <c>null</c> nếu chưa từng được sửa.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Mã người dùng cập nhật bản ghi lần gần nhất.
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// Cờ xoá mềm. <c>true</c> nghĩa là bản ghi đã bị xoá khỏi giao diện
    /// nhưng vẫn còn trong database để phục vụ truy vết và phục hồi.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Thời điểm xoá mềm bản ghi.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Mã người dùng thực hiện xoá mềm bản ghi.
    /// </summary>
    public int? DeletedBy { get; set; }

    /// <summary>
    /// Dấu thời gian phiên bản hàng (mapping với cột <c>RowVer ROWVERSION</c>).
    /// Dùng cho optimistic concurrency: khi cập nhật, client gửi giá trị này
    /// lên server và stored procedure sẽ từ chối nếu giá trị không còn khớp,
    /// đảm bảo dữ liệu không bị ghi đè bởi thao tác song song.
    /// </summary>
    public byte[]? RowVersion { get; set; }
}