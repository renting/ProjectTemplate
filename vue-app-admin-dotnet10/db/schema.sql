-- vue-app-admin-dotnet8 範本資料庫 schema
-- 命名慣例：系統基礎表使用 Basic_ 前綴、參數表使用 Para_ 前綴
-- 稽核欄位：CreatedUserGuid / CreatedTime / ModifiedUserGuid / ModifiedTime
-- 資料表依相依順序建立（先建立會被其他表 FK 參照的表），可在空白資料庫上一次執行完成。

-- =============================================================
-- Basic_Users：使用者主檔
-- =============================================================
CREATE TABLE [dbo].[Basic_Users](
    [UserGuid] [uniqueidentifier] NOT NULL,
    [UserName] [nvarchar](20) NOT NULL,
    [Title] [nvarchar](20) NOT NULL,
    [NormalAccount] [varchar](30) NOT NULL,
    [PasswordHash] [varchar](72) NULL,
    [PasswordUpdTime] [datetime] NULL,
    [PasswordUpdErrorTimes] [int] NULL,
    [PasswordUpdLockDateTime] [datetime] NULL,
    [IdNumber] [varchar](20) NULL, -- TODO: 敏感個資欄位，通用範本是否保留待議；若專案不需身分驗證，建議直接移除本欄位
    [CanEditAfterApproval] [int] NULL,
    [CanRevokeEligibility] [int] NULL,
    [Status] [int] NOT NULL,
    [CreatedUserGuid] [uniqueidentifier] NOT NULL,
    [CreatedTime] [datetime] NOT NULL,
    [ModifiedUserGuid] [uniqueidentifier] NULL,
    [ModifiedTime] [datetime] NULL,
    [PasswordErrorTimes] [int] NULL,
    [LockDateTime] [datetime] NULL,
    CONSTRAINT [PK_Basic_Users] PRIMARY KEY CLUSTERED
(
    [UserGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Basic_Users] ADD CONSTRAINT [DF_Basic_Users_UserGuid] DEFAULT (newid()) FOR [UserGuid]
GO
ALTER TABLE [dbo].[Basic_Users] ADD CONSTRAINT [DF_Basic_Users_PasswordUpdTime] DEFAULT (getdate()) FOR [PasswordUpdTime]
GO
ALTER TABLE [dbo].[Basic_Users] ADD CONSTRAINT [DF_Basic_Users_PasswordUpdErrorTimes] DEFAULT ((0)) FOR [PasswordUpdErrorTimes]
GO
ALTER TABLE [dbo].[Basic_Users] ADD CONSTRAINT [DF_Basic_Users_CanEditAfterApproval] DEFAULT ((0)) FOR [CanEditAfterApproval]
GO
ALTER TABLE [dbo].[Basic_Users] ADD CONSTRAINT [DF_Basic_Users_CanRevokeEligibility] DEFAULT ((0)) FOR [CanRevokeEligibility]
GO
ALTER TABLE [dbo].[Basic_Users] ADD CONSTRAINT [DF_Basic_Users_CreatedTime] DEFAULT (getdate()) FOR [CreatedTime]
GO
ALTER TABLE [dbo].[Basic_Users] ADD CONSTRAINT [DF_Basic_Users_PasswordErrorTimes] DEFAULT ((0)) FOR [PasswordErrorTimes]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'使用者主檔', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'帳號編號(GuidKey)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'UserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'UserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'職稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'Title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'帳號，不能重複' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'NormalAccount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密碼雜湊值，儲存 BCrypt 雜湊結果' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'PasswordHash'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密碼最新異動時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'PasswordUpdTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密碼異動錯誤次數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'PasswordUpdErrorTimes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密碼異動鎖定時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'PasswordUpdLockDateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'身分證號（TODO：敏感個資，通用範本是否保留待議）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'IdNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否擁有核准後編輯權限，0：否、1：是' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'CanEditAfterApproval'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否擁有取消資格權限，0：否、1：是' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'CanRevokeEligibility'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'狀態，來源：參數設定：-10：待審核、0：停用、1：啟用、40：註銷' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立人員帳號編號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'CreatedUserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'CreatedTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後異動帳號編號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'ModifiedUserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後異動時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'ModifiedTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'帳號錯誤次數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'PasswordErrorTimes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'帳號鎖定時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users', @level2type=N'COLUMN',@level2name=N'LockDateTime'
GO

-- =============================================================
-- Basic_Groups：群組主檔
-- =============================================================
CREATE TABLE [dbo].[Basic_Groups](
    [GroupId] [int] IDENTITY(1,1) NOT NULL,
    [GroupName] [nvarchar](20) NOT NULL,
    [Remark] [nvarchar](400) NULL,
    [Status] [int] NULL,
    [SortOrder] [int] NULL,
    [CreatedUserGuid] [uniqueidentifier] NOT NULL,
    [CreatedTime] [datetime] NOT NULL,
    [ModifiedUserGuid] [uniqueidentifier] NULL,
    [ModifiedTime] [datetime] NULL,
    CONSTRAINT [PK_Basic_Groups] PRIMARY KEY CLUSTERED
(
    [GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Basic_Groups] ADD CONSTRAINT [DF_Basic_Groups_Remark] DEFAULT ('') FOR [Remark]
GO
ALTER TABLE [dbo].[Basic_Groups] ADD CONSTRAINT [DF_Basic_Groups_CreatedTime] DEFAULT (getdate()) FOR [CreatedTime]
GO
ALTER TABLE [dbo].[Basic_Groups] WITH CHECK ADD CONSTRAINT [FK_Basic_Groups_Basic_Users] FOREIGN KEY([CreatedUserGuid])
REFERENCES [dbo].[Basic_Users] ([UserGuid])
GO
ALTER TABLE [dbo].[Basic_Groups] CHECK CONSTRAINT [FK_Basic_Groups_Basic_Users]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'群組主檔', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Groups'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'群組編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Groups', @level2type=N'COLUMN',@level2name=N'GroupId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'群組名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Groups', @level2type=N'COLUMN',@level2name=N'GroupName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'備註' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Groups', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'狀態，來源：參數設定：0：停用、1：啟用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Groups', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Groups', @level2type=N'COLUMN',@level2name=N'SortOrder'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立人員帳號編號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Groups', @level2type=N'COLUMN',@level2name=N'CreatedUserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Groups', @level2type=N'COLUMN',@level2name=N'CreatedTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後異動帳號編號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Groups', @level2type=N'COLUMN',@level2name=N'ModifiedUserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後異動時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Groups', @level2type=N'COLUMN',@level2name=N'ModifiedTime'
GO

-- =============================================================
-- Basic_Modules：系統功能選單主檔
-- =============================================================
CREATE TABLE [dbo].[Basic_Modules](
    [ModuleId] [int] IDENTITY(1,1) NOT NULL,
    [FatherModuleId] [int] NULL,
    [ModuleName] [nvarchar](50) NOT NULL,
    [ModuleLink] [nvarchar](50) NULL,
    [ModuleIcon] [nvarchar](50) NULL,
    [SortOrder] [int] NULL,
    [Status] [int] NOT NULL,
    [CreatedUserGuid] [uniqueidentifier] NOT NULL,
    [CreatedTime] [datetime] NOT NULL,
    [ModifiedUserGuid] [uniqueidentifier] NULL,
    [ModifiedTime] [datetime] NULL,
    CONSTRAINT [PK_Basic_Modules] PRIMARY KEY CLUSTERED
(
    [ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Basic_Modules] ADD CONSTRAINT [DF_Basic_Modules_Status] DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Basic_Modules] ADD CONSTRAINT [DF_Basic_Modules_CreatedTime] DEFAULT (getdate()) FOR [CreatedTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系統功能選單主檔', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Modules'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'功能編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Modules', @level2type=N'COLUMN',@level2name=N'ModuleId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'父層Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Modules', @level2type=N'COLUMN',@level2name=N'FatherModuleId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'功能名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Modules', @level2type=N'COLUMN',@level2name=N'ModuleName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'功能頁面' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Modules', @level2type=N'COLUMN',@level2name=N'ModuleLink'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'功能圖示' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Modules', @level2type=N'COLUMN',@level2name=N'ModuleIcon'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Modules', @level2type=N'COLUMN',@level2name=N'SortOrder'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'啟用與否，0：停用、1：啟用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Modules', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立人員帳號編號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Modules', @level2type=N'COLUMN',@level2name=N'CreatedUserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Modules', @level2type=N'COLUMN',@level2name=N'CreatedTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後異動帳號編號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Modules', @level2type=N'COLUMN',@level2name=N'ModifiedUserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後異動時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Modules', @level2type=N'COLUMN',@level2name=N'ModifiedTime'
GO

-- =============================================================
-- Basic_Group_Modules：群組功能權限對照表
-- =============================================================
CREATE TABLE [dbo].[Basic_Group_Modules](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [GroupId] [int] NOT NULL,
    [ModuleId] [int] NOT NULL,
    [ViewStatus] [int] NOT NULL,
    [AddStatus] [int] NOT NULL,
    [EditStatus] [int] NOT NULL,
    [DelStatus] [int] NOT NULL,
    [DownloadStatus] [int] NOT NULL,
    [SortOrder] [int] NULL,
    [CreatedUserGuid] [uniqueidentifier] NOT NULL,
    [CreatedTime] [datetime] NOT NULL,
    [ModifiedUserGuid] [uniqueidentifier] NULL,
    [ModifiedTime] [datetime] NULL,
    CONSTRAINT [PK_Basic_Group_Modules] PRIMARY KEY CLUSTERED
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Basic_Group_Modules] ADD CONSTRAINT [DF_Basic_Group_Modules_ViewStatus] DEFAULT ((0)) FOR [ViewStatus]
GO
ALTER TABLE [dbo].[Basic_Group_Modules] ADD CONSTRAINT [DF_Basic_Group_Modules_AddStatus] DEFAULT ((0)) FOR [AddStatus]
GO
ALTER TABLE [dbo].[Basic_Group_Modules] ADD CONSTRAINT [DF_Basic_Group_Modules_EditStatus] DEFAULT ((0)) FOR [EditStatus]
GO
ALTER TABLE [dbo].[Basic_Group_Modules] ADD CONSTRAINT [DF_Basic_Group_Modules_DelStatus] DEFAULT ((0)) FOR [DelStatus]
GO
ALTER TABLE [dbo].[Basic_Group_Modules] ADD CONSTRAINT [DF_Basic_Group_Modules_DownloadStatus] DEFAULT ((0)) FOR [DownloadStatus]
GO
ALTER TABLE [dbo].[Basic_Group_Modules] ADD CONSTRAINT [DF_Basic_Group_Modules_CreatedTime] DEFAULT (getdate()) FOR [CreatedTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'群組功能權限對照表', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'權限編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'群組編號，來源：群組->群組編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules', @level2type=N'COLUMN',@level2name=N'GroupId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'功能編號，來源：功能->功能編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules', @level2type=N'COLUMN',@level2name=N'ModuleId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'查詢權限 (0:否 1:是)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules', @level2type=N'COLUMN',@level2name=N'ViewStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新增權限 (0:否 1:是)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules', @level2type=N'COLUMN',@level2name=N'AddStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改權限 (0:否 1:是)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules', @level2type=N'COLUMN',@level2name=N'EditStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'刪除權限 (0:否 1:是)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules', @level2type=N'COLUMN',@level2name=N'DelStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'下載權限 (0:否 1:是)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules', @level2type=N'COLUMN',@level2name=N'DownloadStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules', @level2type=N'COLUMN',@level2name=N'SortOrder'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立人員帳號編號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules', @level2type=N'COLUMN',@level2name=N'CreatedUserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules', @level2type=N'COLUMN',@level2name=N'CreatedTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後異動帳號編號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules', @level2type=N'COLUMN',@level2name=N'ModifiedUserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後異動時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Group_Modules', @level2type=N'COLUMN',@level2name=N'ModifiedTime'
GO

-- =============================================================
-- Basic_Users_Groups：使用者與群組對照表
-- =============================================================
CREATE TABLE [dbo].[Basic_Users_Groups](
    [GroupId] [int] NOT NULL,
    [UserGuid] [uniqueidentifier] NOT NULL,
    [CreatedUserGuid] [uniqueidentifier] NULL,
    [CreatedTime] [datetime] NOT NULL,
    CONSTRAINT [PK_Basic_Users_Groups] PRIMARY KEY CLUSTERED
(
    [GroupId] ASC, [UserGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Basic_Users_Groups] ADD CONSTRAINT [DF_Basic_Users_Groups_CreatedTime] DEFAULT (getdate()) FOR [CreatedTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'使用者與群組對照表', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users_Groups'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'群組編號，來源：群組->群組編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users_Groups', @level2type=N'COLUMN',@level2name=N'GroupId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'帳號編號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users_Groups', @level2type=N'COLUMN',@level2name=N'UserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立人員帳號編號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users_Groups', @level2type=N'COLUMN',@level2name=N'CreatedUserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Users_Groups', @level2type=N'COLUMN',@level2name=N'CreatedTime'
GO

-- =============================================================
-- Basic_LoginLog：登入紀錄
-- =============================================================
CREATE TABLE [dbo].[Basic_LoginLog](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [LoginTime] [datetime] NOT NULL,
    [SourceIP] [varchar](50) NOT NULL,
    [InputUserAccount] [varchar](40) NULL,
    [LoginUserGuid] [varchar](40) NOT NULL,
    [LoginMethod] [int] NULL,
    [Device] [int] NULL,
    [ResponseMessage] [int] NULL,
    CONSTRAINT [PK_Basic_LoginLog] PRIMARY KEY CLUSTERED
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Basic_LoginLog] ADD CONSTRAINT [DF_Basic_LoginLog_LoginTime] DEFAULT (getdate()) FOR [LoginTime]
GO
ALTER TABLE [dbo].[Basic_LoginLog] ADD CONSTRAINT [DF_Basic_LoginLog_Device] DEFAULT ((2)) FOR [Device]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'登入紀錄', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_LoginLog'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'序號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_LoginLog', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_LoginLog', @level2type=N'COLUMN',@level2name=N'LoginTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'來源IP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_LoginLog', @level2type=N'COLUMN',@level2name=N'SourceIP'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'輸入帳號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_LoginLog', @level2type=N'COLUMN',@level2name=N'InputUserAccount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'登入帳號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_LoginLog', @level2type=N'COLUMN',@level2name=N'LoginUserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'登入方式，依專案需求自行擴充，範例：1：SSO、2：一般帳號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_LoginLog', @level2type=N'COLUMN',@level2name=N'LoginMethod'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'裝置，來源：參數設定：1：APP、2：WEB' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_LoginLog', @level2type=N'COLUMN',@level2name=N'Device'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'回應訊息，來源：參數設定：0：失敗、1：成功' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_LoginLog', @level2type=N'COLUMN',@level2name=N'ResponseMessage'
GO

-- =============================================================
-- Basic_OperationLog：操作紀錄
-- =============================================================
CREATE TABLE [dbo].[Basic_OperationLog](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SourceIP] [varchar](50) NOT NULL,
    [module_id] [int] NOT NULL,
    [Action] [int] NOT NULL,
    [ActionName] [varchar](50) NOT NULL,
    [Result] [int] NOT NULL,
    [CreatedUserGuid] [uniqueidentifier] NOT NULL,
    [CreatedTime] [datetime] NOT NULL,
    CONSTRAINT [PK_Basic_OperationLog] PRIMARY KEY CLUSTERED
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Basic_OperationLog] ADD CONSTRAINT [DF_Basic_OperationLog_CreatedTime] DEFAULT (getdate()) FOR [CreatedTime]
GO
ALTER TABLE [dbo].[Basic_OperationLog] WITH CHECK ADD CONSTRAINT [FK_Basic_OperationLog_Basic_Users] FOREIGN KEY([CreatedUserGuid])
REFERENCES [dbo].[Basic_Users] ([UserGuid])
GO
ALTER TABLE [dbo].[Basic_OperationLog] CHECK CONSTRAINT [FK_Basic_OperationLog_Basic_Users]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作紀錄', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_OperationLog'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'序號，自動編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_OperationLog', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ip' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_OperationLog', @level2type=N'COLUMN',@level2name=N'SourceIP'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'功能id，來源：功能->功能ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_OperationLog', @level2type=N'COLUMN',@level2name=N'module_id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'動作，來源：參數設定（1: 新增、2: 修改、3: 刪除、4: 匯出、5: 查詢）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_OperationLog', @level2type=N'COLUMN',@level2name=N'Action'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新增、修改、刪除、匯出、查詢' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_OperationLog', @level2type=N'COLUMN',@level2name=N'ActionName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'結果，0：失敗、1：成功' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_OperationLog', @level2type=N'COLUMN',@level2name=N'Result'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立人員帳號編號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_OperationLog', @level2type=N'COLUMN',@level2name=N'CreatedUserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_OperationLog', @level2type=N'COLUMN',@level2name=N'CreatedTime'
GO

-- =============================================================
-- Basic_Api_Log：API 服務 LOG
-- =============================================================
CREATE TABLE [dbo].[Basic_Api_Log](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [Actions] [nvarchar](50) NOT NULL,
    [ModulesNames] [nvarchar](200) NOT NULL,
    [RequestId] [varchar](64) NULL,
    [Method] [varchar](10) NULL,
    [Path] [nvarchar](200) NULL,
    [StatusCode] [int] NULL,
    [ElapsedMs] [int] NULL,
    [Datas] [nvarchar](max) NULL,
    [CreateUserRowId] [uniqueidentifier] NULL,
    [CreateDate] [datetime] NULL,
    CONSTRAINT [PK_Basic_Api_Log] PRIMARY KEY CLUSTERED
(
    [id] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Basic_Api_Log] ADD CONSTRAINT [DF_Basic_Api_Log_CreateDate] DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[Basic_Api_Log] ADD CONSTRAINT [CK_Basic_Api_Log_Datas_IsJson] CHECK (Datas IS NULL OR ISJSON(Datas) = 1)
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'API服務LOG', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Log'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'動作(REQUEST,RESPONSE,EXCEPTION,SCHEDULE)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Log', @level2type=N'COLUMN',@level2name=N'Actions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'功能名稱（格式：Namespace.Class/Method）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Log', @level2type=N'COLUMN',@level2name=N'ModulesNames'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'請求關聯序號，對應 HttpContext.TraceIdentifier，用於串接同一次呼叫的 REQUEST/RESPONSE/EXCEPTION 紀錄' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Log', @level2type=N'COLUMN',@level2name=N'RequestId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'HTTP 方法（GET/POST/PUT/DELETE 等）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Log', @level2type=N'COLUMN',@level2name=N'Method'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'HTTP 請求路徑' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Log', @level2type=N'COLUMN',@level2name=N'Path'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'HTTP 回應狀態碼，僅 RESPONSE/EXCEPTION 列有值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Log', @level2type=N'COLUMN',@level2name=N'StatusCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'整支 API 呼叫耗時（毫秒），僅 RESPONSE/EXCEPTION 列有值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Log', @level2type=N'COLUMN',@level2name=N'ElapsedMs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'資料' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Log', @level2type=N'COLUMN',@level2name=N'Datas'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立者(關聯Basic_Users)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Log', @level2type=N'COLUMN',@level2name=N'CreateUserRowId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Log', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO

-- =============================================================
-- Basic_Api_Change_Log：系統紀錄表（資料異動前後對照）
-- =============================================================
CREATE TABLE [dbo].[Basic_Api_Change_Log](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [Actions] [nvarchar](50) NOT NULL,
    [ModulesNames] [nvarchar](200) NOT NULL,
    [RequestId] [varchar](64) NULL,
    [BeforeDatas] [nvarchar](max) NULL,
    [AfterDatas] [nvarchar](max) NULL,
    [SQLStr] [nvarchar](max) NULL,
    [CreateUserRowId] [uniqueidentifier] NULL,
    [CreateDate] [datetime] NULL,
    CONSTRAINT [PK_Basic_Api_Change_Log] PRIMARY KEY CLUSTERED
(
    [id] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Basic_Api_Change_Log] ADD CONSTRAINT [DF_Basic_Api_Change_Log_CreateDate] DEFAULT (getdate()) FOR [CreateDate]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系統紀錄表', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Change_Log'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'動作(ADD,MODIFY,DELETE)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Change_Log', @level2type=N'COLUMN',@level2name=N'Actions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'功能名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Change_Log', @level2type=N'COLUMN',@level2name=N'ModulesNames'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'請求關聯序號，對應造成此筆資料異動的 API 呼叫（Basic_Api_Log.RequestId）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Change_Log', @level2type=N'COLUMN',@level2name=N'RequestId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'異動前資料' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Change_Log', @level2type=N'COLUMN',@level2name=N'BeforeDatas'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'異動後資料' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Change_Log', @level2type=N'COLUMN',@level2name=N'AfterDatas'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SQL語法' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Change_Log', @level2type=N'COLUMN',@level2name=N'SQLStr'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立者(關聯Basic_Users)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Change_Log', @level2type=N'COLUMN',@level2name=N'CreateUserRowId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Basic_Api_Change_Log', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO

-- =============================================================
-- Para_Categories：參數類別主檔
-- =============================================================
CREATE TABLE [dbo].[Para_Categories](
    [Para_CategoryId] [int] IDENTITY(1,1) NOT NULL,
    [Para_CategoryKey] [nvarchar](100) NULL,
    [Para_CategoryName] [nvarchar](255) NULL,
    [IsHidden] [bit] NULL,
    [CreatedUserGuid] [uniqueidentifier] NULL,
    [CreatedTime] [datetime] NULL,
    CONSTRAINT [PK_Para_Categories] PRIMARY KEY CLUSTERED
(
    [Para_CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Para_Categories] ADD CONSTRAINT [DF_Para_Categories_IsHidden] DEFAULT ((0)) FOR [IsHidden]
GO
ALTER TABLE [dbo].[Para_Categories] ADD CONSTRAINT [DF_Para_Categories_CreatedTime] DEFAULT (getdate()) FOR [CreatedTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'參數類別主檔', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Categories'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'參數類別編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Categories', @level2type=N'COLUMN',@level2name=N'Para_CategoryId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'參數類別Key，查詢用關鍵字' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Categories', @level2type=N'COLUMN',@level2name=N'Para_CategoryKey'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'參數類別名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Categories', @level2type=N'COLUMN',@level2name=N'Para_CategoryName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'隱藏，參數設定功能隱藏 0：顯示、1：隱藏' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Categories', @level2type=N'COLUMN',@level2name=N'IsHidden'
GO

-- =============================================================
-- Para_Info：參數設定明細
-- =============================================================
CREATE TABLE [dbo].[Para_Info](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Para_CategoryId] [int] NOT NULL,
    [ParaName] [nvarchar](100) NOT NULL,
    [ParaValue] [nvarchar](100) NOT NULL,
    [SortOrder] [int] NULL,
    [Status] [int] NULL,
    [CreatedUserGuid] [uniqueidentifier] NOT NULL,
    [CreatedTime] [datetime] NOT NULL,
    [ModifiedUserGuid] [uniqueidentifier] NULL,
    [ModifiedTime] [datetime] NULL,
    CONSTRAINT [PK_Para_Info] PRIMARY KEY CLUSTERED
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Para_Info] ADD CONSTRAINT [DF_Para_Info_Status] DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Para_Info] ADD CONSTRAINT [DF_Para_Info_CreatedTime] DEFAULT (getdate()) FOR [CreatedTime]
GO
ALTER TABLE [dbo].[Para_Info] WITH CHECK ADD CONSTRAINT [FK_Para_Info_Basic_Users] FOREIGN KEY([CreatedUserGuid])
REFERENCES [dbo].[Basic_Users] ([UserGuid])
GO
ALTER TABLE [dbo].[Para_Info] CHECK CONSTRAINT [FK_Para_Info_Basic_Users]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'參數設定明細', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Info'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'參數編號，不可重複' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Info', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'參數類別代碼，來源：參數設定類別->類別編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Info', @level2type=N'COLUMN',@level2name=N'Para_CategoryId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'參數名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Info', @level2type=N'COLUMN',@level2name=N'ParaName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'參數值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Info', @level2type=N'COLUMN',@level2name=N'ParaValue'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Info', @level2type=N'COLUMN',@level2name=N'SortOrder'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'狀態，來源：參數設定：0：停用、1：啟用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Info', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立人員帳號編號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Info', @level2type=N'COLUMN',@level2name=N'CreatedUserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Info', @level2type=N'COLUMN',@level2name=N'CreatedTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'異動人員帳號編號，來源：管理人員->帳號編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Info', @level2type=N'COLUMN',@level2name=N'ModifiedUserGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'異動時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Para_Info', @level2type=N'COLUMN',@level2name=N'ModifiedTime'
GO
