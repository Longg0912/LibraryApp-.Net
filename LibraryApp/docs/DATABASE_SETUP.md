# Hướng dẫn cài đặt cơ sở dữ liệu

## 1. Yêu cầu

- **SQL Server 2019 hoặc 2022** (bản Express cũng được).
- **SSMS** hoặc **Azure Data Studio** để chạy script.
- Tài khoản với quyền `CREATE DATABASE`.

## 2. Tải SQL Server Express (nếu chưa có)

1. Tải từ https://www.microsoft.com/sql-server/sql-server-downloads
2. Cài bản **Express** với **Basic** installation.
3. Sau khi cài, mặc định instance là `.\SQLEXPRESS` hoặc `localhost\SQLEXPRESS`.
4. Cài thêm **SSMS** từ https://aka.ms/ssmsfullsetup để dễ quản lý.

## 3. Chạy script khởi tạo

Mở SSMS → kết nối tới SQL Server → mở file:

```
docs/sql/HUST_Library_Production.sql
```

Nhấn **F5** (Execute). Script sẽ:

1. `DROP DATABASE IF EXISTS HUST_Library_DEV;`
2. `CREATE DATABASE HUST_Library_DEV COLLATE Vietnamese_CI_AS;`
3. Bật options: `READ_COMMITTED_SNAPSHOT ON`, `RECOVERY FULL`, `QUERY_STORE ON`.
4. Tạo **16 bảng**:
   - `Roles`, `Permissions`, `RolePermissions` (RBAC)
   - `Users` (nhân viên hệ thống)
   - `Categories`, `Books` (sách)
   - `Readers` (độc giả)
   - `BorrowReceipts`, `BorrowReceiptDetails` (mượn)
   - `ReturnReceipts`, `ReturnReceiptDetails` (trả)
   - `Reservations`, `Notifications`, `PenaltyHistory`
   - `AuditLogs`, `SystemSettings`
5. Tạo **7 views** cho báo cáo: `vw_BookOverview`, `vw_BorrowSummary`, `vw_OverdueBorrows`,
   `vw_Dashboard_KPI`, `vw_Dashboard_BorrowTrend`, ...
6. Tạo **stored procedures** chính:
   - `sp_Book_Insert/Update/Delete/Search`
   - `sp_Borrow_Create` — transaction-safe với `UPDLOCK + HOLDLOCK`
   - `sp_Return_Create` — tính phạt + cập nhật tồn kho
   - `sp_User_Login` — lockout policy
   - `sp_Stat_*` — báo cáo thống kê
7. Tạo **triggers**:
   - `trg_Books_UpdateStatus` — tự đổi Status theo AvailableQty
   - `trg_BorrowReceipts_Overdue` — tự set Overdue khi quá hạn
8. Insert dữ liệu mẫu:
   - 3 vai trò (`ADMIN`, `LIBRARIAN`, `VIEWER`)
   - 4 user mẫu (mật khẩu placeholder, cần update)
   - 5 danh mục (`CNTT`, `KT`, `VH`, `NN`, `TN`)
   - 10 đầu sách mẫu
   - 5 độc giả mẫu

## 4. Hash mật khẩu cho tài khoản test

⚠ Mật khẩu trong DB là placeholder, cần thay bằng BCrypt hash thật.

### Cách 1: Dùng dotnet script

Tạo file `hash.csx`:

```csharp
#r "nuget: BCrypt.Net-Next, 4.0.3"
Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("123456", workFactor: 11));
```

Chạy:
```bash
dotnet script hash.csx
```

### Cách 2: Tool online

Vào https://www.browserling.com/tools/bcrypt
- Plaintext: `123456`
- Rounds: `11`
- Copy hash bắt đầu bằng `$2a$11$...`

### Cách 3: Console app nhỏ

```csharp
using BCrypt.Net;
Console.WriteLine(BCrypt.HashPassword("123456", workFactor: 11));
```

### Cập nhật DB

```sql
USE HUST_Library_DEV;

-- Thay <hash> bằng kết quả từ bước trên
UPDATE dbo.Users
SET PasswordHash = '<hash-bắt-đầu-bằng-$2a$11$>'
WHERE Username IN ('admin', 'thuthu1', 'thuthu2', 'viewer1');
```

