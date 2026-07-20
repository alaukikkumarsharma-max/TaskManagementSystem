-- ------------------------------------------------------------
-- 1. Database
-- ------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'TaskManagementDB')
BEGIN
    CREATE DATABASE TaskManagementDB;
END
GO

USE TaskManagementDB;
GO

-- ------------------------------------------------------------
-- 2. Drop existing tables (child -> parent order, FK-safe)
-- ------------------------------------------------------------
IF OBJECT_ID(N'dbo.Tasks', N'U') IS NOT NULL DROP TABLE dbo.Tasks;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID(N'dbo.Roles', N'U') IS NOT NULL DROP TABLE dbo.Roles;
IF OBJECT_ID(N'dbo.AuditLogs', N'U') IS NOT NULL DROP TABLE dbo.AuditLogs;
GO

-- ------------------------------------------------------------
-- 3. Roles
-- ------------------------------------------------------------
CREATE TABLE dbo.Roles
(
    RoleId          INT             IDENTITY(1,1)   NOT NULL,
    Name            NVARCHAR(50)                    NOT NULL,

    CONSTRAINT PK_Roles      PRIMARY KEY CLUSTERED (RoleId),
    CONSTRAINT UQ_Roles_Name UNIQUE (Name)
);
GO

-- ------------------------------------------------------------
-- 4. Users
-- ------------------------------------------------------------
CREATE TABLE dbo.Users
(
    UserId          INT             IDENTITY(1,1)   NOT NULL,
    FirstName       NVARCHAR(100)                   NOT NULL,
    LastName        NVARCHAR(100)                   NOT NULL,
    Email           NVARCHAR(256)                   NOT NULL,
    PasswordHash    NVARCHAR(256)                   NOT NULL,
    RoleId          INT                             NOT NULL,
    IsActive        BIT                             NOT NULL CONSTRAINT DF_Users_IsActive    DEFAULT (1),
    CreatedDate     DATETIME2                       NOT NULL CONSTRAINT DF_Users_CreatedDate DEFAULT (SYSUTCDATETIME()),
    UpdatedDate     DATETIME2                       NOT NULL CONSTRAINT DF_Users_UpdatedDate DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_Users       PRIMARY KEY CLUSTERED (UserId),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId)
        REFERENCES dbo.Roles (RoleId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

CREATE NONCLUSTERED INDEX IX_Users_RoleId ON dbo.Users (RoleId);
GO

-- ------------------------------------------------------------
-- 5. Tasks
-- ------------------------------------------------------------
CREATE TABLE dbo.Tasks
(
    TaskId              INT             IDENTITY(1,1)   NOT NULL,
    Title                NVARCHAR(200)                   NOT NULL,
    Description          NVARCHAR(MAX)                   NULL,
    Priority             NVARCHAR(20)                    NOT NULL,
    Status               NVARCHAR(20)                    NOT NULL CONSTRAINT DF_Tasks_Status      DEFAULT (N'ToDo'),
    DueDate              DATETIME2                       NOT NULL,
    AssignedToUserId     INT                             NULL,
    CreatedByUserId      INT                             NOT NULL,
    CreatedDate          DATETIME2                       NOT NULL CONSTRAINT DF_Tasks_CreatedDate DEFAULT (SYSUTCDATETIME()),
    UpdatedDate          DATETIME2                       NOT NULL CONSTRAINT DF_Tasks_UpdatedDate DEFAULT (SYSUTCDATETIME()),
    IsDeleted            BIT                             NOT NULL CONSTRAINT DF_Tasks_IsDeleted   DEFAULT (0),

    CONSTRAINT PK_Tasks PRIMARY KEY CLUSTERED (TaskId),
    CONSTRAINT CK_Tasks_Priority CHECK (Priority IN (N'Low', N'Medium', N'High')),
    CONSTRAINT CK_Tasks_Status   CHECK (Status IN (N'ToDo', N'InProgress', N'Completed', N'Cancelled')),
    CONSTRAINT FK_Tasks_Users_AssignedTo FOREIGN KEY (AssignedToUserId)
        REFERENCES dbo.Users (UserId)
        ON DELETE SET NULL
        ON UPDATE NO ACTION,
    CONSTRAINT FK_Tasks_Users_CreatedBy FOREIGN KEY (CreatedByUserId)
        REFERENCES dbo.Users (UserId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

CREATE NONCLUSTERED INDEX IX_Tasks_AssignedToUserId ON dbo.Tasks (AssignedToUserId);
CREATE NONCLUSTERED INDEX IX_Tasks_Status            ON dbo.Tasks (Status);
CREATE NONCLUSTERED INDEX IX_Tasks_Priority          ON dbo.Tasks (Priority);
CREATE NONCLUSTERED INDEX IX_Tasks_DueDate           ON dbo.Tasks (DueDate);
GO

-- ------------------------------------------------------------
-- 5. Audit Logs
-- ------------------------------------------------------------

CREATE TABLE AuditLogs (
    AuditLogId  INT IDENTITY(1,1) PRIMARY KEY,
    UserId      INT NULL,
    Action      NVARCHAR(30) NOT NULL,
    EntityName  NVARCHAR(100) NULL,
    EntityId    INT NULL,
    Details     NVARCHAR(500) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_AuditLogs_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL,
    CONSTRAINT CK_AuditLogs_Action CHECK (Action IN (N'Login', N'TaskCreated', N'TaskUpdated', N'TaskDeleted'))
);

CREATE INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
CREATE INDEX IX_AuditLogs_CreatedDate ON AuditLogs(CreatedDate);

-- ------------------------------------------------------------
-- 6. Seed data
-- ------------------------------------------------------------

INSERT INTO dbo.Roles (Name) VALUES
    (N'Admin'),
    (N'Manager'),
    (N'Employee');
GO