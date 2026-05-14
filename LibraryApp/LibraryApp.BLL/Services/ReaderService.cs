using LibraryApp.BLL.Common;
using LibraryApp.BLL.Interfaces;
using LibraryApp.BLL.Validation;

using LibraryApp.DAL.Common;
using LibraryApp.DAL.Interfaces;

using LibraryApp.Models;
using LibraryApp.Models.Enums;

using System.Data;

namespace LibraryApp.BLL.Services;

/// <summary>
/// Service quản lý độc giả. Validate dữ liệu thẻ, ngày hết hạn, định dạng email/sđt.
/// </summary>
public sealed class ReaderService : IReaderService
{
    private readonly IReaderRepository _repository;

    /// <summary>Khởi tạo với repository được inject.</summary>
    public ReaderService(IReaderRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc/>
    public List<Reader> GetAll() => _repository.GetAll();

    /// <inheritdoc/>
    public Reader? GetById(int id)
    {
        Validator.Positive(id, "ID độc giả");
        return _repository.GetById(id);
    }

    /// <inheritdoc/>
    public Reader? GetByCardNumber(string cardNumber)
    {
        Validator.NotEmpty(cardNumber, "Số thẻ");
        return _repository.GetByCardNumber(cardNumber.Trim());
    }

    /// <inheritdoc/>
    public List<Reader> Search(string? keyword, ReaderStatus? status)
        => _repository.Search(keyword?.Trim(), status);

    /// <inheritdoc/>
    public DataTable SearchAsDataTable(string? keyword, ReaderStatus? status)
        => _repository.SearchAsDataTable(keyword?.Trim(), status);

    /// <inheritdoc/>
    public int Create(Reader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ValidateCommon(reader);
        Validator.NotEmpty(reader.CardNumber, "Số thẻ");
        Validator.Length(reader.CardNumber, "Số thẻ", 2, 20);

        if (_repository.ExistsByCardNumber(reader.CardNumber))
            throw new BusinessException(nameof(reader.CardNumber),
                $"Số thẻ '{reader.CardNumber}' đã tồn tại.");

        return _repository.Insert(reader);
    }

    /// <inheritdoc/>
    public void Update(Reader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        Validator.Positive(reader.ReaderId, "ID độc giả");
        ValidateCommon(reader);

        if (!_repository.Update(reader))
            throw new BusinessException("Không tìm thấy độc giả để cập nhật.");
    }

    /// <inheritdoc/>
    public void Delete(int readerId)
    {
        Validator.Positive(readerId, "ID độc giả");

        try
        {
            if (!_repository.Delete(readerId))
                throw new BusinessException("Không tìm thấy độc giả để xoá.");
        }
        catch (DalException ex) when (ex.SqlErrorNumber == 50101)
        {
            throw new BusinessException(ex.Message, ex);
        }
    }

    /// <inheritdoc/>
    public void RenewCard(int readerId, DateOnly newExpireDate)
    {
        Validator.Positive(readerId, "ID độc giả");

        var reader = _repository.GetById(readerId)
            ?? throw new BusinessException("Không tìm thấy độc giả.");

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (newExpireDate <= today)
            throw new BusinessException("Ngày hết hạn mới phải sau ngày hôm nay.");
        if (newExpireDate <= reader.CardExpireDate)
            throw new BusinessException(
                "Ngày hết hạn mới phải sau ngày hết hạn hiện tại " +
                $"({reader.CardExpireDate:dd/MM/yyyy}).");

        // Tối đa gia hạn 3 năm/lần (chống nhập nhầm)
        if (newExpireDate.DayNumber - today.DayNumber > 365 * 3)
            throw new BusinessException("Mỗi lần gia hạn tối đa 3 năm.");

        reader.CardExpireDate = newExpireDate;
        reader.Status = ReaderStatus.Active;
        _repository.Update(reader);
    }

    // ----------------------------------------------------------------
    // Validate dùng chung cho Create + Update
    // ----------------------------------------------------------------

    private static void ValidateCommon(Reader r)
    {
        Validator.NotEmpty(r.FullName, "Họ tên");
        Validator.Length(r.FullName, "Họ tên", 2, 100);
        Validator.MaxLength(r.Address, "Địa chỉ", 200);
        Validator.Phone(r.Phone);
        Validator.Email(r.Email);

        // Ngày sinh hợp lý
        if (r.DateOfBirth.HasValue)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (r.DateOfBirth.Value > today)
                throw new BusinessException(nameof(r.DateOfBirth),
                    "Ngày sinh không được sau hôm nay.");
            if (r.DateOfBirth.Value.Year < 1900)
                throw new BusinessException(nameof(r.DateOfBirth),
                    "Ngày sinh không hợp lệ.");
        }

        // Ngày cấp / hết hạn thẻ
        Validator.DateRange(r.CardIssueDate, r.CardExpireDate,
            "Ngày cấp thẻ", "Ngày hết hạn");

        // Thẻ tối đa 5 năm/lần cấp
        if (r.CardExpireDate.DayNumber - r.CardIssueDate.DayNumber > 365 * 5)
            throw new BusinessException("Ngày hết hạn",
                "Thời hạn thẻ tối đa 5 năm tính từ ngày cấp.");
    }
}
