using System.Data;
using LibraryApp.Models;
using LibraryApp.Models.Enums;

namespace LibraryApp.BLL.Interfaces;

/// <summary>Service quản lý độc giả.</summary>
public interface IReaderService
{
    /// <summary>Lấy toàn bộ độc giả chưa bị xoá.</summary>
    List<Reader> GetAll();

    /// <summary>Lấy chi tiết một độc giả theo ID.</summary>
    Reader? GetById(int id);

    /// <summary>Tra cứu nhanh theo số thẻ (form quét barcode).</summary>
    Reader? GetByCardNumber(string cardNumber);

    /// <summary>Tìm kiếm theo từ khoá (tên/số thẻ/sđt) và trạng thái.</summary>
    List<Reader> Search(string? keyword, ReaderStatus? status);

    /// <summary>Tìm kiếm trả về DataTable.</summary>
    DataTable SearchAsDataTable(string? keyword, ReaderStatus? status);

    /// <summary>Thêm độc giả mới. Validate đầy đủ.</summary>
    int Create(Reader reader);

    /// <summary>Cập nhật độc giả.</summary>
    void Update(Reader reader);

    /// <summary>Xoá mềm độc giả. Không cho xoá nếu còn sách đang mượn.</summary>
    void Delete(int readerId);

    /// <summary>Cấp lại thẻ (gia hạn) cho độc giả.</summary>
    void RenewCard(int readerId, DateOnly newExpireDate);
}
