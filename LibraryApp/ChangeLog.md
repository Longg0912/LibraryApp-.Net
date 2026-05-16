# Changelog

Theo định dạng [Keep a Changelog](https://keepachangelog.com/vi/1.1.0/).

## [1.0.0] - 2026-05-16

### Added

#### Database
- 16 bảng nghiệp vụ với RBAC, audit log, ROWVERSION concurrency
- 7 views cho báo cáo (Dashboard KPI, Top Books, Overdue, ...)
- Stored procedures với `UPDLOCK + HOLDLOCK` chống race condition
- Triggers tự cập nhật trạng thái sách và phiếu mượn quá hạn
- Soft delete trên mọi bảng nghiệp vụ
- Indexes tối ưu cho query thường dùng

#### Models (LibraryApp.Models)
- POCO entities với XML documentation đầy đủ
- 5 enum (BookStatus, BorrowStatus, ReaderStatus, BookCondition, Gender)
- `BaseAuditableEntity` với 7 audit fields

#### Data Access (LibraryApp.DAL)
- 5 repositories (Book, Category, Reader, User, BorrowReceipt)
- ADO.NET với Microsoft.Data.SqlClient 5.2.2
- Manual mappers (không dùng EF Core)
- `DalException` với SqlErrorNumber + tự dịch lỗi sang tiếng Việt
- Hỗ trợ TVP (Table-Valued Parameters) cho lập phiếu mượn/trả

#### Business Logic (LibraryApp.BLL)
- 7 services (Auth, Book, Category, Reader, Borrow, Report, User)
- BCrypt cost factor 11 cho hash mật khẩu
- `Validator` static với 12 rule (NotEmpty, Email, Phone, Password, ...)
- `BusinessException` + `ValidationResult`
- DTOs: `LoginResult`, `DashboardKpi`

#### UI (LibraryApp.UI - WinForms .NET 10)
- 4 forms: `FrmLogin`, `FrmMain`, `FrmReturn`, `FrmChangePassword`
- 11 UserControls cho CRUD và báo cáo
- `UcReports` với 5 tab thống kê + biểu đồ Bar/Line/Pie tự vẽ GDI+
- `UcAdvancedSearch` đa thực thể (Books / Readers / BorrowReceipts)
- `UcUserList` cho Admin (CRUD + reset password + khoá tài khoản)
- `MenuStrip` + `Sidebar` + `StatusStrip` với đồng hồ realtime
- Phân quyền menu theo role (`AppContext.IsAdmin` / `IsLibrarian`)
- Async login với BCrypt trên background thread
- Lockout policy: 5 lần sai → khoá 15 phút

#### Cross-cutting concerns
- `Logger` ghi file `logs/yyyy-MM-dd.log` với 4 mức
- `UiValidator` fluent API validate WinForms controls
- `ErrorHandler` dispatcher phân loại 9+ loại exception
- `ExcelExporter` xuất xlsx với ClosedXML 0.104.2 (header màu, freeze pane, banded rows)
- Global exception handlers (ThreadException, UnhandledException, UnobservedTaskException)

#### DevOps
- Solution file `.sln` wrap 4 csproj
- `.gitignore` chuẩn .NET
- `.editorconfig` đồng bộ code style
- PowerShell scripts: `Backup-Database.ps1`, `Restore-Database.ps1`
- Tài liệu: README, DATABASE_SETUP, DEPLOY

### Security

- Tất cả password hash bằng BCrypt cost factor 11
- SQL injection prevention: 100% qua `SqlParameter`
- Account lockout sau 5 lần đăng nhập sai
- Audit log đầy đủ cho mọi thao tác CRUD
- Connection string mặc định dùng Windows Authentication
- TLS encryption bật mặc định (`Encrypt=True`)

### Known limitations

- Chưa có chức năng đặt trước (reservation) — đã có bảng nhưng chưa UI
- Chưa có notification system — bảng có sẵn
- Chưa có module xuất PDF — chỉ có Excel
- Reset password chỉ sinh chuỗi 8 ký tự (đủ phức tạp cho bài tập, nhưng production cần mạnh hơn)
