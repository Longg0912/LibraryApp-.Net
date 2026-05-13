using LibraryApp.Models.Enums;

namespace LibraryApp.Models;

/// <summary>
/// Đầu sách trong thư viện. Một bản ghi đại diện cho một tựa sách (theo mã sách),
/// có thể có nhiều bản copy được phản ánh qua <see cref="Quantity"/>.
/// Mapping với bảng <c>dbo.Books</c>.
/// </summary>
public sealed class Book : BaseAuditableEntity
{
    /// <summary>
    /// Khoá chính. Mapping với cột <c>BookId</c>.
    /// </summary>
    public int BookId { get; set; }

    /// <summary>
    /// Mã sách nội bộ duy nhất (barcode hoặc mã tự đặt, ví dụ: <c>B001</c>).
    /// Dùng để in lên gáy sách và quét lúc mượn/trả.
    /// </summary>
    public required string BookCode { get; set; }

    /// <summary>
    /// Tên sách (tiêu đề).
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Tác giả. Trong thiết kế đơn giản, lưu dưới dạng chuỗi. Nếu cần nhiều tác giả
    /// cho một sách, hãy chuẩn hoá thành bảng <c>Authors</c> và bảng nối.
    /// </summary>
    public required string Author { get; set; }

    /// <summary>Nhà xuất bản.</summary>
    public string? Publisher { get; set; }

    /// <summary>Năm xuất bản (phạm vi cho phép: 1500–2100, kiểm tra bằng CHECK constraint).</summary>
    public int? PublishYear { get; set; }

    /// <summary>Khoá ngoại tới <see cref="Category"/>.</summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Tổng số bản copy mà thư viện sở hữu. Không bao giờ âm.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Số bản copy hiện đang sẵn có (chưa được ai mượn).
    /// Luôn nằm trong khoảng <c>[0, Quantity]</c>.
    /// </summary>
    public int AvailableQty { get; set; }

    /// <summary>
    /// Giá tham chiếu của một bản sách (VNĐ). Dùng để tính tiền đền bù khi mất.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Trạng thái lưu thông của sách. Khi <see cref="Status"/> không phải
    /// <see cref="BookStatus.Available"/>, sách không được phép xuất hiện
    /// trên phiếu mượn mới.
    /// </summary>
    public BookStatus Status { get; set; } = BookStatus.Available;

    // ---------- Navigation property ----------

    /// <summary>
    /// Đối tượng danh mục tương ứng (nạp khi truy vấn JOIN).
    /// </summary>
    public Category? Category { get; set; }

    // ---------- Computed properties ----------

    /// <summary>
    /// Số bản đang được mượn = <see cref="Quantity"/> - <see cref="AvailableQty"/>.
    /// Tiện cho binding DataGridView mà không cần truy vấn riêng.
    /// </summary>
    public int BorrowedQty => Quantity - AvailableQty;

    /// <summary>
    /// Cho biết sách có thể cho mượn ngay tại thời điểm này hay không.
    /// </summary>
    public bool CanBeBorrowed =>
        Status == BookStatus.Available && AvailableQty > 0 && !IsDeleted;

    /// <summary>
    /// Hiển thị "Mã – Tiêu đề" trên ComboBox khi lập phiếu mượn.
    /// </summary>
    public override string ToString() => $"{BookCode} - {Title}";
}