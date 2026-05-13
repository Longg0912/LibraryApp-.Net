namespace LibraryApp.Models.Enums;

/// <summary>
/// Trạng thái của một đầu sách trong hệ thống. Giá trị enum được serialize
/// thành chuỗi khi lưu vào cột <c>Books.Status</c> (kiểu <c>VARCHAR(20)</c>)
/// nên tên thành viên phải khớp tuyệt đối với CHECK constraint trong database.
/// </summary>
public enum BookStatus
{
    /// <summary>Sách đang sẵn có trong kho và có thể cho mượn.</summary>
    Available,

    /// <summary>Tạm thời hết tồn kho (đã được mượn hết).</summary>
    OutOfStock,

    /// <summary>Sách đã bị thất lạc.</summary>
    Lost,

    /// <summary>Sách bị hỏng, không cho mượn được nữa.</summary>
    Damaged,

    /// <summary>Sách đã được rút khỏi lưu thông (xoá mềm).</summary>
    Retired
}