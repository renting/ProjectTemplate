-- vue-app-admin-dotnet8 範本初始資料
-- 執行順序：先執行 schema.sql 建好資料表，再執行本檔案。
--
-- 預設 admin 帳號：admin / Admin@123
-- ⚠ 僅供開發與展示使用，正式環境上線前務必更改此密碼（並重新產生 PasswordHash）。

-- =============================================================
-- Basic_Users：admin 帳號
-- =============================================================
INSERT INTO [dbo].[Basic_Users]
    ([UserGuid], [UserName], [Title], [NormalAccount], [PasswordHash], [Status], [CreatedUserGuid], [CreatedTime])
VALUES
    ('11111111-1111-1111-1111-111111111111', N'系統管理員', N'管理員', 'admin',
     '$2b$11$kzifdwdUO3rRCGzkyM6cFunz59lvlVpqJ8rlpAWsXXWy.MGFNoD7S', -- BCrypt(Admin@123, cost=11)
     1, '11111111-1111-1111-1111-111111111111', GETDATE());
GO

-- =============================================================
-- Basic_Modules：選單樹（對齊前端 vueappadmin.client/src/router/index.ts 現有路由）
-- =============================================================
SET IDENTITY_INSERT [dbo].[Basic_Modules] ON;

INSERT INTO [dbo].[Basic_Modules]
    ([ModuleId], [FatherModuleId], [ModuleName], [ModuleLink], [ModuleIcon], [SortOrder], [Status], [CreatedUserGuid], [CreatedTime])
VALUES
    (1, NULL, N'儀表板',   '/dashboard',           'bi-speedometer2', 1, 1, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (2, NULL, N'資料管理', NULL,                   'bi-folder2-open', 2, 1, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (3, 2,    N'範例清單', '/example-items',       'bi-list-ul',      1, 1, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (4, 2,    N'類別管理', '/example-categories',  'bi-tags',         2, 1, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (5, NULL, N'系統管理', NULL,                   'bi-gear',         3, 1, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (6, 5,    N'群組管理', '/groups',              'bi-people',       1, 1, '11111111-1111-1111-1111-111111111111', GETDATE());

SET IDENTITY_INSERT [dbo].[Basic_Modules] OFF;
GO

-- =============================================================
-- Basic_Groups：Administrators 群組
-- =============================================================
SET IDENTITY_INSERT [dbo].[Basic_Groups] ON;

INSERT INTO [dbo].[Basic_Groups]
    ([GroupId], [GroupName], [Remark], [Status], [SortOrder], [CreatedUserGuid], [CreatedTime])
VALUES
    (1, N'Administrators', N'系統管理員群組，擁有全部功能權限', 1, 1, '11111111-1111-1111-1111-111111111111', GETDATE());

SET IDENTITY_INSERT [dbo].[Basic_Groups] OFF;
GO

-- =============================================================
-- Basic_Group_Modules：Administrators 群組擁有全部模組的五種權限
-- =============================================================
INSERT INTO [dbo].[Basic_Group_Modules]
    ([GroupId], [ModuleId], [ViewStatus], [AddStatus], [EditStatus], [DelStatus], [DownloadStatus], [SortOrder], [CreatedUserGuid], [CreatedTime])
VALUES
    (1, 1, 1, 1, 1, 1, 1, 1, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (1, 2, 1, 1, 1, 1, 1, 2, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (1, 3, 1, 1, 1, 1, 1, 3, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (1, 4, 1, 1, 1, 1, 1, 4, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (1, 5, 1, 1, 1, 1, 1, 5, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (1, 6, 1, 1, 1, 1, 1, 6, '11111111-1111-1111-1111-111111111111', GETDATE());
GO

-- =============================================================
-- Basic_Users_Groups：admin 加入 Administrators 群組
-- =============================================================
INSERT INTO [dbo].[Basic_Users_Groups]
    ([GroupId], [UserGuid], [CreatedUserGuid], [CreatedTime])
VALUES
    (1, '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', GETDATE());
GO

-- =============================================================
-- Para_Categories / Para_Info：示範列舉資料
-- 對應 schema.sql 中 Basic_LoginLog.Device / ResponseMessage 欄位的 MS_Description 列舉值
-- =============================================================
SET IDENTITY_INSERT [dbo].[Para_Categories] ON;

INSERT INTO [dbo].[Para_Categories]
    ([Para_CategoryId], [Para_CategoryKey], [Para_CategoryName], [IsHidden], [CreatedUserGuid], [CreatedTime])
VALUES
    (1, 'LoginDevice',   N'登入裝置',     0, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (2, 'LoginResponse', N'登入回應訊息', 0, '11111111-1111-1111-1111-111111111111', GETDATE());

SET IDENTITY_INSERT [dbo].[Para_Categories] OFF;
GO

INSERT INTO [dbo].[Para_Info]
    ([Para_CategoryId], [ParaName], [ParaValue], [SortOrder], [Status], [CreatedUserGuid], [CreatedTime])
VALUES
    (1, N'APP', '1', 1, 1, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (1, N'WEB', '2', 2, 1, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (2, N'失敗', '0', 1, 1, '11111111-1111-1111-1111-111111111111', GETDATE()),
    (2, N'成功', '1', 2, 1, '11111111-1111-1111-1111-111111111111', GETDATE());
GO
