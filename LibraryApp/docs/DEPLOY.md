# Deploy production

Tài liệu này hướng dẫn đóng gói và triển khai hệ thống lên môi trường thực tế.

## 1. Build & publish

### Cấu hình Release

```bash
dotnet publish LibraryApp.UI -c Release -r win-x64 --self-contained false -o publish/
```

Trong đó:
- `-c Release` — tối ưu code, tắt debug symbols
- `-r win-x64` — target Windows 64-bit
- `--self-contained false` — yêu cầu máy đích có **.NET 10 Runtime** cài sẵn (gọn ~50 MB)

### Self-contained (không cần cài .NET runtime)

Nếu máy đích không thể cài .NET runtime, đóng gói self-contained:

```bash
dotnet publish LibraryApp.UI -c Release -r win-x64 --self-contained true -o publish/
```

→ thư mục output sẽ nặng hơn ~150 MB nhưng chạy độc lập.

### Single file (gọn nhất)

```bash
dotnet publish LibraryApp.UI -c Release -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -o publish/
```

→ chỉ còn 1 file `LibraryApp.UI.exe` + vài file native DLL.

## 2. Cấu trúc thư mục publish

```
publish/
├── LibraryApp.UI.exe                 # Main executable
├── LibraryApp.UI.dll
├── LibraryApp.Models.dll
├── LibraryApp.DAL.dll
├── LibraryApp.BLL.dll
├── Microsoft.Data.SqlClient.dll
├── BCrypt.Net-Next.dll
├── ClosedXML.dll
├── App.config                        # ⚠ Cần sửa connection string
├── LibraryApp.UI.runtimeconfig.json
├── runtimes/                          # native dependencies
└── logs/                              # tự tạo khi app chạy
```

## 3. Cài đặt trên máy đích

### Bước 1: Chuẩn bị máy

- Windows 10/11 (64-bit) đã update
- Cài **.NET 10 Desktop Runtime** từ https://dotnet.microsoft.com/download/dotnet/10.0
  (chỉ cần `dotnet-runtime` hoặc `windowsdesktop-runtime`, không cần SDK)
- Cài **SQL Server 2019/2022 Express** nếu chưa có
- Cài **Visual C++ Redistributable** (thường đã có sẵn trên Win 10/11)

### Bước 2: Copy thư mục `publish/`

Copy toàn bộ thư mục vào một vị trí cố định, ví dụ:

```
C:\Program Files\LibraryApp\
```

### Bước 3: Cấu hình connection string

Sửa file `App.config` trong thư mục cài đặt:

```xml
<connectionStrings>
  <add name="LibraryDb"
       connectionString="Server=YOUR_SERVER;Database=HUST_Library_DEV;User Id=library_user;Password=...;Encrypt=True;TrustServerCertificate=True"
       providerName="Microsoft.Data.SqlClient" />
</connectionStrings>
```

> ⚠ **Production:** dùng SQL Login riêng (không phải `sa`), cấp quyền `db_owner` trên DB này.

### Bước 4: Setup database

Theo [DATABASE_SETUP.md](DATABASE_SETUP.md), chạy script trên SQL Server đích.

### Bước 5: Tạo shortcut

Tạo shortcut Desktop / Start Menu trỏ tới `LibraryApp.UI.exe`.

### Bước 6: Test

Double-click shortcut → form đăng nhập hiện ra → đăng nhập với `admin/123456`.

Nếu lỗi kết nối → kiểm tra:
1. SQL Server đang chạy (`services.msc` → SQL Server)
2. Tường lửa cho phép port 1433
3. Connection string trong `App.config` đúng

## 4. Tạo MSI installer (tuỳ chọn)

Dùng **WiX Toolset** hoặc **Inno Setup** để tạo file `.msi` / `.exe` installer.

### Inno Setup (đơn giản nhất)

Cài Inno Setup từ https://jrsoftware.org/isdl.php

Tạo file `LibraryApp.iss`:

