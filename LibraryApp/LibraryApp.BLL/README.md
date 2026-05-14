# LibraryApp.BLL

Business Logic Layer cho hệ thống quản lý thư viện.
Tách hoàn toàn khỏi UI; chỉ phụ thuộc `LibraryApp.Models` và `LibraryApp.DAL`.

## Cấu trúc

```
LibraryApp.BLL/
├── LibraryApp.BLL.csproj
├── Common/
│   ├── BusinessException.cs        - exception nghiệp vụ + FieldName
│   └── ValidationResult.cs         - kết quả validate nhiều field
├── Validation/
│   └── Validator.cs                - NotEmpty / Length / Email / Phone / Password...
├── Dtos/
│   ├── LoginResult.cs              - kết quả Login (Success, Message, MustChangePassword)
│   └── DashboardKpi.cs             - KPI tổng quan
├── Interfaces/
│   ├── IAuthService.cs
│   ├── IBookService.cs
│   ├── ICategoryService.cs
│   ├── IReaderService.cs
│   ├── IBorrowService.cs
│   └── IReportService.cs
└── Services/
    ├── AuthService.cs              - BCrypt + lockout
    ├── BookService.cs              - CRUD sách + check trùng mã + check danh mục
    ├── CategoryService.cs          - CRUD danh mục
    ├── ReaderService.cs            - CRUD độc giả + RenewCard
    ├── BorrowService.cs            - Mượn/trả + tính phạt
    └── ReportService.cs            - Thống kê / dashboard
```

## Nguyên tắc thiết kế

### 1. Validate đầu vào trước khi gọi DAL

Mọi service đều validate đầu vào ngay đầu method qua lớp `Validator`:

```csharp
public int Create(Book book)
{
    ArgumentNullException.ThrowIfNull(book);
    Validator.NotEmpty(book.BookCode, "Mã sách");
    Validator.Length(book.BookCode, "Mã sách", 2, 30);
    Validator.PublishYear(book.PublishYear);
    Validator.NonNegative(book.Quantity, "Số lượng");
    // ... rồi mới gọi DAL
}
```

### 2. Bao bọc lỗi DAL → BusinessException

Mỗi service biết các mã lỗi từ stored procedure (50010, 50020, 50022...)
và chuyển thành `BusinessException` với thông báo tiếng Việt:

```csharp
catch (DalException ex) when (ex.SqlErrorNumber is 50020 or 50021 or 50022)
{
    throw new BusinessException(ex.Message, ex);
}
```

### 3. Inject repository qua constructor (DI ready)

Service nhận `IRepository` qua constructor → dễ unit test (mock repository)
và dễ wire qua `Microsoft.Extensions.DependencyInjection`:

```csharp
services.AddScoped<IBookRepository, BookRepository>();
services.AddScoped<IBookService,    BookService>();
```

### 4. Tự sinh mã phiếu

`BorrowService` tự sinh mã `PM{yyMMddHHmmss}{xx}` để UI không cần lo về duplicate:

```csharp
private static string GenerateReceiptCode(string prefix)
{
    var ts     = DateTime.Now.ToString("yyMMddHHmmss");
    var random = Random.Shared.Next(0, 100).ToString("D2");
    return $"{prefix}{ts}{random}";
}
```

### 5. Tính tiền phạt tự động

Khi UI gọi `CreateReturn` mà không truyền `FineOverride`, service tự tính:
- **Phạt quá hạn** = `daysOverdue × 5000 × quantity`
- **Phạt hỏng** = `Price × quantity × 0.3`
- **Phạt mất** = `Price × quantity × 1.0`

## Cách dùng từ UI

```csharp
// Khởi tạo các service
ICategoryRepository categoryRepo = new CategoryRepository();
IBookRepository     bookRepo     = new BookRepository();
IBookService        bookService  = new BookService(bookRepo, categoryRepo);

// Form thêm sách
try
{
    var book = new Book
    {
        BookCode    = txtCode.Text,
        Title       = txtTitle.Text,
        Author      = txtAuthor.Text,
        PublishYear = (int)numYear.Value,
        CategoryId  = (int)cbCategory.SelectedValue,
        Quantity    = (int)numQty.Value,
        Price       = numPrice.Value
    };
    int newId = bookService.Create(book);
    MessageBox.Show("Thêm sách thành công!");
}
catch (BusinessException ex)
{
    if (!string.IsNullOrEmpty(ex.FieldName))
    {
        // Highlight đúng field gây lỗi qua ErrorProvider
        errorProvider.SetError(GetControlByName(ex.FieldName), ex.Message);
    }
    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
```

## Login flow chuẩn

```csharp
IAuthService auth = new AuthService(userRepo);

var result = auth.Login(txtUsername.Text, txtPassword.Text, ipAddress: GetLocalIp());

if (!result.Success)
{
    MessageBox.Show(result.Message);
    return;
}

if (result.MustChangePassword)
{
    var frm = new FrmChangePassword(result.User!);
    frm.ShowDialog();
}

// Mở form chính
new FrmMain().Show();
this.Hide();
```

## NuGet packages

```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.2.2" />
```
