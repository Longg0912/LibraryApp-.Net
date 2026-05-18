# Hệ thống Quản lý Thư viện

> WinForms .NET 10 + SQL Server 2022 — bài tập lớn với kiến trúc 5-layer
> production-ready (RBAC, audit log, anti race-condition, ROWVERSION concurrency).

## Tổng quan

Ứng dụng quản lý thư viện đầy đủ chức năng nghiệp vụ:

- **Đăng nhập** với BCrypt + lockout sau 5 lần sai
- **Quản lý sách** — CRUD, tìm kiếm, danh mục, trạng thái
- **Quản lý độc giả** — CRUD, gia hạn thẻ, validate email/SĐT
- **Mượn / trả** — giỏ sách, transaction-safe, tự tính phạt
- **Tìm kiếm nâng cao** — đa thực thể, kết hợp nhiều tiêu chí
- **Thống kê** — KPI, biểu đồ bar/line/pie, doanh thu phạt
- **Xuất Excel** — ClosedXML, format đẹp, freeze pane
- **Quản lý người dùng** — Admin only, reset mật khẩu, khoá tài khoản
- **Đổi mật khẩu** 

## Kiến trúc

```
┌─────────────────────────────────────────────────────────┐
│  LibraryApp.UI (WinForms .NET 10 - net10.0-windows)     │
│  Forms, UserControls, AppContext, ServiceLocator         │
│  ErrorHandler, UiValidator, Logger, ExcelExporter        │
└────────────────────────┬────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────┐
│  LibraryApp.BLL (Business Logic - net10.0)              │
│  AuthService, BookService, ReaderService, BorrowService │
│  ReportService, UserService, CategoryService             │
│  + Validator + BusinessException + DTOs                  │
└────────────────────────┬────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────┐
│  LibraryApp.DAL (Data Access - net10.0)                 │
│  Repositories + Mappers + DatabaseConnection             │
│  ADO.NET + Microsoft.Data.SqlClient + Stored Procedures │
└────────────────────────┬────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────┐
│  LibraryApp.Models (POCO Entities - net10.0)            │
│  Book, Reader, BorrowReceipt, User, Role, ...           │
│  + Enums (BookStatus, BorrowStatus, ReaderStatus, ...)  │
└─────────────────────────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────┐
│  SQL Server 2022 — HUST_Library_DEV                     │
│  16 tables + views + stored procedures + triggers       │
│  RBAC, AuditLogs, ROWVERSION, Soft Delete                │
└─────────────────────────────────────────────────────────┘
```

## ⚙ Yêu cầu hệ thống

| Thành phần | Phiên bản |
|---|---|
| OS | Windows 10/11 (64-bit) |
| .NET SDK | **10.0** trở lên |
| SQL Server | **2019 hoặc 2022** (Express là đủ) |
| Visual Studio | 2022 (17.8+) hoặc VS Code + C# extension |
| Disk space | ~500 MB cho dependencies + database |
| RAM | tối thiểu 4 GB |

## Cài đặt nhanh

### 1. Clone repo

```bash
git clone <repo-url> LibraryApp
cd LibraryApp
```

### 2. Setup database

Mở **SQL Server Management Studio** (SSMS) hoặc **Azure Data Studio**, kết nối tới SQL Server local, chạy script:

```
docs/sql/HUST_Library_Production.sql
```

Script tự tạo database `TLU_Library_DEV`, 16 bảng, view, stored procedures, triggers, và insert dữ liệu mẫu.

Tiếp theo, hash mật khẩu mặc định "123456" cho 4 tài khoản test:

```powershell
# Chạy snippet này trong dotnet script hoặc LinqPad
# Hoặc dùng tool online (https://www.browserling.com/tools/bcrypt) với cost=11
$hash = "<copy hash từ tool BCrypt>"
sqlcmd -S . -d HUST_Library_DEV -Q `
  "UPDATE dbo.Users SET PasswordHash = '$hash' WHERE Username IN ('admin','thuthu1','thuthu2','viewer1');"