```ini
[Setup]
AppName=Hệ thống Quản lý Thư viện
AppVersion=1.0
DefaultDirName={pf}\LibraryApp
DefaultGroupName=Library Management
OutputBaseFilename=LibraryApp-Setup-1.0
Compression=lzma2
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64

[Files]
Source: "publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\Hệ thống Quản lý Thư viện"; Filename: "{app}\LibraryApp.UI.exe"
Name: "{commondesktop}\Hệ thống Quản lý Thư viện"; Filename: "{app}\LibraryApp.UI.exe"; Tasks: desktopicon

[Tasks]
Name: desktopicon; Description: "Tạo shortcut trên Desktop"; GroupDescription: "Tuỳ chọn:"

[Run]
Filename: "{app}\LibraryApp.UI.exe"; Description: "Chạy ngay sau khi cài"; Flags: postinstall nowait skipifsilent
```

Right-click → Compile → Output file `LibraryApp-Setup-1.0.exe`.

## 5. Update sau khi triển khai

### Update code

1. Build lại theo bước 1.
2. Copy chèn file `.dll` mới vào thư mục cài đặt (ghi đè).
3. Restart app.

> ⚠ Đóng app trước khi copy. Nếu app đang chạy, các DLL bị lock.

### Update database

1. Backup DB trước (xem `scripts/Backup-Database.ps1`).
2. Chạy script migration (nếu có).
3. Test app sau khi update.

## 6. Logging trong production

Mặc định log nằm trong `{exe-dir}\logs\YYYY-MM-DD.log`.

Để debug khi có sự cố, copy file log gửi về dev. Hoặc cài thêm:

```csharp
// Trong Program.cs - bật DEBUG khi cần troubleshoot
Logger.SetMinLevel(LogLevel.Debug);
```

## 7. Bảo trì định kỳ

### Backup DB tự động

Setup job backup hằng ngày (xem `DATABASE_SETUP.md` mục 6).

### Dọn log cũ

PowerShell xoá log >30 ngày:

```powershell
$logDir = "C:\Program Files\LibraryApp\logs"
Get-ChildItem $logDir -Filter *.log |
    Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-30) } |
    Remove-Item
```

Đặt vào Task Scheduler chạy hàng tuần.

### Update statistics & rebuild index (SQL Server)

```sql
USE HUST_Library_DEV;
EXEC sp_updatestats;

ALTER INDEX ALL ON dbo.Books REBUILD;
ALTER INDEX ALL ON dbo.BorrowReceipts REBUILD;
ALTER INDEX ALL ON dbo.BorrowReceiptDetails REBUILD;
ALTER INDEX ALL ON dbo.AuditLogs REBUILD;
```

Chạy hàng tuần để giữ performance.

## 8. Troubleshooting

### App không khởi động

1. Mở Event Viewer (Win+R → `eventvwr.msc`) → Windows Logs → Application
2. Tìm event nguồn `.NET Runtime` → đọc stack trace
3. Hoặc chạy app từ CMD/PowerShell để xem console output:
   ```
   cd "C:\Program Files\LibraryApp"
   LibraryApp.UI.exe
   ```

### Lỗi "Could not load file or assembly Microsoft.Data.SqlClient"

→ Thiếu Visual C++ Redistributable. Cài từ:
https://aka.ms/vs/17/release/vc_redist.x64.exe

### App rất chậm khi mở

→ Anti-virus đang scan. Add `C:\Program Files\LibraryApp\` vào exclusion list.

### Login OK nhưng MainForm trắng

→ DPI scaling issue. Click chuột phải `LibraryApp.UI.exe` → Properties → Compatibility →
**Change high DPI settings** → Tick **Override high DPI scaling behavior** → System (Enhanced).

## 9. Bảo mật production

### Connection string

❌ **Không hardcode credential** trong `App.config` của bản phân phối.
✅ Dùng **Windows Authentication** với service account, hoặc lưu credentials trong **Windows Credential Manager**.

### SSL/TLS

Connection mặc định có `Encrypt=True`. Đảm bảo SQL Server đã cài certificate:
- Production: cert từ CA tin cậy
- Dev/test: self-signed cert + `TrustServerCertificate=True`

### Audit log

Bảng `AuditLogs` ghi mọi thao tác. Đảm bảo:
- Chỉ Admin được xem
- Có job archive hàng tháng để bảng không quá to
- Backup riêng cho compliance
