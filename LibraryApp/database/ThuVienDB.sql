/* =============================================================================
   FILE          : TLU_Library_Production.sql
   MÔ TẢ        : Script SQL Server PRODUCTION-READY cho Hệ thống Quản lý Thư viện
   MÔI TRƯỜNG   : SQL Server 2022 (chạy được trong SSMS 2022)
   ĐẶC ĐIỂM     : - Optimistic concurrency (ROWVERSION)
                  - Anti race-condition (UPDLOCK + HOLDLOCK)
                  - Soft delete chuẩn cho mọi bảng
                  - Audit log toàn diện qua JSON
                  - RBAC granular (Permissions + RolePermissions)
                  - Account lockout
                  - Config động qua SystemSettings
                  - Dashboard views
                  - Trigger chống recursive update
   ============================================================================= */

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* =============================================================================
   KHỐI 1: TẠO DATABASE + CẤU HÌNH PRODUCTION
   ============================================================================= */
USE master;
GO

IF DB_ID('TLU_Library_DEV') IS NOT NULL
BEGIN
    ALTER DATABASE TLU_Library_DEV SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TLU_Library_DEV;
END
GO

CREATE DATABASE TLU_Library_DEV COLLATE Vietnamese_CI_AS;
GO

ALTER DATABASE TLU_Library_DEV SET RECOVERY FULL;
ALTER DATABASE TLU_Library_DEV SET READ_COMMITTED_SNAPSHOT ON;    -- reader không chặn writer
ALTER DATABASE TLU_Library_DEV SET ALLOW_SNAPSHOT_ISOLATION ON;
ALTER DATABASE TLU_Library_DEV SET AUTO_UPDATE_STATISTICS ON;
ALTER DATABASE TLU_Library_DEV SET AUTO_CREATE_STATISTICS ON;
ALTER DATABASE TLU_Library_DEV SET QUERY_STORE = ON;              -- theo dõi performance
ALTER DATABASE TLU_Library_DEV SET RECURSIVE_TRIGGERS OFF;
GO

USE TLU_Library_DEV;
GO


/* =============================================================================
   KHỐI 2: TẠO BẢNG
   - Mọi bảng nghiệp vụ có: CreatedAt, CreatedBy, UpdatedAt, UpdatedBy,
                            IsDeleted, DeletedAt, DeletedBy, RowVersion
   ============================================================================= */

-- 2.1 Roles -------------------------------------------------------------------
CREATE TABLE dbo.Roles (
    RoleId      INT IDENTITY(1,1) NOT NULL,
    RoleCode    VARCHAR(20)   NOT NULL,
    RoleName    NVARCHAR(50)  NOT NULL,
    Description NVARCHAR(200) NULL,
    IsActive    BIT NOT NULL CONSTRAINT DF_Roles_IsActive  DEFAULT 1,
    CreatedAt   DATETIME2(0) NOT NULL CONSTRAINT DF_Roles_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy   INT NULL,
    UpdatedAt   DATETIME2(0) NULL,
    UpdatedBy   INT NULL,
    IsDeleted   BIT NOT NULL CONSTRAINT DF_Roles_IsDeleted DEFAULT 0,
    DeletedAt   DATETIME2(0) NULL,
    DeletedBy   INT NULL,
    RowVer      ROWVERSION NOT NULL,
    CONSTRAINT PK_Roles      PRIMARY KEY (RoleId),
    CONSTRAINT UQ_Roles_Code UNIQUE (RoleCode)
);
GO

-- 2.2 Permissions (granular RBAC) --------------------------------------------
CREATE TABLE dbo.Permissions (
    PermissionId   INT IDENTITY(1,1) NOT NULL,
    PermissionCode VARCHAR(50)  NOT NULL,    -- ví dụ: BOOK_CREATE, BORROW_APPROVE
    Module         VARCHAR(30)  NOT NULL,    -- BOOK / READER / BORROW / REPORT / USER
    Description    NVARCHAR(200) NULL,
    CreatedAt      DATETIME2(0) NOT NULL CONSTRAINT DF_Permissions_CreatedAt DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Permissions      PRIMARY KEY (PermissionId),
    CONSTRAINT UQ_Permissions_Code UNIQUE (PermissionCode)
);
GO

-- 2.3 RolePermissions (M-N) ---------------------------------------------------
CREATE TABLE dbo.RolePermissions (
    RoleId       INT NOT NULL,
    PermissionId INT NOT NULL,
    GrantedAt    DATETIME2(0) NOT NULL CONSTRAINT DF_RP_GrantedAt DEFAULT SYSUTCDATETIME(),
    GrantedBy    INT NULL,
    CONSTRAINT PK_RolePermissions PRIMARY KEY (RoleId, PermissionId),
    CONSTRAINT FK_RP_Roles        FOREIGN KEY (RoleId)       REFERENCES dbo.Roles(RoleId)             ON DELETE CASCADE,
    CONSTRAINT FK_RP_Permissions  FOREIGN KEY (PermissionId) REFERENCES dbo.Permissions(PermissionId) ON DELETE CASCADE
);
GO

-- 2.4 Users -------------------------------------------------------------------
CREATE TABLE dbo.Users (
    UserId             INT IDENTITY(1,1) NOT NULL,
    Username           VARCHAR(50)  NOT NULL,
    PasswordHash       VARCHAR(255) NOT NULL,
    PasswordSalt       VARCHAR(128) NULL,
    FullName           NVARCHAR(100) NOT NULL,
    Email              VARCHAR(100) NULL,
    Phone              VARCHAR(20)  NULL,
    RoleId             INT NOT NULL,
    IsActive           BIT NOT NULL CONSTRAINT DF_Users_IsActive   DEFAULT 1,
    FailedLoginCount   INT NOT NULL CONSTRAINT DF_Users_Failed     DEFAULT 0,
    LockoutUntil       DATETIME2(0) NULL,
    LastLoginAt        DATETIME2(0) NULL,
    LastLoginIp        VARCHAR(45) NULL,
    PasswordChangedAt  DATETIME2(0) NULL,
    MustChangePassword BIT NOT NULL CONSTRAINT DF_Users_MustChg    DEFAULT 0,
    CreatedAt          DATETIME2(0) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy          INT NULL,
    UpdatedAt          DATETIME2(0) NULL,
    UpdatedBy          INT NULL,
    IsDeleted          BIT NOT NULL CONSTRAINT DF_Users_IsDeleted  DEFAULT 0,
    DeletedAt          DATETIME2(0) NULL,
    DeletedBy          INT NULL,
    RowVer             ROWVERSION NOT NULL,
    CONSTRAINT PK_Users          PRIMARY KEY (UserId),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT FK_Users_Roles    FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId),
    CONSTRAINT CK_Users_Email    CHECK (Email IS NULL OR Email LIKE '%_@_%._%'),
    CONSTRAINT CK_Users_Phone    CHECK (Phone IS NULL OR Phone NOT LIKE '%[^0-9+]%')
);
GO

-- 2.5 Categories --------------------------------------------------------------
CREATE TABLE dbo.Categories (
    CategoryId   INT IDENTITY(1,1) NOT NULL,
    CategoryCode VARCHAR(20)   NOT NULL,
    CategoryName NVARCHAR(100) NOT NULL,
    Description  NVARCHAR(300) NULL,
    IsActive     BIT NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT 1,
    CreatedAt    DATETIME2(0) NOT NULL CONSTRAINT DF_Categories_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy    INT NULL,
    UpdatedAt    DATETIME2(0) NULL,
    UpdatedBy    INT NULL,
    IsDeleted    BIT NOT NULL CONSTRAINT DF_Categories_IsDeleted DEFAULT 0,
    DeletedAt    DATETIME2(0) NULL,
    DeletedBy    INT NULL,
    RowVer       ROWVERSION NOT NULL,
    CONSTRAINT PK_Categories      PRIMARY KEY (CategoryId),
    CONSTRAINT UQ_Categories_Code UNIQUE (CategoryCode)
);
GO

-- 2.6 Books -------------------------------------------------------------------
CREATE TABLE dbo.Books (
    BookId       INT IDENTITY(1,1) NOT NULL,
    BookCode     VARCHAR(30)   NOT NULL,
    Title        NVARCHAR(200) NOT NULL,
    Author       NVARCHAR(150) NOT NULL,
    Publisher    NVARCHAR(150) NULL,
    PublishYear  INT NULL,
    CategoryId   INT NOT NULL,
    Quantity     INT NOT NULL CONSTRAINT DF_Books_Quantity     DEFAULT 0,
    AvailableQty INT NOT NULL CONSTRAINT DF_Books_AvailableQty DEFAULT 0,
    Price        DECIMAL(12,2) NOT NULL CONSTRAINT DF_Books_Price DEFAULT 0,
    Status       VARCHAR(20)  NOT NULL CONSTRAINT DF_Books_Status DEFAULT 'Available',
    CreatedAt    DATETIME2(0) NOT NULL CONSTRAINT DF_Books_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy    INT NULL,
    UpdatedAt    DATETIME2(0) NULL,
    UpdatedBy    INT NULL,
    IsDeleted    BIT NOT NULL CONSTRAINT DF_Books_IsDeleted DEFAULT 0,
    DeletedAt    DATETIME2(0) NULL,
    DeletedBy    INT NULL,
    RowVer       ROWVERSION NOT NULL,
    CONSTRAINT PK_Books            PRIMARY KEY (BookId),
    CONSTRAINT UQ_Books_Code       UNIQUE (BookCode),
    CONSTRAINT FK_Books_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(CategoryId),
    CONSTRAINT CK_Books_Quantity   CHECK (Quantity >= 0),
    CONSTRAINT CK_Books_Available  CHECK (AvailableQty >= 0 AND AvailableQty <= Quantity),
    CONSTRAINT CK_Books_Year       CHECK (PublishYear IS NULL OR PublishYear BETWEEN 1500 AND 2100),
    CONSTRAINT CK_Books_Price      CHECK (Price >= 0),
    CONSTRAINT CK_Books_Status     CHECK (Status IN ('Available','OutOfStock','Lost','Damaged','Retired'))
);
GO