Verify:
```sql
SELECT Username, FullName, PasswordHash FROM dbo.Users;
```

Tất cả 4 dòng phải có `PasswordHash` bắt đầu bằng `$2a$11$`.

## 5. Test connection từ app

Mở `LibraryApp.UI/App.config`:

```xml
<connectionStrings>
  <add name="LibraryDb"
       connectionString="Server=.;Database=HUST_Library_DEV;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
       providerName="Microsoft.Data.SqlClient" />
</connectionStrings>
```

Các trường hợp thường gặp:

| Tình huống | Connection string |
|---|---|
| SQL Server Default instance | `Server=.` |
| SQL Server Express named instance | `Server=.\SQLEXPRESS` |
| Remote server | `Server=192.168.1.100,1433` |
| Windows Authentication | `Trusted_Connection=True` |
| SQL Authentication | `User Id=sa;Password=YourPwd;` |

## 6. Backup / Restore

### Backup thủ công bằng SSMS

Chuột phải database → **Tasks → Back Up...** → chọn nơi lưu file `.bak`.

### Backup tự động bằng PowerShell

```powershell
.\scripts\Backup-Database.ps1 -OutputDir "C:\Backups\Library"
```

### Backup theo lịch (SQL Server Agent)

```sql
USE msdb;
EXEC dbo.sp_add_job
    @job_name = N'Library_DailyBackup';
EXEC dbo.sp_add_jobstep
    @job_name   = N'Library_DailyBackup',
    @step_name  = N'Backup',
    @subsystem  = N'TSQL',
    @command    = N'BACKUP DATABASE HUST_Library_DEV TO DISK = ''C:\Backups\Library_$(ESCAPE_SQUOTE(DATE))_$(ESCAPE_SQUOTE(TIME)).bak'' WITH COMPRESSION;',
    @database_name = N'HUST_Library_DEV';
EXEC dbo.sp_add_schedule
    @schedule_name = N'EveryDayAt2AM',
    @freq_type = 4,  -- daily
    @freq_interval = 1,
    @active_start_time = 020000;
EXEC dbo.sp_attach_schedule
    @job_name = N'Library_DailyBackup',
    @schedule_name = N'EveryDayAt2AM';
EXEC dbo.sp_add_jobserver @job_name = N'Library_DailyBackup';
```

### Restore từ .bak

```sql
USE master;
ALTER DATABASE HUST_Library_DEV SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE HUST_Library_DEV
   FROM DISK = 'C:\Backups\Library_20260515.bak'
   WITH REPLACE, RECOVERY;
ALTER DATABASE HUST_Library_DEV SET MULTI_USER;
```

Hoặc dùng PowerShell:

```powershell
.\scripts\Restore-Database.ps1 -BackupFile "C:\Backups\Library_20260515.bak"
```

## 7. Reset database về dữ liệu mẫu

Khi muốn xóa hết test data và làm lại từ đầu:

```sql
USE master;
DROP DATABASE HUST_Library_DEV;
```

Rồi chạy lại `HUST_Library_Production.sql` ở bước 3.

## 8. Troubleshooting

### Lỗi `Cannot open database "HUST_Library_DEV"`

Database chưa được tạo. Chạy lại script khởi tạo.

### Lỗi `Login failed for user`

Tài khoản Windows hoặc SQL không có quyền. Trong SSMS:
- **Security → Logins** → chuột phải tài khoản → **Properties → User Mapping**
- Tick `HUST_Library_DEV` và đánh dấu `db_owner`.

### Lỗi `A network-related or instance-specific error occurred`

- SQL Server service không chạy: mở **Services.msc** → start `SQL Server (MSSQLSERVER)`
- Tường lửa block port 1433: tạo inbound rule allow port này
- Sai instance name: kiểm tra trong **SQL Server Configuration Manager**

### Lỗi `Cannot resolve the collation conflict`

Database collation khác `Vietnamese_CI_AS`. Drop và tạo lại bằng script.

### Mất hết quyền admin trong app

Mở SSMS, mở khóa tài khoản admin:
```sql
USE HUST_Library_DEV;
UPDATE dbo.Users
SET IsActive = 1, FailedLoginCount = 0, LockoutUntil = NULL
WHERE Username = 'admin';
```
