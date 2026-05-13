using System.Data;
using LibraryApp.Models;
using LibraryApp.Models.Enums;

namespace LibraryApp.DAL.Interfaces;

/// <summary>
/// Repository cho entity <see cref="Reader"/>.
/// </summary>
public interface IReaderRepository : IRepository<Reader, int>
{
    /// <summary>
    /// Tìm độc giả theo từ khoá (tên / số thẻ / sđt) và/hoặc trạng thái.
    /// </summary>
    List<Reader> Search(string? keyword, ReaderStatus? status);

    /// <summary>Trả về <see cref="DataTable"/> để bind DataGridView.</summary>
    DataTable SearchAsDataTable(string? keyword, ReaderStatus? status);

    /// <summary>Lấy độc giả theo số thẻ (form tìm nhanh, scan barcode).</summary>
    Reader? GetByCardNumber(string cardNumber);

    /// <summary>Kiểm tra số thẻ đã tồn tại.</summary>
    bool ExistsByCardNumber(string cardNumber);
}