-- 2.7 Readers -----------------------------------------------------------------
CREATE TABLE dbo.Readers (
    ReaderId       INT IDENTITY(1,1) NOT NULL,
    CardNumber     VARCHAR(20)   NOT NULL,
    FullName       NVARCHAR(100) NOT NULL,
    DateOfBirth    DATE NULL,
    Gender         NVARCHAR(10) NULL,
    Address        NVARCHAR(200) NULL,
    Phone          VARCHAR(20)   NULL,
    Email          VARCHAR(100)  NULL,
    CardIssueDate  DATE NOT NULL CONSTRAINT DF_Readers_Issue  DEFAULT CAST(SYSUTCDATETIME() AS DATE),
    CardExpireDate DATE NOT NULL,
    Status         VARCHAR(20) NOT NULL CONSTRAINT DF_Readers_Status DEFAULT 'Active',
    CreatedAt      DATETIME2(0) NOT NULL CONSTRAINT DF_Readers_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy      INT NULL,
    UpdatedAt      DATETIME2(0) NULL,
    UpdatedBy      INT NULL,
    IsDeleted      BIT NOT NULL CONSTRAINT DF_Readers_IsDeleted DEFAULT 0,
    DeletedAt      DATETIME2(0) NULL,
    DeletedBy      INT NULL,
    RowVer         ROWVERSION NOT NULL,
    CONSTRAINT PK_Readers         PRIMARY KEY (ReaderId),
    CONSTRAINT UQ_Readers_Card    UNIQUE (CardNumber),
    CONSTRAINT CK_Readers_Gender  CHECK (Gender IS NULL OR Gender IN (N'Nam', N'Nữ', N'Khác')),
    CONSTRAINT CK_Readers_Status  CHECK (Status IN ('Active','Locked','Expired')),
    CONSTRAINT CK_Readers_Expire  CHECK (CardExpireDate >= CardIssueDate),
    CONSTRAINT CK_Readers_Email   CHECK (Email IS NULL OR Email LIKE '%_@_%._%'),
    CONSTRAINT CK_Readers_Phone   CHECK (Phone IS NULL OR Phone NOT LIKE '%[^0-9+]%')
);
GO

-- 2.8 BorrowReceipts ----------------------------------------------------------
CREATE TABLE dbo.BorrowReceipts (
    BorrowId    INT IDENTITY(1,1) NOT NULL,
    ReceiptCode VARCHAR(30) NOT NULL,
    ReaderId    INT NOT NULL,
    UserId      INT NOT NULL,
    BorrowDate  DATE NOT NULL CONSTRAINT DF_Borrow_Date DEFAULT CAST(SYSUTCDATETIME() AS DATE),
    DueDate     DATE NOT NULL,
    Status      VARCHAR(20) NOT NULL CONSTRAINT DF_Borrow_Status DEFAULT 'Borrowing',
    TotalFine   DECIMAL(12,2) NOT NULL CONSTRAINT DF_Borrow_Fine DEFAULT 0,
    RenewCount  INT NOT NULL CONSTRAINT DF_Borrow_Renew DEFAULT 0,
    Note        NVARCHAR(300) NULL,
    CreatedAt   DATETIME2(0) NOT NULL CONSTRAINT DF_Borrow_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy   INT NULL,
    UpdatedAt   DATETIME2(0) NULL,
    UpdatedBy   INT NULL,
    IsDeleted   BIT NOT NULL CONSTRAINT DF_Borrow_IsDeleted DEFAULT 0,
    DeletedAt   DATETIME2(0) NULL,
    DeletedBy   INT NULL,
    RowVer      ROWVERSION NOT NULL,
    CONSTRAINT PK_BorrowReceipts      PRIMARY KEY (BorrowId),
    CONSTRAINT UQ_BorrowReceipts_Code UNIQUE (ReceiptCode),
    CONSTRAINT FK_Borrow_Readers      FOREIGN KEY (ReaderId) REFERENCES dbo.Readers(ReaderId),
    CONSTRAINT FK_Borrow_Users        FOREIGN KEY (UserId)   REFERENCES dbo.Users(UserId),
    CONSTRAINT CK_Borrow_Status       CHECK (Status IN ('Borrowing','PartiallyReturned','Returned','Overdue','Cancelled')),
    CONSTRAINT CK_Borrow_Due          CHECK (DueDate >= BorrowDate),
    CONSTRAINT CK_Borrow_Fine         CHECK (TotalFine >= 0),
    CONSTRAINT CK_Borrow_DateRange    CHECK (DATEDIFF(DAY, BorrowDate, DueDate) BETWEEN 1 AND 90)
);
GO

-- 2.9 BorrowReceiptDetails ----------------------------------------------------
CREATE TABLE dbo.BorrowReceiptDetails (
    BorrowDetailId INT IDENTITY(1,1) NOT NULL,
    BorrowId       INT NOT NULL,
    BookId         INT NOT NULL,
    Quantity       INT NOT NULL CONSTRAINT DF_BorrowDtl_Qty    DEFAULT 1,
    ReturnedQty    INT NOT NULL CONSTRAINT DF_BorrowDtl_RetQty DEFAULT 0,
    Note           NVARCHAR(200) NULL,
    CONSTRAINT PK_BorrowDtl          PRIMARY KEY (BorrowDetailId),
    CONSTRAINT FK_BorrowDtl_Receipt  FOREIGN KEY (BorrowId) REFERENCES dbo.BorrowReceipts(BorrowId) ON DELETE CASCADE,
    CONSTRAINT FK_BorrowDtl_Books    FOREIGN KEY (BookId)   REFERENCES dbo.Books(BookId),
    CONSTRAINT UQ_BorrowDtl_BookOnce UNIQUE (BorrowId, BookId),
    CONSTRAINT CK_BorrowDtl_Qty      CHECK (Quantity > 0),
    CONSTRAINT CK_BorrowDtl_Returned CHECK (ReturnedQty >= 0 AND ReturnedQty <= Quantity)
);
GO

-- 2.10 ReturnReceipts ---------------------------------------------------------
CREATE TABLE dbo.ReturnReceipts (
    ReturnId    INT IDENTITY(1,1) NOT NULL,
    ReturnCode  VARCHAR(30) NOT NULL,
    BorrowId    INT NOT NULL,
    UserId      INT NOT NULL,
    ReturnDate  DATE NOT NULL CONSTRAINT DF_Return_Date DEFAULT CAST(SYSUTCDATETIME() AS DATE),
    TotalFine   DECIMAL(12,2) NOT NULL CONSTRAINT DF_Return_Fine DEFAULT 0,
    Note        NVARCHAR(300) NULL,
    CreatedAt   DATETIME2(0) NOT NULL CONSTRAINT DF_Return_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy   INT NULL,
    UpdatedAt   DATETIME2(0) NULL,
    UpdatedBy   INT NULL,
    IsDeleted   BIT NOT NULL CONSTRAINT DF_Return_IsDeleted DEFAULT 0,
    DeletedAt   DATETIME2(0) NULL,
    DeletedBy   INT NULL,
    RowVer      ROWVERSION NOT NULL,
    CONSTRAINT PK_ReturnReceipts      PRIMARY KEY (ReturnId),
    CONSTRAINT UQ_ReturnReceipts_Code UNIQUE (ReturnCode),
    CONSTRAINT FK_Return_Borrow       FOREIGN KEY (BorrowId) REFERENCES dbo.BorrowReceipts(BorrowId),
    CONSTRAINT FK_Return_Users        FOREIGN KEY (UserId)   REFERENCES dbo.Users(UserId),
    CONSTRAINT CK_Return_Fine         CHECK (TotalFine >= 0)
);
GO

-- 2.11 ReturnReceiptDetails ---------------------------------------------------
CREATE TABLE dbo.ReturnReceiptDetails (
    ReturnDetailId INT IDENTITY(1,1) NOT NULL,
    ReturnId       INT NOT NULL,
    BorrowDetailId INT NOT NULL,
    Quantity       INT NOT NULL,
    Condition      VARCHAR(20) NOT NULL CONSTRAINT DF_RetDtl_Cond DEFAULT 'Good',
    Fine           DECIMAL(12,2) NOT NULL CONSTRAINT DF_RetDtl_Fine DEFAULT 0,
    Note           NVARCHAR(200) NULL,
    CONSTRAINT PK_ReturnDtl       PRIMARY KEY (ReturnDetailId),
    CONSTRAINT FK_RetDtl_Return   FOREIGN KEY (ReturnId)       REFERENCES dbo.ReturnReceipts(ReturnId) ON DELETE CASCADE,
    CONSTRAINT FK_RetDtl_Borrow   FOREIGN KEY (BorrowDetailId) REFERENCES dbo.BorrowReceiptDetails(BorrowDetailId),
    CONSTRAINT CK_RetDtl_Qty      CHECK (Quantity > 0),
    CONSTRAINT CK_RetDtl_Fine     CHECK (Fine >= 0),
    CONSTRAINT CK_RetDtl_Cond     CHECK (Condition IN ('Good','Damaged','Lost'))
);
GO

-- 2.12 Reservations (đặt trước sách) ------------------------------------------
CREATE TABLE dbo.Reservations (
    ReservationId INT IDENTITY(1,1) NOT NULL,
    ReaderId      INT NOT NULL,
    BookId        INT NOT NULL,
    ReservedAt    DATETIME2(0) NOT NULL CONSTRAINT DF_Res_ReservedAt DEFAULT SYSUTCDATETIME(),
    ExpiresAt     DATETIME2(0) NOT NULL,
    Status        VARCHAR(20)  NOT NULL CONSTRAINT DF_Res_Status DEFAULT 'Pending',
    FulfilledAt   DATETIME2(0) NULL,
    Note          NVARCHAR(200) NULL,
    CONSTRAINT PK_Reservations PRIMARY KEY (ReservationId),
    CONSTRAINT FK_Res_Readers  FOREIGN KEY (ReaderId) REFERENCES dbo.Readers(ReaderId),
    CONSTRAINT FK_Res_Books    FOREIGN KEY (BookId)   REFERENCES dbo.Books(BookId),
    CONSTRAINT CK_Res_Status   CHECK (Status IN ('Pending','Fulfilled','Expired','Cancelled')),
    CONSTRAINT CK_Res_Expire   CHECK (ExpiresAt > ReservedAt)
);
GO