```


### 3. Cấu hình connection string

Mở `LibraryApp.UI/App.config`, sửa nếu cần:

```xml
<connectionStrings>
  <add name="LibraryDb"
       connectionString="Server=.;Database=HUST_Library_DEV;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
       providerName="Microsoft.Data.SqlClient" />
</connectionStrings>
```

> **Nếu SQL Server cài named instance (vd: `SQLEXPRESS`):** đổi `Server=.` thành `Server=.\SQLEXPRESS`.
> **Nếu dùng SQL Authentication:** đổi `Trusted_Connection=True` thành `User Id=sa;Password=...`.

### 4. Restore + Build

```bash
dotnet restore LibraryApp.sln
dotnet build LibraryApp.sln --configuration Debug
```

### 5. Chạy

```bash
dotnet run --project LibraryApp.UI
```

Hoặc mở `LibraryApp.sln` trong Visual Studio → nhấn **F5**.

### Tài khoản đăng nhập mặc định

| Username | Mật khẩu | Vai trò |
|---|---|---|
| `admin` | `123456` | Quản trị viên |
| `thuthu1` | `123456` | Thủ thư |
| `thuthu2` | `123456` | Thủ thư |
| `viewer1` | `123456` | Người xem (chỉ đọc) |

## Cấu trúc thư mục

```
LibraryApp/
├── LibraryApp.sln                    # Solution file
├── README.md                          # Tài liệu này
├── .gitignore
├── .editorconfig                      # Code style nhất quán
│
├── LibraryApp.Models/                 # POCO entities
│   ├── Book.cs, Reader.cs, ...
│   ├── Enums/
│   └── BaseAuditableEntity.cs
│
├── LibraryApp.DAL/                    # Data access
│   ├── Common/                        # DatabaseConnection, DalException
│   ├── Interfaces/                    # IBookRepository, ...
│   ├── Mappers/                       # IDataReader → entity
│   └── Repositories/                  # ADO.NET implementations
│
├── LibraryApp.BLL/                    # Business logic
│   ├── Common/                        # BusinessException, ValidationResult
│   ├── Validation/                    # Validator (static helpers)
│   ├── Interfaces/                    # IBookService, IUserService, ...
│   ├── Dtos/                          # LoginResult, DashboardKpi
│   └── Services/                      # Implementations + BCrypt + transactions
│
├── LibraryApp.UI/                     # WinForms presentation
│   ├── App.config                     # Connection string
│   ├── Program.cs                     # Entry + global handlers
│   ├── Common/
│   │   ├── CurrentSession.cs              # CurrentUser, IsAdmin
│   │   ├── ServiceLocator.cs          # DI container
│   │   ├── Logger.cs                  # File logging
│   │   ├── UiValidator.cs             # Fluent control validation
│   │   ├── ErrorHandler.cs            # Centralized exception dispatch
│   │   └── ExcelExporter.cs           # ClosedXML wrapper
│   ├── Forms/
│   │   ├── FrmLogin.* / FrmMain.*
│   │   ├── FrmChangePassword.*
│   │   └── FrmReturn.*                # Dialog ghi nhận trả
│   └── UserControls/
│       ├── UcDashboard, UcPlaceholder
│       ├── UcBookList, UcCategoryList, UcReaderList
│       ├── UcAdvancedSearch
│       ├── UcBorrowCreate, UcBorrowList
│       ├── UcReports + SimpleChart    # Tự vẽ chart GDI+
│       └── UcUserList                 # Admin only
│
├── docs/
│   ├── DATABASE_SETUP.md              # Hướng dẫn setup CSDL
│   ├── DEPLOY.md                      # Deploy production
│   └── sql/
│       └── HUST_Library_Production.sql
│
└── scripts/
    ├── Backup-Database.ps1            # Backup tự động
    └── Restore-Database.ps1           # Restore từ .bak
