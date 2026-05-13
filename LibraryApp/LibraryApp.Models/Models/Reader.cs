using LibraryApp.Models.Enums;

namespace LibraryApp.Models;

/// <summary>
/// Độc giả của thư viện. Một độc giả tương ứng với một thẻ thư viện, có thể
/// có nhiều phiếu mượn xuyên suốt thời gian sử dụng.
/// Mapping với bảng <c>dbo.Readers</c>.
/// </summary>
public sealed class Reader : BaseAuditableEntity
{
    /// <summary>
    /// Khoá chính. Mapping với cột <c>ReaderId</c>.
    /// </summary>
    public int ReaderId { get; set; }

    /// <summary>
    /// Số thẻ thư viện duy nhất (ví dụ: <c>TV001</c>). In trên thẻ vật lý
    /// và dùng làm khoá nghiệp vụ khi tra cứu nhanh.
    /// </summary>
    public required string CardNumber { get; set; }

    /// <summary>
    /// Họ và tên đầy đủ của độc giả.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// Ngày sinh. Dùng <see cref="DateOnly"/> để khớp đúng kiểu <c>DATE</c>
    /// của SQL Server (không có thành phần giờ).
    /// </summary>
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>Giới tính.</summary>
    public Gender? Gender { get; set; }

    /// <summary>Địa chỉ liên hệ.</summary>
    public string? Address { get; set; }

    /// <summary>Số điện thoại.</summary>
    public string? Phone { get; set; }

    /// <summary>Email liên hệ.</summary>
    public string? Email { get; set; }

    /// <summary>
    /// Ngày cấp thẻ.
    /// </summary>
    public DateOnly CardIssueDate { get; set; }

    /// <summary>
    /// Ngày thẻ hết hạn. Phải >= <see cref="CardIssueDate"/>.
    /// </summary>
    public DateOnly CardExpireDate { get; set; }

    /// <summary>
    /// Trạng thái thẻ tại thời điểm hiện tại.
    /// </summary>
    public ReaderStatus Status { get; set; } = ReaderStatus.Active;

    // ---------- Computed properties ----------

    /// <summary>
    /// Cho biết thẻ còn hiệu lực để mượn sách hay không, xét cả trạng thái
    /// và ngày hết hạn so với ngày hôm nay.
    /// </summary>
    public bool IsCardValid =>
        Status == ReaderStatus.Active
        && !IsDeleted
        && CardExpireDate >= DateOnly.FromDateTime(DateTime.Today);

    /// <summary>
    /// Tuổi hiện tại của độc giả (tính theo năm). Trả về <c>null</c> nếu
    /// chưa nhập ngày sinh.
    /// </summary>
    public int? Age
    {
        get
        {
            if (!DateOfBirth.HasValue) return null;
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - DateOfBirth.Value.Year;
            if (DateOfBirth.Value > today.AddYears(-age)) age--;
            return age;
        }
    }

    /// <summary>
    /// Hiển thị "Số thẻ – Họ tên" cho ComboBox khi lập phiếu mượn.
    /// </summary>
    public override string ToString() => $"{CardNumber} - {FullName}";
}