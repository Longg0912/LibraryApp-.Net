namespace LibraryApp.Models.Enums;

/// <summary>
/// Giới tính độc giả. Mapping với cột <c>Readers.Gender</c> (kiểu <c>NVARCHAR(10)</c>).
/// Khi lưu xuống database, các giá trị tương ứng là "Nam", "Nữ", "Khác".
/// </summary>
public enum Gender
{
    /// <summary>Nam.</summary>
    Male,

    /// <summary>Nữ.</summary>
    Female,

    /// <summary>Giới tính khác hoặc không xác định.</summary>
    Other
}