```

## Test nhanh các luồng nghiệp vụ

Sau khi đăng nhập với `admin/123456`:

### Luồng 1: Thêm sách → cho mượn → trả

1. Menu **Sách → Danh sách** → thêm sách "C# In-depth" số lượng 5
2. Menu **Mượn / Trả → Lập phiếu mượn** (`Ctrl+N`)
3. Nhập số thẻ `TV001` → Enter → kiểm tra info độc giả
4. Tìm "c#" → chọn sách → SL 1 → "Thêm vào giỏ"
5. Đặt hạn trả 7 ngày → "✓ LẬP PHIẾU MƯỢN"
6. Menu **Mượn / Trả → Danh sách** → chọn phiếu vừa tạo → "✓ Ghi nhận trả"
7. Confirm → kiểm tra tổng phạt = 0 (không quá hạn)

### Luồng 2: Quá hạn + phạt

1. Cho mượn sách
2. Sửa hạn trả thủ công trong DB lùi về quá khứ:
   ```sql
   UPDATE dbo.BorrowReceipts SET DueDate = '2026-01-01' WHERE ReceiptCode = 'PM...';
   ```
3. Trả sách → kiểm tra tự tính phạt = `daysOverdue × 5000`

### Luồng 3: Admin quản lý user

1. Menu **Tài khoản → Quản lý người dùng**
2. "➕ Thêm mới" → điền form, mật khẩu tạm `abc12345` → Lưu
3. Logout → đăng nhập bằng tài khoản mới → app yêu cầu đổi mật khẩu
4. Login lại bằng admin → "Reset mật khẩu" cho user → copy mật khẩu mới

## Phím tắt

| Phím | Chức năng |
|---|---|
| `Ctrl+B` | Quản lý sách |
| `Ctrl+M` | Danh mục |
| `Ctrl+D` | Độc giả |
| `Ctrl+N` | Lập phiếu mượn |
| `Ctrl+R` | Ghi nhận trả |
| `Ctrl+F` | Tìm kiếm nâng cao |
| `Ctrl+L` | Đăng xuất |
| `Alt+F4` | Thoát app |

## Bảo mật & RBAC

| Vai trò | Quyền |
|---|---|
| **ADMIN** | Toàn bộ chức năng + quản lý người dùng + audit log |
| **LIBRARIAN** | CRUD sách/độc giả/danh mục, lập/ghi nhận phiếu mượn, xem báo cáo |
| **VIEWER** | Chỉ xem dashboard + tìm kiếm + báo cáo (read-only) |

Mật khẩu được hash bằng **BCrypt cost factor 11** (~100ms/lần verify).
Sau 5 lần đăng nhập sai → khóa tài khoản 15 phút.

## 📝 Logging

- File log: `{exe-dir}/logs/yyyy-MM-dd.log`
- 4 mức: Debug / Info / Warning / Error
- Mặc định: `Info` trở lên. Bật Debug: `Logger.SetMinLevel(LogLevel.Debug)` trong `Program.cs`.
- Stack trace đầy đủ cho mức Error.

## Xử lý lỗi

Toàn bộ exception đi qua `ErrorHandler.Handle()` — phân loại tự động:

- `BusinessException` → MessageBox Warning với message tiếng Việt
- `DalException` → phân loại theo `SqlErrorNumber`
- `SqlException` raw → bắt 10+ loại lỗi connection/auth/permission
- Lỗi khác → MessageBox Error + ghi log đầy đủ

## Deploy production

Xem chi tiết [docs/DEPLOY.md](docs/DEPLOY.md).

Tóm tắt:
```bash
dotnet publish LibraryApp.UI -c Release -r win-x64 --self-contained false -o ./publish
```

## Tài liệu

- [Database Setup](docs/DATABASE_SETUP.md) — chi tiết script SQL + dữ liệu mẫu
- [Deploy Guide](docs/DEPLOY.md) — publish, cài đặt SQL Server, troubleshoot
- [SQL Script](docs/sql/HUST_Library_Production.sql) — full schema + sample data

## Đóng góp

Dự án bài tập lớn — không nhận PR ngoài.

## 📄 License

MIT — chi tiết trong file `LICENSE` (nếu có).