-- 2.13 Notifications ----------------------------------------------------------
CREATE TABLE dbo.Notifications (
    NotificationId INT IDENTITY(1,1) NOT NULL,
    RecipientType  VARCHAR(20) NOT NULL,    -- 'User' / 'Reader'
    RecipientId    INT NOT NULL,
    NotifyType     VARCHAR(30) NOT NULL,    -- 'DueSoon','Overdue','Reservation','Penalty','System'
    Title          NVARCHAR(200) NOT NULL,
    Body           NVARCHAR(MAX) NULL,
    IsRead         BIT NOT NULL CONSTRAINT DF_Notif_IsRead DEFAULT 0,
    CreatedAt      DATETIME2(0) NOT NULL CONSTRAINT DF_Notif_CreatedAt DEFAULT SYSUTCDATETIME(),
    ReadAt         DATETIME2(0) NULL,
    CONSTRAINT PK_Notifications     PRIMARY KEY (NotificationId),
    CONSTRAINT CK_Notif_Recipient   CHECK (RecipientType IN ('User','Reader')),
    CONSTRAINT CK_Notif_Type        CHECK (NotifyType IN ('DueSoon','Overdue','Reservation','Penalty','System'))
);
GO

-- 2.14 PenaltyHistory (lịch sử thu phạt) --------------------------------------
CREATE TABLE dbo.PenaltyHistory (
    PenaltyId     INT IDENTITY(1,1) NOT NULL,
    BorrowId      INT NOT NULL,
    ReaderId      INT NOT NULL,
    Amount        DECIMAL(12,2) NOT NULL,
    Reason        NVARCHAR(200) NOT NULL,
    PaidAt        DATETIME2(0) NULL,
    CollectedBy   INT NULL,
    PaymentMethod VARCHAR(20) NULL,
    Note          NVARCHAR(300) NULL,
    CreatedAt     DATETIME2(0) NOT NULL CONSTRAINT DF_Penalty_CreatedAt DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_PenaltyHistory PRIMARY KEY (PenaltyId),
    CONSTRAINT FK_Penalty_Borrow FOREIGN KEY (BorrowId)    REFERENCES dbo.BorrowReceipts(BorrowId),
    CONSTRAINT FK_Penalty_Reader FOREIGN KEY (ReaderId)    REFERENCES dbo.Readers(ReaderId),
    CONSTRAINT FK_Penalty_User   FOREIGN KEY (CollectedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT CK_Penalty_Amount CHECK (Amount >= 0),
    CONSTRAINT CK_Penalty_Method CHECK (PaymentMethod IS NULL OR PaymentMethod IN ('Cash','Transfer','Card'))
);
GO

-- 2.15 AuditLogs --------------------------------------------------------------
CREATE TABLE dbo.AuditLogs (
    AuditId      BIGINT IDENTITY(1,1) NOT NULL,
    UserId       INT NULL,
    Action       VARCHAR(50)  NOT NULL,   -- INSERT/UPDATE/DELETE/LOGIN/LOGIN_FAILED/LOGOUT
    EntityType   VARCHAR(50)  NOT NULL,
    EntityId     VARCHAR(50)  NULL,
    OldValues    NVARCHAR(MAX) NULL,      -- JSON
    NewValues    NVARCHAR(MAX) NULL,      -- JSON
    IpAddress    VARCHAR(45)  NULL,
    UserAgent    NVARCHAR(300) NULL,
    Description  NVARCHAR(500) NULL,
    CreatedAt    DATETIME2(0) NOT NULL CONSTRAINT DF_Audit_CreatedAt DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_AuditLogs PRIMARY KEY (AuditId)
);
GO

-- 2.16 SystemSettings (config động) -------------------------------------------
CREATE TABLE dbo.SystemSettings (
    SettingKey   VARCHAR(50) NOT NULL,
    SettingValue NVARCHAR(500) NOT NULL,
    DataType     VARCHAR(20)  NOT NULL CONSTRAINT DF_Setting_DataType DEFAULT 'string',
    Description  NVARCHAR(300) NULL,
    UpdatedAt    DATETIME2(0) NULL,
    UpdatedBy    INT NULL,
    CONSTRAINT PK_SystemSettings PRIMARY KEY (SettingKey),
    CONSTRAINT CK_Setting_DataType CHECK (DataType IN ('string','int','decimal','bool','date'))
);
GO


/* =============================================================================
   KHỐI 3: INDEX – tối ưu cho tìm kiếm, thống kê và dashboard
   ============================================================================= */

-- Books
CREATE INDEX IX_Books_Title       ON dbo.Books(Title)  WHERE IsDeleted = 0;
CREATE INDEX IX_Books_Author      ON dbo.Books(Author) WHERE IsDeleted = 0;
CREATE INDEX IX_Books_Search      ON dbo.Books(IsDeleted, Status, CategoryId)
    INCLUDE (BookCode, Title, Author, AvailableQty, Quantity);
CREATE INDEX IX_Books_Status      ON dbo.Books(Status) WHERE Status <> 'Retired' AND IsDeleted = 0;

-- Readers
CREATE INDEX IX_Readers_FullName  ON dbo.Readers(FullName) WHERE IsDeleted = 0;
CREATE INDEX IX_Readers_Phone     ON dbo.Readers(Phone)    WHERE IsDeleted = 0;

-- BorrowReceipts
CREATE INDEX IX_Borrow_Reader_Status_Date ON dbo.BorrowReceipts(ReaderId, Status, BorrowDate DESC)
    INCLUDE (ReceiptCode, DueDate, TotalFine) WHERE IsDeleted = 0;
CREATE INDEX IX_Borrow_BorrowDate ON dbo.BorrowReceipts(BorrowDate DESC) WHERE IsDeleted = 0;
CREATE INDEX IX_Borrow_DueDate    ON dbo.BorrowReceipts(DueDate, Status)
    WHERE Status IN ('Borrowing','PartiallyReturned');
CREATE INDEX IX_Borrow_Status     ON dbo.BorrowReceipts(Status)
    WHERE Status IN ('Borrowing','Overdue','PartiallyReturned');

-- BorrowReceiptDetails
CREATE INDEX IX_BorrowDtl_BookId ON dbo.BorrowReceiptDetails(BookId) INCLUDE (Quantity, ReturnedQty);

-- ReturnReceipts
CREATE INDEX IX_Return_ReturnDate ON dbo.ReturnReceipts(ReturnDate DESC) WHERE IsDeleted = 0;

-- AuditLogs
CREATE INDEX IX_Audit_User_Date   ON dbo.AuditLogs(UserId, CreatedAt DESC);
CREATE INDEX IX_Audit_Entity_Date ON dbo.AuditLogs(EntityType, EntityId, CreatedAt DESC);

-- Notifications
CREATE INDEX IX_Notif_Recipient_Unread ON dbo.Notifications(RecipientType, RecipientId, IsRead)
    INCLUDE (CreatedAt, Title);

-- Reservations
CREATE INDEX IX_Reservations_Book_Status ON dbo.Reservations(BookId, Status);

-- PenaltyHistory
CREATE INDEX IX_Penalty_PaidAt ON dbo.PenaltyHistory(PaidAt DESC) WHERE PaidAt IS NOT NULL;
GO


/* =============================================================================
   KHỐI 4: DỮ LIỆU MẪU
   ============================================================================= */

-- 4.1 Roles
INSERT INTO dbo.Roles (RoleCode, RoleName, Description) VALUES
('ADMIN',     N'Quản trị viên', N'Toàn quyền hệ thống'),
('LIBRARIAN', N'Thủ thư',       N'Quản lý mượn/trả, sách, độc giả'),
('VIEWER',    N'Người xem',     N'Chỉ xem báo cáo, không sửa');

-- 4.2 Permissions
INSERT INTO dbo.Permissions (PermissionCode, Module, Description) VALUES
('BOOK_VIEW',     'BOOK',   N'Xem danh sách sách'),
('BOOK_CREATE',   'BOOK',   N'Thêm sách mới'),
('BOOK_UPDATE',   'BOOK',   N'Sửa thông tin sách'),
('BOOK_DELETE',   'BOOK',   N'Xoá sách'),
('READER_VIEW',   'READER', N'Xem độc giả'),
('READER_CREATE', 'READER', N'Thêm độc giả'),
('READER_UPDATE', 'READER', N'Sửa độc giả'),
('READER_DELETE', 'READER', N'Xoá độc giả'),
('BORROW_CREATE', 'BORROW', N'Lập phiếu mượn'),
('BORROW_RETURN', 'BORROW', N'Ghi nhận trả sách'),
('BORROW_CANCEL', 'BORROW', N'Huỷ phiếu mượn'),
('REPORT_VIEW',   'REPORT', N'Xem báo cáo thống kê'),
('REPORT_EXPORT', 'REPORT', N'Xuất báo cáo Excel'),
('USER_MANAGE',   'USER',   N'Quản lý tài khoản'),
('SETTING_MANAGE','SYSTEM', N'Cấu hình hệ thống');

-- 4.3 RolePermissions
-- ADMIN: toàn quyền
INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
SELECT 1, PermissionId FROM dbo.Permissions;

-- LIBRARIAN: nghiệp vụ + xem báo cáo
INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
SELECT 2, PermissionId FROM dbo.Permissions
WHERE PermissionCode IN ('BOOK_VIEW','BOOK_CREATE','BOOK_UPDATE',
                          'READER_VIEW','READER_CREATE','READER_UPDATE',
                          'BORROW_CREATE','BORROW_RETURN','BORROW_CANCEL',
                          'REPORT_VIEW','REPORT_EXPORT');

-- VIEWER: chỉ xem
INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
SELECT 3, PermissionId FROM dbo.Permissions
WHERE PermissionCode IN ('BOOK_VIEW','READER_VIEW','REPORT_VIEW');

-- 4.4 Users (PasswordHash là placeholder, hash thật bằng BCrypt khi đăng nhập từ app)
INSERT INTO dbo.Users (Username, PasswordHash, FullName, Email, Phone, RoleId, PasswordChangedAt) VALUES
('admin',   '$2a$12$qKYxRUk95Nlqim6TraJX4Og.DCQcXvtO8Q570dgGjsZXFIQBINAjG',   N'Nguyễn Quản Trị', 'admin@lib.vn',  '0900000001', 1, SYSUTCDATETIME()),
('thuthu1', '$2a$12$qKYxRUk95Nlqim6TraJX4Og.DCQcXvtO8Q570dgGjsZXFIQBINAjG',   N'Trần Thị Thư',    'thu1@lib.vn',   '0900000002', 2, SYSUTCDATETIME()),
('thuthu2', '$2a$12$qKYxRUk95Nlqim6TraJX4Og.DCQcXvtO8Q570dgGjsZXFIQBINAjG',   N'Lê Văn Mượn',     'thu2@lib.vn',   '0900000003', 2, SYSUTCDATETIME()),
('viewer1', '$2a$12$qKYxRUk95Nlqim6TraJX4Og.DCQcXvtO8Q570dgGjsZXFIQBINAjG',      N'Phạm Quan Sát',   'view@lib.vn',   '0900000004', 3, SYSUTCDATETIME());

-- 4.5 Categories
INSERT INTO dbo.Categories (CategoryCode, CategoryName, Description) VALUES
('CNTT', N'Công nghệ thông tin', N'Sách lập trình, mạng, AI'),
('KT',   N'Kinh tế',             N'Quản trị, marketing, tài chính'),
('VH',   N'Văn học',             N'Tiểu thuyết, thơ, truyện ngắn'),
('NN',   N'Ngoại ngữ',           N'Tiếng Anh, tiếng Nhật, tiếng Trung'),
('TN',   N'Khoa học tự nhiên',   N'Toán, lý, hoá, sinh');

-- 4.6 Books
INSERT INTO dbo.Books (BookCode, Title, Author, Publisher, PublishYear, CategoryId, Quantity, AvailableQty, Price) VALUES
('B001', N'C# Programming In-depth',         N'Nguyễn Văn A',   N'NXB Bách Khoa', 2022, 1, 10, 10, 180000),
('B002', N'Lập trình WinForms .NET',         N'Trần B',         N'NXB Bách Khoa', 2023, 1,  8,  8, 150000),
('B003', N'SQL Server từ A đến Z',           N'Lê C',           N'NXB Thống Kê',  2021, 1,  6,  6, 165000),
('B004', N'Marketing căn bản',               N'Phạm D',         N'NXB Kinh Tế',   2020, 2,  5,  5, 120000),
('B005', N'Quản trị tài chính doanh nghiệp', N'Đỗ E',           N'NXB Tài Chính', 2022, 2,  4,  4, 140000),
('B006', N'Số đỏ',                           N'Vũ Trọng Phụng', N'NXB Văn Học',   2019, 3,  7,  7,  80000),
('B007', N'Truyện Kiều',                     N'Nguyễn Du',      N'NXB Văn Học',   2018, 3,  9,  9,  70000),
('B008', N'TOEIC 900 Practice',              N'Mai F',          N'NXB Tổng Hợp',  2023, 4,  6,  6, 200000),
('B009', N'Minna no Nihongo I',              N'3A Corporation', N'NXB Trẻ',       2021, 4,  5,  5, 220000),
('B010', N'Giải tích 1',                     N'Hoàng G',        N'NXB Giáo Dục',  2020, 5, 12, 12,  90000);

-- 4.7 Readers
INSERT INTO dbo.Readers (CardNumber, FullName, DateOfBirth, Gender, Address, Phone, Email, CardIssueDate, CardExpireDate) VALUES
('TV001', N'Nguyễn Thị Hoa', '2003-05-10', N'Nữ',  N'Hà Nội',    '0911000001', 'hoa@gmail.com',  '2025-01-01', '2027-01-01'),
('TV002', N'Trần Văn Minh',  '2002-08-20', N'Nam', N'Hà Nội',    '0911000002', 'minh@gmail.com', '2025-01-15', '2027-01-15'),
('TV003', N'Phạm Thị Lan',   '2004-02-12', N'Nữ',  N'Bắc Ninh',  '0911000003', 'lan@gmail.com',  '2025-02-01', '2027-02-01'),
('TV004', N'Lê Quốc Hùng',   '2001-11-05', N'Nam', N'Hải Phòng', '0911000004', 'hung@gmail.com', '2024-09-01', '2026-09-01'),
('TV005', N'Đặng Mỹ Linh',   '2003-07-18', N'Nữ',  N'Hà Nội',    '0911000005', 'linh@gmail.com', '2025-03-10', '2027-03-10');

-- 4.8 BorrowReceipts
INSERT INTO dbo.BorrowReceipts (ReceiptCode, ReaderId, UserId, BorrowDate, DueDate, Status, Note) VALUES
('PM2026001', 1, 2, '2026-04-20', '2026-05-04', 'Returned',          N'Đã trả đầy đủ'),
('PM2026002', 2, 2, '2026-04-25', '2026-05-09', 'PartiallyReturned', N'Trả 1/2 sách'),
('PM2026003', 3, 3, '2026-05-01', '2026-05-15', 'Borrowing',         NULL),
('PM2026004', 4, 2, '2026-04-10', '2026-04-24', 'Overdue',           N'Quá hạn chưa trả'),
('PM2026005', 5, 3, '2026-05-05', '2026-05-19', 'Borrowing',         NULL);

-- 4.9 BorrowReceiptDetails
INSERT INTO dbo.BorrowReceiptDetails (BorrowId, BookId, Quantity, ReturnedQty) VALUES
(1, 1, 1, 1),
(1, 6, 1, 1),
(2, 2, 1, 1),
(2, 3, 1, 0),
(3, 8, 1, 0),
(3, 9, 1, 0),
(4, 4, 1, 0),
(5, 7, 1, 0),
(5,10, 1, 0);

UPDATE dbo.Books SET AvailableQty = AvailableQty - 1 WHERE BookId IN (3, 8, 9, 4, 7, 10);

-- 4.10 ReturnReceipts
INSERT INTO dbo.ReturnReceipts (ReturnCode, BorrowId, UserId, ReturnDate, TotalFine, Note) VALUES
('PT2026001', 1, 2, '2026-05-03', 0,    N'Trả đúng hạn'),
('PT2026002', 2, 2, '2026-05-08', 5000, N'Trả 1 cuốn, hỏng nhẹ');

-- 4.11 ReturnReceiptDetails
INSERT INTO dbo.ReturnReceiptDetails (ReturnId, BorrowDetailId, Quantity, Condition, Fine) VALUES
(1, 1, 1, 'Good',    0),
(1, 2, 1, 'Good',    0),
(2, 3, 1, 'Damaged', 5000);

UPDATE dbo.BorrowReceipts SET TotalFine = 5000 WHERE BorrowId = 2;

-- 4.12 PenaltyHistory (lịch sử thu phạt)
INSERT INTO dbo.PenaltyHistory (BorrowId, ReaderId, Amount, Reason, PaidAt, CollectedBy, PaymentMethod) VALUES
(2, 2, 5000, N'Trả sách bị hỏng', '2026-05-08', 2, 'Cash');

-- 4.13 Reservations
INSERT INTO dbo.Reservations (ReaderId, BookId, ExpiresAt, Status, Note) VALUES
(3, 1, DATEADD(DAY, 7, SYSUTCDATETIME()), 'Pending', N'Đặt trước sách C#');

-- 4.14 Notifications
INSERT INTO dbo.Notifications (RecipientType, RecipientId, NotifyType, Title, Body) VALUES
('Reader', 4, 'Overdue', N'Sách quá hạn', N'Bạn có sách quá hạn từ 2026-04-24, vui lòng trả gấp.'),
('User',   1, 'System',  N'Chào mừng',    N'Hệ thống đã sẵn sàng.');

-- 4.15 SystemSettings (config động)
INSERT INTO dbo.SystemSettings (SettingKey, SettingValue, DataType, Description) VALUES
('MaxBorrowPerReader',  '5',     'int',     N'Số sách tối đa được mượn cùng lúc'),
('DefaultBorrowDays',   '14',    'int',     N'Số ngày mượn mặc định'),
('FinePerDay',          '5000',  'decimal', N'Tiền phạt quá hạn / ngày (VNĐ)'),
('MaxRenewCount',       '2',     'int',     N'Số lần gia hạn tối đa'),
('LockoutThreshold',    '5',     'int',     N'Số lần đăng nhập sai bị khoá'),
('LockoutMinutes',      '15',    'int',     N'Thời gian khoá (phút)'),
('DueSoonDays',         '2',     'int',     N'Số ngày trước hạn để gửi thông báo'),
('ReservationDays',     '7',     'int',     N'Thời hạn giữ chỗ đặt trước (ngày)');
GO


/* =============================================================================
   KHỐI 5: USER-DEFINED TABLE TYPES (cho Stored Procedure)
   ============================================================================= */

IF TYPE_ID('dbo.BorrowItemList') IS NOT NULL DROP TYPE dbo.BorrowItemList;
GO
CREATE TYPE dbo.BorrowItemList AS TABLE (
    BookId   INT NOT NULL,
    Quantity INT NOT NULL
);
GO

IF TYPE_ID('dbo.ReturnItemList') IS NOT NULL DROP TYPE dbo.ReturnItemList;
GO
CREATE TYPE dbo.ReturnItemList AS TABLE (
    BorrowDetailId INT NOT NULL,
    Quantity       INT NOT NULL,
    Condition      VARCHAR(20) NOT NULL,
    Fine           DECIMAL(12,2) NOT NULL DEFAULT 0
);
GO


/* =============================================================================
   KHỐI 6: VIEW (báo cáo + dashboard)
   ============================================================================= */

-- 6.1 Tổng quan sách (kèm danh mục)
CREATE OR ALTER VIEW dbo.vw_BookOverview AS
SELECT  b.BookId, b.BookCode, b.Title, b.Author, b.Publisher, b.PublishYear,
        c.CategoryName,
        b.Quantity, b.AvailableQty,
        (b.Quantity - b.AvailableQty) AS BorrowedQty,
        b.Price, b.Status
FROM dbo.Books b
JOIN dbo.Categories c ON c.CategoryId = b.CategoryId
WHERE b.IsDeleted = 0;
GO

-- 6.2 Tổng hợp phiếu mượn
CREATE OR ALTER VIEW dbo.vw_BorrowSummary AS
SELECT  br.BorrowId, br.ReceiptCode,
        r.CardNumber, r.FullName AS ReaderName,
        u.FullName AS LibrarianName,
        br.BorrowDate, br.DueDate, br.Status, br.TotalFine,
        SUM(d.Quantity)                  AS TotalBooks,
        SUM(d.Quantity - d.ReturnedQty)  AS NotReturnedYet
FROM dbo.BorrowReceipts br
JOIN dbo.Readers r              ON r.ReaderId = br.ReaderId
JOIN dbo.Users u                ON u.UserId   = br.UserId
JOIN dbo.BorrowReceiptDetails d ON d.BorrowId = br.BorrowId
WHERE br.IsDeleted = 0
GROUP BY br.BorrowId, br.ReceiptCode, r.CardNumber, r.FullName,
         u.FullName, br.BorrowDate, br.DueDate, br.Status, br.TotalFine;
GO

-- 6.3 Phiếu mượn quá hạn
CREATE OR ALTER VIEW dbo.vw_OverdueBorrows AS
SELECT  br.BorrowId, br.ReceiptCode, r.CardNumber, r.FullName AS ReaderName,
        r.Phone, r.Email,
        br.BorrowDate, br.DueDate,
        DATEDIFF(DAY, br.DueDate, CAST(SYSUTCDATETIME() AS DATE)) AS DaysOverdue,
        br.Status, br.TotalFine
FROM dbo.BorrowReceipts br
JOIN dbo.Readers r ON r.ReaderId = br.ReaderId
WHERE br.IsDeleted = 0
  AND br.Status IN ('Borrowing','PartiallyReturned','Overdue')
  AND br.DueDate < CAST(SYSUTCDATETIME() AS DATE);
GO

-- 6.4 Top sách mượn nhiều nhất
CREATE OR ALTER VIEW dbo.vw_TopBorrowedBooks AS
SELECT TOP (100) b.BookId, b.BookCode, b.Title, b.Author,
       c.CategoryName,
       SUM(d.Quantity) AS TotalBorrowed,
       COUNT(DISTINCT br.ReaderId) AS UniqueReaders
FROM dbo.BorrowReceiptDetails d
JOIN dbo.BorrowReceipts br ON br.BorrowId = d.BorrowId AND br.IsDeleted = 0
JOIN dbo.Books b           ON b.BookId    = d.BookId
JOIN dbo.Categories c      ON c.CategoryId = b.CategoryId
GROUP BY b.BookId, b.BookCode, b.Title, b.Author, c.CategoryName
ORDER BY TotalBorrowed DESC;
GO

-- 6.5 KPI dashboard (load 1 lần khi mở dashboard)
CREATE OR ALTER VIEW dbo.vw_Dashboard_KPI AS
SELECT
    (SELECT COUNT(*) FROM dbo.Books   WHERE IsDeleted = 0 AND Status <> 'Retired')             AS TotalBooks,
    (SELECT ISNULL(SUM(Quantity),0)     FROM dbo.Books WHERE IsDeleted = 0)                    AS TotalCopies,
    (SELECT ISNULL(SUM(Quantity - AvailableQty),0) FROM dbo.Books WHERE IsDeleted = 0)         AS TotalBorrowedNow,
    (SELECT COUNT(*) FROM dbo.Readers WHERE IsDeleted = 0 AND Status = 'Active')               AS ActiveReaders,
    (SELECT COUNT(*) FROM dbo.BorrowReceipts
        WHERE IsDeleted = 0 AND Status IN ('Borrowing','PartiallyReturned'))                   AS ActiveBorrows,
    (SELECT COUNT(*) FROM dbo.BorrowReceipts
        WHERE IsDeleted = 0 AND Status = 'Overdue')                                            AS OverdueBorrows,
    (SELECT ISNULL(SUM(Amount),0) FROM dbo.PenaltyHistory
        WHERE PaidAt >= DATEADD(DAY,-30, SYSUTCDATETIME()))                                    AS FineRevenue30d,
    (SELECT COUNT(*) FROM dbo.Reservations WHERE Status = 'Pending')                           AS PendingReservations;
GO

-- 6.6 Xu hướng mượn 30 ngày gần nhất (line chart)
CREATE OR ALTER VIEW dbo.vw_Dashboard_BorrowTrend AS
SELECT TOP (30)
       br.BorrowDate,
       COUNT(DISTINCT br.BorrowId)   AS Receipts,
       SUM(d.Quantity)               AS Books
FROM dbo.BorrowReceipts br
JOIN dbo.BorrowReceiptDetails d ON d.BorrowId = br.BorrowId
WHERE br.IsDeleted = 0
  AND br.BorrowDate >= DATEADD(DAY,-30, CAST(SYSUTCDATETIME() AS DATE))
GROUP BY br.BorrowDate
ORDER BY br.BorrowDate DESC;
GO

-- 6.7 Phân bố sách theo danh mục (pie chart)
CREATE OR ALTER VIEW dbo.vw_Dashboard_CategoryDistribution AS
SELECT c.CategoryName,
       COUNT(b.BookId)             AS TitleCount,
       ISNULL(SUM(b.Quantity),0)   AS CopyCount,
       ISNULL(SUM(b.Quantity - b.AvailableQty),0) AS BorrowedCount
FROM dbo.Categories c
LEFT JOIN dbo.Books b ON b.CategoryId = c.CategoryId AND b.IsDeleted = 0
WHERE c.IsActive = 1 AND c.IsDeleted = 0
GROUP BY c.CategoryName;
GO


/* =============================================================================
   KHỐI 7: STORED PROCEDURE NGHIỆP VỤ
   ============================================================================= */

-- 7.1 Thêm sách --------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_Book_Insert
    @BookCode    VARCHAR(30),
    @Title       NVARCHAR(200),
    @Author      NVARCHAR(150),
    @Publisher   NVARCHAR(150) = NULL,
    @PublishYear INT = NULL,
    @CategoryId  INT,
    @Quantity    INT = 0,
    @Price       DECIMAL(12,2) = 0,
    @CreatedBy   INT = NULL,
    @NewBookId   INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM dbo.Books WHERE BookCode = @BookCode AND IsDeleted = 0)
            THROW 50010, N'Mã sách đã tồn tại.', 1;

        INSERT INTO dbo.Books (BookCode, Title, Author, Publisher, PublishYear,
                               CategoryId, Quantity, AvailableQty, Price, Status, CreatedBy)
        VALUES (@BookCode, @Title, @Author, @Publisher, @PublishYear,
                @CategoryId, @Quantity, @Quantity, @Price,
                CASE WHEN @Quantity > 0 THEN 'Available' ELSE 'OutOfStock' END,
                @CreatedBy);

        SET @NewBookId = SCOPE_IDENTITY();

        INSERT INTO dbo.AuditLogs (UserId, Action, EntityType, EntityId, NewValues, Description)
        VALUES (@CreatedBy, 'INSERT', 'Book', CAST(@NewBookId AS VARCHAR(50)),
                (SELECT @BookCode AS BookCode, @Title AS Title, @Author AS Author,
                        @Quantity AS Quantity, @Price AS Price
                 FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
                N'Thêm sách mới');

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- 7.2 Sửa sách (có optimistic concurrency qua @RowVer) -----------------------
CREATE OR ALTER PROCEDURE dbo.sp_Book_Update
    @BookId      INT,
    @Title       NVARCHAR(200),
    @Author      NVARCHAR(150),
    @Publisher   NVARCHAR(150) = NULL,
    @PublishYear INT = NULL,
    @CategoryId  INT,
    @Quantity    INT,
    @Price       DECIMAL(12,2),
    @Status      VARCHAR(20),
    @RowVer      BINARY(8),                  -- để check concurrency
    @UpdatedBy   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @OldJson NVARCHAR(MAX), @CurrentRowVer BINARY(8), @CurrentBorrowed INT;

        SELECT @OldJson = (SELECT BookCode, Title, Author, Publisher, PublishYear,
                                  CategoryId, Quantity, AvailableQty, Price, Status
                           FROM dbo.Books WHERE BookId = @BookId
                           FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
               @CurrentRowVer   = RowVer,
               @CurrentBorrowed = Quantity - AvailableQty
        FROM dbo.Books WITH (UPDLOCK, ROWLOCK)
        WHERE BookId = @BookId AND IsDeleted = 0;

        IF @CurrentRowVer IS NULL
            THROW 50011, N'Không tìm thấy sách hoặc đã bị xoá.', 1;

        IF @CurrentRowVer <> @RowVer
            THROW 50012, N'Dữ liệu đã bị người khác thay đổi. Vui lòng tải lại.', 1;

        IF @Quantity < @CurrentBorrowed
            THROW 50013, N'Số lượng mới nhỏ hơn số sách đang được mượn.', 1;

        UPDATE dbo.Books
           SET Title        = @Title,
               Author       = @Author,
               Publisher    = @Publisher,
               PublishYear  = @PublishYear,
               CategoryId   = @CategoryId,
               Quantity     = @Quantity,
               AvailableQty = @Quantity - @CurrentBorrowed,
               Price        = @Price,
               Status       = @Status,
               UpdatedAt    = SYSUTCDATETIME(),
               UpdatedBy    = @UpdatedBy
         WHERE BookId = @BookId;

        INSERT INTO dbo.AuditLogs (UserId, Action, EntityType, EntityId, OldValues, NewValues, Description)
        VALUES (@UpdatedBy, 'UPDATE', 'Book', CAST(@BookId AS VARCHAR(50)),
                @OldJson,
                (SELECT @Title AS Title, @Author AS Author, @Quantity AS Quantity,
                        @Price AS Price, @Status AS Status
                 FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
                N'Cập nhật sách');

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- 7.3 Xoá sách (soft delete) -------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_Book_Delete
    @BookId    INT,
    @DeletedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (
            SELECT 1 FROM dbo.BorrowReceiptDetails d
            JOIN dbo.BorrowReceipts b ON b.BorrowId = d.BorrowId
            WHERE d.BookId = @BookId
              AND b.IsDeleted = 0
              AND b.Status IN ('Borrowing','PartiallyReturned','Overdue')
        )
            THROW 50014, N'Sách đang có người mượn, không thể xoá.', 1;

        UPDATE dbo.Books
           SET IsDeleted = 1,
               DeletedAt = SYSUTCDATETIME(),
               DeletedBy = @DeletedBy,
               Status    = 'Retired',
               UpdatedAt = SYSUTCDATETIME(),
               UpdatedBy = @DeletedBy
         WHERE BookId = @BookId AND IsDeleted = 0;

        IF @@ROWCOUNT = 0
            THROW 50015, N'Không tìm thấy sách hoặc đã bị xoá trước đó.', 1;

        INSERT INTO dbo.AuditLogs (UserId, Action, EntityType, EntityId, Description)
        VALUES (@DeletedBy, 'DELETE', 'Book', CAST(@BookId AS VARCHAR(50)), N'Xoá mềm sách');

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- 7.4 Tìm sách nâng cao ------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_Book_Search
    @Keyword    NVARCHAR(200) = NULL,
    @CategoryId INT           = NULL,
    @Status     VARCHAR(20)   = NULL,
    @YearFrom   INT           = NULL,
    @YearTo     INT           = NULL,
    @PageIndex  INT           = 1,
    @PageSize   INT           = 50
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Skip INT = (@PageIndex - 1) * @PageSize;

    -- Tổng số bản ghi (cho phân trang trên DataGridView)
    SELECT COUNT(*) AS TotalCount
    FROM dbo.Books b
    WHERE b.IsDeleted = 0
      AND (@Keyword    IS NULL OR b.Title    LIKE N'%' + @Keyword + N'%'
                                OR b.Author   LIKE N'%' + @Keyword + N'%'
                                OR b.BookCode LIKE       @Keyword + '%')
      AND (@CategoryId IS NULL OR b.CategoryId  = @CategoryId)
      AND (@Status     IS NULL OR b.Status      = @Status)
      AND (@YearFrom   IS NULL OR b.PublishYear >= @YearFrom)
      AND (@YearTo     IS NULL OR b.PublishYear <= @YearTo);

    -- Dữ liệu
    SELECT b.BookId, b.BookCode, b.Title, b.Author, b.Publisher,
           b.PublishYear, c.CategoryName, b.Quantity, b.AvailableQty,
           b.Price, b.Status, b.RowVer
    FROM dbo.Books b
    JOIN dbo.Categories c ON c.CategoryId = b.CategoryId
    WHERE b.IsDeleted = 0
      AND (@Keyword    IS NULL OR b.Title    LIKE N'%' + @Keyword + N'%'
                                OR b.Author   LIKE N'%' + @Keyword + N'%'
                                OR b.BookCode LIKE       @Keyword + '%')
      AND (@CategoryId IS NULL OR b.CategoryId  = @CategoryId)
      AND (@Status     IS NULL OR b.Status      = @Status)
      AND (@YearFrom   IS NULL OR b.PublishYear >= @YearFrom)
      AND (@YearTo     IS NULL OR b.PublishYear <= @YearTo)
    ORDER BY b.Title
    OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- 7.5 Lập phiếu mượn (anti race-condition) -----------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_Borrow_Create
    @ReceiptCode VARCHAR(30),
    @ReaderId    INT,
    @UserId      INT,
    @BorrowDate  DATE,
    @DueDate     DATE,
    @Note        NVARCHAR(300) = NULL,
    @Items       dbo.BorrowItemList READONLY,
    @NewBorrowId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Đọc config động
        DECLARE @MaxAllowed INT =
            (SELECT CAST(SettingValue AS INT) FROM dbo.SystemSettings WHERE SettingKey = 'MaxBorrowPerReader');

        -- 2. Validate độc giả còn hiệu lực
        IF NOT EXISTS (
            SELECT 1 FROM dbo.Readers WITH (READCOMMITTEDLOCK)
            WHERE ReaderId = @ReaderId
              AND Status = 'Active'
              AND CardExpireDate >= @BorrowDate
              AND IsDeleted = 0
        )
            THROW 50020, N'Độc giả không hợp lệ hoặc thẻ đã hết hạn.', 1;

        -- 3. Kiểm tra giới hạn số sách (UPDLOCK + HOLDLOCK trên phiếu mượn)
        DECLARE @CurrentBorrowed INT;
        SELECT @CurrentBorrowed = ISNULL(SUM(d.Quantity - d.ReturnedQty), 0)
        FROM dbo.BorrowReceipts br WITH (UPDLOCK, HOLDLOCK)
        JOIN dbo.BorrowReceiptDetails d ON d.BorrowId = br.BorrowId
        WHERE br.ReaderId = @ReaderId
          AND br.Status IN ('Borrowing','PartiallyReturned','Overdue')
          AND br.IsDeleted = 0;

        DECLARE @RequestQty INT = (SELECT ISNULL(SUM(Quantity),0) FROM @Items);
        IF (@CurrentBorrowed + @RequestQty) > @MaxAllowed
            THROW 50021, N'Vượt quá số sách tối đa được mượn.', 1;

        -- 4. KHÓA HÀNG SÁCH bằng UPDLOCK + HOLDLOCK + ROWLOCK
        -- Đây là chốt chống race condition: 2 thủ thư cùng mượn 1 quyển sẽ tuần tự hoá
        IF EXISTS (
            SELECT 1
            FROM @Items i
            JOIN dbo.Books b WITH (UPDLOCK, HOLDLOCK, ROWLOCK) ON b.BookId = i.BookId
            WHERE b.AvailableQty < i.Quantity
               OR b.Status <> 'Available'
               OR b.IsDeleted = 1
        )
            THROW 50022, N'Một hoặc nhiều sách không đủ tồn kho hoặc không khả dụng.', 1;

        -- 5. Tạo phiếu
        INSERT INTO dbo.BorrowReceipts (ReceiptCode, ReaderId, UserId,
                                        BorrowDate, DueDate, Status, Note, CreatedBy)
        VALUES (@ReceiptCode, @ReaderId, @UserId,
                @BorrowDate, @DueDate, 'Borrowing', @Note, @UserId);

        SET @NewBorrowId = SCOPE_IDENTITY();

        INSERT INTO dbo.BorrowReceiptDetails (BorrowId, BookId, Quantity, ReturnedQty)
        SELECT @NewBorrowId, BookId, Quantity, 0 FROM @Items;

        -- 6. Trừ tồn kho
        UPDATE b
           SET b.AvailableQty = b.AvailableQty - i.Quantity,
               b.UpdatedAt    = SYSUTCDATETIME(),
               b.UpdatedBy    = @UserId
        FROM dbo.Books b
        JOIN @Items i ON i.BookId = b.BookId;

        -- 7. Audit
        INSERT INTO dbo.AuditLogs (UserId, Action, EntityType, EntityId, NewValues, Description)
        VALUES (@UserId, 'CREATE', 'BorrowReceipt', CAST(@NewBorrowId AS VARCHAR(50)),
                (SELECT @ReceiptCode AS ReceiptCode, @ReaderId AS ReaderId,
                        @BorrowDate AS BorrowDate, @DueDate AS DueDate,
                        @RequestQty AS TotalQty
                 FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
                N'Lập phiếu mượn');

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- 7.6 Trả sách ---------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_Return_Create
    @ReturnCode  VARCHAR(30),
    @BorrowId    INT,
    @UserId      INT,
    @ReturnDate  DATE,
    @Note        NVARCHAR(300) = NULL,
    @Items       dbo.ReturnItemList READONLY,
    @NewReturnId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Validate phiếu mượn tồn tại
        IF NOT EXISTS (
            SELECT 1 FROM dbo.BorrowReceipts WITH (UPDLOCK, ROWLOCK)
            WHERE BorrowId = @BorrowId AND IsDeleted = 0
        )
            THROW 50030, N'Phiếu mượn không tồn tại.', 1;

        -- 2. Validate từng dòng trả không vượt quá số còn nợ
        IF EXISTS (
            SELECT 1
            FROM @Items i
            JOIN dbo.BorrowReceiptDetails d WITH (UPDLOCK, ROWLOCK) ON d.BorrowDetailId = i.BorrowDetailId
            WHERE d.BorrowId <> @BorrowId
               OR i.Quantity > (d.Quantity - d.ReturnedQty)
               OR i.Quantity <= 0
        )
            THROW 50031, N'Số lượng trả không hợp lệ hoặc dòng không thuộc phiếu mượn.', 1;

        DECLARE @TotalFine DECIMAL(12,2) = (SELECT ISNULL(SUM(Fine),0) FROM @Items);
        DECLARE @ReaderId  INT           = (SELECT ReaderId FROM dbo.BorrowReceipts WHERE BorrowId = @BorrowId);

        -- 3. Tạo phiếu trả
        INSERT INTO dbo.ReturnReceipts (ReturnCode, BorrowId, UserId,
                                        ReturnDate, TotalFine, Note, CreatedBy)
        VALUES (@ReturnCode, @BorrowId, @UserId, @ReturnDate, @TotalFine, @Note, @UserId);

        SET @NewReturnId = SCOPE_IDENTITY();

        INSERT INTO dbo.ReturnReceiptDetails (ReturnId, BorrowDetailId,
                                              Quantity, Condition, Fine)
        SELECT @NewReturnId, BorrowDetailId, Quantity, Condition, Fine FROM @Items;

        -- 4. Cập nhật ReturnedQty
        UPDATE d
           SET d.ReturnedQty = d.ReturnedQty + i.Quantity
        FROM dbo.BorrowReceiptDetails d
        JOIN @Items i ON i.BorrowDetailId = d.BorrowDetailId;

        -- 5. Cộng tồn kho cho sách Good/Damaged (Lost không cộng)
        UPDATE b
           SET b.AvailableQty = b.AvailableQty + x.QtyReturnable,
               b.UpdatedAt    = SYSUTCDATETIME(),
               b.UpdatedBy    = @UserId
        FROM dbo.Books b
        JOIN (
            SELECT d.BookId,
                   SUM(CASE WHEN i.Condition IN ('Good','Damaged') THEN i.Quantity ELSE 0 END) AS QtyReturnable
            FROM @Items i
            JOIN dbo.BorrowReceiptDetails d ON d.BorrowDetailId = i.BorrowDetailId
            GROUP BY d.BookId
        ) x ON x.BookId = b.BookId
        WHERE x.QtyReturnable > 0;

        -- 6. Cộng dồn tiền phạt
        UPDATE dbo.BorrowReceipts
           SET TotalFine = TotalFine + @TotalFine,
               UpdatedAt = SYSUTCDATETIME(),
               UpdatedBy = @UserId
         WHERE BorrowId = @BorrowId;

        -- 7. Cập nhật trạng thái phiếu mượn
        DECLARE @Remaining INT;
        SELECT @Remaining = SUM(Quantity - ReturnedQty)
        FROM dbo.BorrowReceiptDetails
        WHERE BorrowId = @BorrowId;

        UPDATE dbo.BorrowReceipts
           SET Status = CASE WHEN @Remaining = 0 THEN 'Returned' ELSE 'PartiallyReturned' END,
               UpdatedAt = SYSUTCDATETIME(),
               UpdatedBy = @UserId
         WHERE BorrowId = @BorrowId;

        -- 8. Ghi vào PenaltyHistory nếu có phạt
        IF @TotalFine > 0
        BEGIN
            INSERT INTO dbo.PenaltyHistory (BorrowId, ReaderId, Amount, Reason, PaidAt, CollectedBy, PaymentMethod)
            VALUES (@BorrowId, @ReaderId, @TotalFine, N'Phạt khi trả sách', @ReturnDate, @UserId, 'Cash');
        END

        -- 9. Audit
        INSERT INTO dbo.AuditLogs (UserId, Action, EntityType, EntityId, NewValues, Description)
        VALUES (@UserId, 'CREATE', 'ReturnReceipt', CAST(@NewReturnId AS VARCHAR(50)),
                (SELECT @ReturnCode AS ReturnCode, @BorrowId AS BorrowId,
                        @ReturnDate AS ReturnDate, @TotalFine AS TotalFine
                 FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
                N'Ghi nhận trả sách');

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO


/* =============================================================================
   KHỐI 8: STORED PROCEDURE THỐNG KÊ
   ============================================================================= */

-- 8.1 Sách đang được mượn
CREATE OR ALTER PROCEDURE dbo.sp_Stat_BooksCurrentlyBorrowed
    @CategoryId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  b.BookId, b.BookCode, b.Title, b.Author,
            c.CategoryName,
            b.Quantity, b.AvailableQty,
            (b.Quantity - b.AvailableQty) AS BorrowedQty
    FROM dbo.Books b
    JOIN dbo.Categories c ON c.CategoryId = b.CategoryId
    WHERE b.IsDeleted = 0
      AND b.Status <> 'Retired'
      AND (@CategoryId IS NULL OR b.CategoryId = @CategoryId)
      AND (b.Quantity - b.AvailableQty) > 0
    ORDER BY BorrowedQty DESC, b.Title;
END
GO

-- 8.2 Top sách mượn nhiều nhất
CREATE OR ALTER PROCEDURE dbo.sp_Stat_TopBorrowedBooks
    @FromDate DATE = NULL,
    @ToDate   DATE = NULL,
    @TopN     INT  = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@TopN)
           b.BookId, b.BookCode, b.Title, b.Author,
           c.CategoryName,
           SUM(d.Quantity)             AS TotalBorrowed,
           COUNT(DISTINCT br.ReaderId) AS UniqueReaders
    FROM dbo.BorrowReceiptDetails d
    JOIN dbo.BorrowReceipts br ON br.BorrowId = d.BorrowId AND br.IsDeleted = 0
    JOIN dbo.Books b           ON b.BookId    = d.BookId
    JOIN dbo.Categories c      ON c.CategoryId = b.CategoryId
    WHERE (@FromDate IS NULL OR br.BorrowDate >= @FromDate)
      AND (@ToDate   IS NULL OR br.BorrowDate <= @ToDate)
    GROUP BY b.BookId, b.BookCode, b.Title, b.Author, c.CategoryName
    ORDER BY TotalBorrowed DESC;
END
GO

-- 8.3 Top độc giả hoạt động nhiều nhất
CREATE OR ALTER PROCEDURE dbo.sp_Stat_TopActiveReaders
    @FromDate DATE = NULL,
    @ToDate   DATE = NULL,
    @TopN     INT  = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@TopN)
           r.ReaderId, r.CardNumber, r.FullName, r.Phone,
           COUNT(DISTINCT br.BorrowId) AS TotalReceipts,
           SUM(d.Quantity)             AS TotalBooks,
           SUM(br.TotalFine)           AS TotalFinePaid,
           MAX(br.BorrowDate)          AS LastBorrowDate
    FROM dbo.Readers r
    JOIN dbo.BorrowReceipts br      ON br.ReaderId = r.ReaderId AND br.IsDeleted = 0
    JOIN dbo.BorrowReceiptDetails d ON d.BorrowId  = br.BorrowId
    WHERE r.IsDeleted = 0
      AND (@FromDate IS NULL OR br.BorrowDate >= @FromDate)
      AND (@ToDate   IS NULL OR br.BorrowDate <= @ToDate)
    GROUP BY r.ReaderId, r.CardNumber, r.FullName, r.Phone
    ORDER BY TotalBooks DESC, TotalReceipts DESC;
END
GO

-- 8.4 Doanh thu tiền phạt theo thời gian
CREATE OR ALTER PROCEDURE dbo.sp_Stat_FineRevenue
    @FromDate  DATE,
    @ToDate    DATE,
    @GroupBy   VARCHAR(10) = 'Day'         -- Day / Month / Year
AS
BEGIN
    SET NOCOUNT ON;

    IF @GroupBy = 'Day'
        SELECT CAST(PaidAt AS DATE)           AS Period,
               COUNT(*)                        AS Transactions,
               SUM(Amount)                     AS Revenue
        FROM dbo.PenaltyHistory
        WHERE PaidAt IS NOT NULL
          AND PaidAt >= @FromDate
          AND PaidAt <  DATEADD(DAY, 1, @ToDate)
        GROUP BY CAST(PaidAt AS DATE)
        ORDER BY Period;
    ELSE IF @GroupBy = 'Month'
        SELECT FORMAT(PaidAt,'yyyy-MM')        AS Period,
               COUNT(*)                        AS Transactions,
               SUM(Amount)                     AS Revenue
        FROM dbo.PenaltyHistory
        WHERE PaidAt IS NOT NULL
          AND PaidAt >= @FromDate
          AND PaidAt <  DATEADD(DAY, 1, @ToDate)
        GROUP BY FORMAT(PaidAt,'yyyy-MM')
        ORDER BY Period;
    ELSE
        SELECT YEAR(PaidAt)                    AS Period,
               COUNT(*)                        AS Transactions,
               SUM(Amount)                     AS Revenue
        FROM dbo.PenaltyHistory
        WHERE PaidAt IS NOT NULL
          AND PaidAt >= @FromDate
          AND PaidAt <  DATEADD(DAY, 1, @ToDate)
        GROUP BY YEAR(PaidAt)
        ORDER BY Period;
END
GO

-- 8.5 Sách quá hạn chi tiết (tính tiền phạt ước tính)
CREATE OR ALTER PROCEDURE dbo.sp_Stat_OverdueBooks
    @AsOfDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET @AsOfDate = ISNULL(@AsOfDate, CAST(SYSUTCDATETIME() AS DATE));

    DECLARE @FinePerDay DECIMAL(12,2) =
        (SELECT CAST(SettingValue AS DECIMAL(12,2))
         FROM dbo.SystemSettings WHERE SettingKey = 'FinePerDay');

    SELECT br.BorrowId, br.ReceiptCode,
           r.CardNumber, r.FullName AS ReaderName, r.Phone, r.Email,
           b.BookCode, b.Title,
           d.Quantity - d.ReturnedQty                                  AS NotReturnedQty,
           br.BorrowDate, br.DueDate,
           DATEDIFF(DAY, br.DueDate, @AsOfDate)                        AS DaysOverdue,
           DATEDIFF(DAY, br.DueDate, @AsOfDate)
              * @FinePerDay
              * (d.Quantity - d.ReturnedQty)                           AS EstimatedFine
    FROM dbo.BorrowReceipts br
    JOIN dbo.Readers r              ON r.ReaderId = br.ReaderId
    JOIN dbo.BorrowReceiptDetails d ON d.BorrowId = br.BorrowId
    JOIN dbo.Books b                ON b.BookId   = d.BookId
    WHERE br.IsDeleted = 0
      AND br.Status IN ('Borrowing','PartiallyReturned','Overdue')
      AND br.DueDate < @AsOfDate
      AND d.Quantity > d.ReturnedQty
    ORDER BY DaysOverdue DESC;
END
GO


/* =============================================================================
   KHỐI 9: STORED PROCEDURE AUTHENTICATION
   ============================================================================= */

-- 9.1 Xác thực đăng nhập (kèm lockout)
CREATE OR ALTER PROCEDURE dbo.sp_User_Login
    @Username        VARCHAR(50),
    @PasswordHash    VARCHAR(255),     -- ứng dụng đã hash + so sánh phía client; ở đây kiểm tra match
    @IpAddress       VARCHAR(45)  = NULL,
    @UserAgent       NVARCHAR(300) = NULL,
    @ResultCode      INT OUTPUT,       -- 0=OK, 1=NotFound, 2=Locked, 3=WrongPass, 4=Inactive
    @UserId          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @StoredHash VARCHAR(255), @IsActive BIT, @LockoutUntil DATETIME2(0),
                @FailedCount INT, @Threshold INT, @LockMinutes INT;

        SELECT @Threshold   = CAST(SettingValue AS INT) FROM dbo.SystemSettings WHERE SettingKey = 'LockoutThreshold';
        SELECT @LockMinutes = CAST(SettingValue AS INT) FROM dbo.SystemSettings WHERE SettingKey = 'LockoutMinutes';

        SELECT @UserId       = UserId,
               @StoredHash   = PasswordHash,
               @IsActive     = IsActive,
               @LockoutUntil = LockoutUntil,
               @FailedCount  = FailedLoginCount
        FROM dbo.Users WITH (UPDLOCK, ROWLOCK)
        WHERE Username = @Username AND IsDeleted = 0;

        IF @UserId IS NULL
        BEGIN
            SET @ResultCode = 1;        -- NotFound
            INSERT INTO dbo.AuditLogs (Action, EntityType, EntityId, IpAddress, UserAgent, Description)
            VALUES ('LOGIN_FAILED', 'User', @Username, @IpAddress, @UserAgent, N'Không tìm thấy tài khoản');
            COMMIT TRANSACTION;
            RETURN;
        END

        IF @IsActive = 0
        BEGIN
            SET @ResultCode = 4;        -- Inactive
            COMMIT TRANSACTION;
            RETURN;
        END

        IF @LockoutUntil IS NOT NULL AND @LockoutUntil > SYSUTCDATETIME()
        BEGIN
            SET @ResultCode = 2;        -- Locked
            COMMIT TRANSACTION;
            RETURN;
        END

        IF @StoredHash <> @PasswordHash
        BEGIN
            SET @FailedCount = @FailedCount + 1;

            UPDATE dbo.Users
               SET FailedLoginCount = @FailedCount,
                   LockoutUntil = CASE WHEN @FailedCount >= @Threshold
                                       THEN DATEADD(MINUTE, @LockMinutes, SYSUTCDATETIME())
                                       ELSE LockoutUntil END,
                   UpdatedAt = SYSUTCDATETIME()
             WHERE UserId = @UserId;

            INSERT INTO dbo.AuditLogs (UserId, Action, EntityType, EntityId, IpAddress, UserAgent, Description)
            VALUES (@UserId, 'LOGIN_FAILED', 'User', CAST(@UserId AS VARCHAR(50)),
                    @IpAddress, @UserAgent,
                    N'Sai mật khẩu, lần thứ ' + CAST(@FailedCount AS NVARCHAR(10)));

            SET @ResultCode = 3;        -- WrongPass
            COMMIT TRANSACTION;
            RETURN;
        END

        -- Đăng nhập thành công
        UPDATE dbo.Users
           SET FailedLoginCount = 0,
               LockoutUntil     = NULL,
               LastLoginAt      = SYSUTCDATETIME(),
               LastLoginIp      = @IpAddress,
               UpdatedAt        = SYSUTCDATETIME()
         WHERE UserId = @UserId;

        INSERT INTO dbo.AuditLogs (UserId, Action, EntityType, EntityId, IpAddress, UserAgent, Description)
        VALUES (@UserId, 'LOGIN', 'User', CAST(@UserId AS VARCHAR(50)),
                @IpAddress, @UserAgent, N'Đăng nhập thành công');

        SET @ResultCode = 0;            -- OK
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- 9.2 Lấy danh sách quyền của user (cho phân quyền giao diện)
CREATE OR ALTER PROCEDURE dbo.sp_User_GetPermissions
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.PermissionId, p.PermissionCode, p.Module, p.Description
    FROM dbo.Users u
    JOIN dbo.RolePermissions rp ON rp.RoleId       = u.RoleId
    JOIN dbo.Permissions     p  ON p.PermissionId  = rp.PermissionId
    WHERE u.UserId = @UserId AND u.IsDeleted = 0 AND u.IsActive = 1
    ORDER BY p.Module, p.PermissionCode;
END
GO


/* =============================================================================
   KHỐI 10: TRIGGER (có chống recursive update)
   ============================================================================= */

-- 10.1 Tự động cập nhật Status sách dựa trên tồn kho
CREATE OR ALTER TRIGGER dbo.trg_Books_UpdateStatus
ON dbo.Books
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chốt chống đệ quy: nếu trigger gọi UPDATE trên Books -> không bắn lại
    IF TRIGGER_NESTLEVEL(OBJECT_ID('dbo.trg_Books_UpdateStatus')) > 1
        RETURN;

    IF NOT UPDATE(Quantity) AND NOT UPDATE(AvailableQty)
        RETURN;

    -- Chỉ update các hàng thực sự cần đổi Status (tránh ghi không cần thiết)
    ;WITH ToUpdate AS (
        SELECT b.BookId,
               NewStatus = CASE
                   WHEN b.Status IN ('Retired','Lost','Damaged') THEN b.Status
                   WHEN b.Quantity = 0 OR b.AvailableQty = 0     THEN 'OutOfStock'
                   ELSE 'Available'
               END
        FROM dbo.Books b
        JOIN inserted i ON i.BookId = b.BookId
    )
    UPDATE b
       SET b.Status    = t.NewStatus,
           b.UpdatedAt = SYSUTCDATETIME()
    FROM dbo.Books b
    JOIN ToUpdate t ON t.BookId = b.BookId
    WHERE b.Status <> t.NewStatus;
END
GO

-- 10.2 Tự động đánh dấu phiếu Overdue khi có UPDATE
CREATE OR ALTER TRIGGER dbo.trg_BorrowReceipts_Overdue
ON dbo.BorrowReceipts
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF TRIGGER_NESTLEVEL(OBJECT_ID('dbo.trg_BorrowReceipts_Overdue')) > 1
        RETURN;

    IF NOT UPDATE(Status) AND NOT UPDATE(DueDate)
        RETURN;

    UPDATE br
       SET Status    = 'Overdue',
           UpdatedAt = SYSUTCDATETIME()
    FROM dbo.BorrowReceipts br
    JOIN inserted i ON i.BorrowId = br.BorrowId
    WHERE br.Status IN ('Borrowing','PartiallyReturned')
      AND br.DueDate < CAST(SYSUTCDATETIME() AS DATE)
      AND EXISTS (
            SELECT 1 FROM dbo.BorrowReceiptDetails d
            WHERE d.BorrowId = br.BorrowId
              AND d.Quantity > d.ReturnedQty
      );
END
GO


/* =============================================================================
   KHỐI 11: JOB SQL (gợi ý chạy hàng ngày)
   - Cần SQL Server Agent.
   - Đây là code mẫu để tạo job; chạy tay nếu chưa có Agent.
   ============================================================================= */

-- Procedure quét toàn bộ phiếu, đánh dấu quá hạn (gọi từ Agent hoặc app)
CREATE OR ALTER PROCEDURE dbo.sp_Job_MarkOverdueBorrows
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.BorrowReceipts
       SET Status    = 'Overdue',
           UpdatedAt = SYSUTCDATETIME()
     WHERE IsDeleted = 0
       AND Status IN ('Borrowing','PartiallyReturned')
       AND DueDate < CAST(SYSUTCDATETIME() AS DATE)
       AND EXISTS (
           SELECT 1 FROM dbo.BorrowReceiptDetails d
           WHERE d.BorrowId = dbo.BorrowReceipts.BorrowId
             AND d.Quantity > d.ReturnedQty
       );

    -- Gửi notification "DueSoon" cho phiếu sắp đến hạn
    DECLARE @DueSoonDays INT =
        (SELECT CAST(SettingValue AS INT) FROM dbo.SystemSettings WHERE SettingKey = 'DueSoonDays');

    INSERT INTO dbo.Notifications (RecipientType, RecipientId, NotifyType, Title, Body)
    SELECT 'Reader', br.ReaderId, 'DueSoon',
           N'Sách sắp đến hạn trả',
           N'Phiếu ' + br.ReceiptCode + N' sẽ đến hạn trả ngày ' + CONVERT(NVARCHAR(10), br.DueDate, 23)
    FROM dbo.BorrowReceipts br
    WHERE br.IsDeleted = 0
      AND br.Status IN ('Borrowing','PartiallyReturned')
      AND DATEDIFF(DAY, CAST(SYSUTCDATETIME() AS DATE), br.DueDate) BETWEEN 0 AND @DueSoonDays
      AND NOT EXISTS (
          SELECT 1 FROM dbo.Notifications n
          WHERE n.RecipientType = 'Reader' AND n.RecipientId = br.ReaderId
            AND n.NotifyType = 'DueSoon'
            AND CAST(n.CreatedAt AS DATE) = CAST(SYSUTCDATETIME() AS DATE)
      );

    -- Dọn Reservation hết hạn
    UPDATE dbo.Reservations
       SET Status = 'Expired'
     WHERE Status = 'Pending'
       AND ExpiresAt < SYSUTCDATETIME();
END
GO


/* =============================================================================
   KHỐI 12: KIỂM TRA NHANH
   ============================================================================= */
PRINT '=== Số lượng bản ghi từng bảng ===';
SELECT 'Roles'                AS TableName, COUNT(*) AS [RowCount] FROM dbo.Roles
UNION ALL SELECT 'Permissions',           COUNT(*) FROM dbo.Permissions
UNION ALL SELECT 'RolePermissions',       COUNT(*) FROM dbo.RolePermissions
UNION ALL SELECT 'Users',                 COUNT(*) FROM dbo.Users
UNION ALL SELECT 'Categories',            COUNT(*) FROM dbo.Categories
UNION ALL SELECT 'Books',                 COUNT(*) FROM dbo.Books
UNION ALL SELECT 'Readers',               COUNT(*) FROM dbo.Readers
UNION ALL SELECT 'BorrowReceipts',        COUNT(*) FROM dbo.BorrowReceipts
UNION ALL SELECT 'BorrowReceiptDetails',  COUNT(*) FROM dbo.BorrowReceiptDetails
UNION ALL SELECT 'ReturnReceipts',        COUNT(*) FROM dbo.ReturnReceipts
UNION ALL SELECT 'ReturnReceiptDetails',  COUNT(*) FROM dbo.ReturnReceiptDetails
UNION ALL SELECT 'Reservations',          COUNT(*) FROM dbo.Reservations
UNION ALL SELECT 'Notifications',         COUNT(*) FROM dbo.Notifications
UNION ALL SELECT 'PenaltyHistory',        COUNT(*) FROM dbo.PenaltyHistory
UNION ALL SELECT 'AuditLogs',             COUNT(*) FROM dbo.AuditLogs
UNION ALL SELECT 'SystemSettings',        COUNT(*) FROM dbo.SystemSettings;

PRINT '=== Dashboard KPI ===';
SELECT * FROM dbo.vw_Dashboard_KPI;

PRINT '=== Phiếu quá hạn ===';
SELECT * FROM dbo.vw_OverdueBorrows;

PRINT '=== Top 5 sách mượn nhiều ===';
EXEC dbo.sp_Stat_TopBorrowedBooks @TopN = 5;

PRINT '=== Top 5 độc giả hoạt động ===';
EXEC dbo.sp_Stat_TopActiveReaders @TopN = 5;

PRINT '';
PRINT '====================================================';
PRINT '  TLU_Library_DEV - PRODUCTION READY - SETUP DONE';
PRINT '====================================================';
GO