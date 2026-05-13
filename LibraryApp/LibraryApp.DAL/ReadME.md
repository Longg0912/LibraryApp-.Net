# LibraryApp.DAL

Data Access Layer cho hệ thống quản lý thư viện (WinForms .NET 10 + SQL Server 2022).
Dùng **ADO.NET thuần** với `Microsoft.Data.SqlClient`, **không phụ thuộc EF Core**.

## Cấu trúc thư mục

```
LibraryApp.DAL/
├── LibraryApp.DAL.csproj
├── Common/
│   ├── DatabaseConnection.cs   - quản lý SqlConnection + connection pool
│   ├── DalException.cs         - exception riêng kèm SqlErrorNumber + Operation
│   └── DbExtensions.cs         - helper đọc DataReader (GetStringOrNull, GetEnum...)
├── Interfaces/
│   ├── IRepository.cs          - interface chung CRUD
│   ├── IBookRepository.cs
│   ├── ICategoryRepository.cs
│   ├── IReaderRepository.cs
│   ├── IUserRepository.cs
│   └── IBorrowReceiptRepository.cs
├── Mappers/                    - chuyển IDataReader -> entity
│   ├── BookMapper.cs
│   ├── CategoryMapper.cs
│   ├── ReaderMapper.cs
│   ├── UserMapper.cs
│   └── BorrowReceiptMapper.cs
└── Repositories/
    ├── BaseRepository.cs       - chứa Execute() bọc exception
    ├── BookRepository.cs
    ├── CategoryRepository.cs
    ├── ReaderRepository.cs
    ├── UserRepository.cs
    └── BorrowReceiptRepository.cs
```

## Nguyên tắc thiết kế

### 1. Mọi tham số đều qua `SqlParameter` — không nối chuỗi
Tuyệt đối không có `"WHERE Name = '" + name + "'"` trong codebase. Tất cả input
từ UI đều đi qua `cmd.Parameters.Add()` để chống SQL injection triệt để.

### 2. Hai cách trả dữ liệu: `List<T>` hoặc `DataTable`
- **`List<T>`**: dùng khi cần thao tác trên object (binding form chi tiết, BLL logic).
- **`DataTable`**: dùng khi chỉ cần hiển thị DataGridView. Nhanh hơn ~30% vì
  không tạo object trung gian. Mọi repository có search đều có cả 2 phương thức:
  `Search()` và `SearchAsDataTable()`.

### 3. Thao tác phức tạp uỷ thác cho stored procedure
- `BookRepository.Insert/Update/Delete` → gọi `sp_Book_Insert/Update/Delete`.
- `BorrowReceiptRepository.CreateBorrow` → gọi `sp_Borrow_Create` (có `UPDLOCK + HOLDLOCK`).
- `BorrowReceiptRepository.CreateReturn` → gọi `sp_Return_Create`.

Stored procedure đảm nhận transaction, race condition, audit log → tầng C# gọn.

### 4. Mọi exception SQL được dịch sang tiếng Việt
`BaseRepository.Execute()` bọc mọi thao tác trong try-catch và dịch:

| Mã lỗi | Thông báo tiếng Việt |
|---|---|
| 50010 | Mã sách đã tồn tại |
| 50012 | Dữ liệu đã bị người khác thay đổi |
| 50020 | Độc giả không hợp lệ hoặc thẻ đã hết hạn |
| 50022 | Một hoặc nhiều sách không đủ tồn kho |
| 2627 / 2601 | Dữ liệu bị trùng |
| 547 | Vi phạm khoá ngoại |
| -2 | Truy vấn quá thời gian |

Tầng UI/BLL chỉ cần `catch (DalException ex)` và `MessageBox.Show(ex.Message)`.

### 5. Hỗ trợ Table-Valued Parameter (TVP)
`CreateBorrow` và `CreateReturn` truyền danh sách sách qua TVP
(`dbo.BorrowItemList`, `dbo.ReturnItemList`) để insert nhiều dòng trong 1 round-trip.

### 6. Optimistic concurrency
`BookRepository.Update` gửi `RowVersion` lên `sp_Book_Update` → SP từ chối nếu
dữ liệu đã thay đổi (lỗi 50012). Tầng UI cần catch và hiển thị "Vui lòng tải lại".

## Cách dùng từ BLL

```csharp
// Khởi tạo (thường qua DI nếu dùng Microsoft.Extensions.DependencyInjection)
IBookRepository bookRepo = new BookRepository();

try
{
    // Tìm kiếm trả List<T>
    var books = bookRepo.Search(
        keyword: "C#",
        categoryId: 1,
        status: BookStatus.Available,
        yearFrom: 2020,
        yearTo: null);

    // Tìm kiếm trả DataTable cho DataGridView
    var table = bookRepo.SearchAsDataTable(null, 1, null, null, null);
    dgvBooks.DataSource = table;
}
catch (DalException ex)
{
    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    // Nếu cần biết loại lỗi cụ thể:
    if (ex.SqlErrorNumber == 50012)
    {
        // Reload data...
    }
}
```

## Cách dùng lập phiếu mượn

```csharp
var receipt = new BorrowReceipt
{
    ReceiptCode = $"PM{DateTime.Now:yyyyMMddHHmmss}",
    ReaderId    = selectedReader.ReaderId,
    UserId      = currentUser.UserId,
    BorrowDate  = DateOnly.FromDateTime(DateTime.Today),
    DueDate     = DateOnly.FromDateTime(DateTime.Today.AddDays(14)),
    Note        = txtNote.Text
};

var items = cart.Select(c => new BorrowReceiptDetail
{
    BookId = c.BookId,
    Quantity = c.Quantity
});

try
{
    int newId = borrowRepo.CreateBorrow(receipt, items);
    MessageBox.Show($"Lập phiếu thành công. Mã phiếu: {receipt.ReceiptCode}");
}
catch (DalException ex) when (ex.SqlErrorNumber == 50022)
{
    MessageBox.Show("Có sách đã hết, vui lòng chọn lại.");
}
```

## NuGet packages

```xml
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.2.2" />
<PackageReference Include="System.Configuration.ConfigurationManager" Version="9.0.0" />
```
