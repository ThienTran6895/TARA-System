USE [MBNID]
GO
/****** Object:  StoredProcedure [dbo].[spa_AddNewUser]    Script Date: 8/11/2016 8:18:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
--	Name				Date			Description
--	hungnq1				10/08/2016		Tạo User
-- =============================================
CREATE PROCEDURE [dbo].[spa_AddNewUser] 
	-- Add the parameters for the stored procedure here
	@UserId varchar(50),
	@AppId  int,
	@Username	varchar(100),
	@Password	varchar(100),
	@FirstName	varchar(100),
	@LastName	varchar(100)
AS
BEGIN	
INSERT INTO Users(Username,[Password],FirstName,LastName,CreatedBy,CreatedDate,UpdatedDate)
VAlUES (@Username,@Password,@FirstName,@LastName,@UserId,GETDATE(),GETDATE())
END



GO
/****** Object:  StoredProcedure [dbo].[spa_GetAllUsers]    Script Date: 8/11/2016 8:18:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
--	Name				Date			Description
--	hungnq1				09/08/2016		Lay danh sach user phân trang
-- =============================================
CREATE PROCEDURE [dbo].[spa_GetAllUsers] 
	-- Add the parameters for the stored procedure here
	@UserId varchar(50),
	@AppId  int,
	@PageSize  int = null,
	@Page int   = null

AS
BEGIN	
DECLARE @sql NVARCHAR(4000)

SET @sql ='Declare @total int
	  SELECT @total = count(1) FROM Users'

  IF(@PageSize is null)
   SET @sql =@sql+'
   SELECT *,@total as Total FROM Users
					ORDER  BY UserId'
  ELSE
  SET @sql =@sql + '
		SELECT *,@total AS Total
		FROM Users
		ORDER BY UserId
		OFFSET @PageSize * (@Page - 1) ROWS FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE)'
		EXEC sp_executesql @sql, N'@PageSize INT, @Page INT',@PageSize, @Page
END



GO
/****** Object:  StoredProcedure [dbo].[spa_GetStaticData]    Script Date: 8/11/2016 8:18:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

--	Name				Date			Description

--	hungnq1				09/08/2016		Lay danh sach user phân trang

-- =============================================

CREATE PROCEDURE [dbo].[spa_GetStaticData] 

	-- Add the parameters for the stored procedure here

	@UserId varchar(50),

	@AppId  int,

	@DataSourceName varchar(100),

	@Language varchar(5)

AS

BEGIN	

SELECT DataKey,DataValue,DataDesc
FROM StaticData
WHERE  DataSource = @DataSourceName and [Language] = @Language

END





GO
/****** Object:  StoredProcedure [dbo].[spa_GetUserById]    Script Date: 8/11/2016 8:18:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
--	Name				Date			Description
--	hungnq1				09/08/2016		Lay user theo id
-- =============================================
CREATE PROCEDURE [dbo].[spa_GetUserById] 
	-- Add the parameters for the stored procedure here
	@UserId varchar(50),
	@AppId  int,
	@Id  int = null

AS
BEGIN	

		SELECT *
		FROM Users
		WHERE UserId = @Id

END



GO
/****** Object:  Table [dbo].[StaticData]    Script Date: 8/11/2016 8:18:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StaticData](
	[StaticDataId] [int] IDENTITY(1,1) NOT NULL,
	[DataKey] [int] NULL,
	[DataValue] [varchar](100) NULL,
	[DataSource] [varchar](50) NULL,
	[DataDesc] [varchar](100) NULL,
	[Language] [varchar](5) NULL,
 CONSTRAINT [PK_StaticData] PRIMARY KEY CLUSTERED 
(
	[StaticDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 8/11/2016 8:18:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](100) NULL,
	[Password] [varchar](100) NULL,
	[UserRole] [int] NULL,
	[FirstName] [varchar](100) NULL,
	[LastName] [varchar](100) NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[StaticData] ON 

INSERT [dbo].[StaticData] ([StaticDataId], [DataKey], [DataValue], [DataSource], [DataDesc], [Language]) VALUES (1, 1, N'Admin', N'UserRole', N'Role of user', N'VIE')
INSERT [dbo].[StaticData] ([StaticDataId], [DataKey], [DataValue], [DataSource], [DataDesc], [Language]) VALUES (2, 2, N'Accountant', N'UserRole', NULL, N'VIE')
SET IDENTITY_INSERT [dbo].[StaticData] OFF
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (1, N'hungnq', N'123456', 1, N'Hung', N'Nguyen', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (2, N'user1', N'123456', 2, N'User 1', N'Test', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (3, N'user2', N'123456', 1, N'User 2', N'Test', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (4, N'user3', N'123456', 1, N'User 3', N'Test', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (5, N'user4', N'123456', 2, N'User 4', N'Test', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (6, N'user5', N'123456', 2, N'User 5', N'Test', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (7, N'user6', N'123456', 2, N'User 6', N'Test', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (8, N'user7', N'123456', 2, N'User 7', N'Test', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (9, N'user8', N'123456', 2, N'User 8', N'Test', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (10, N'user9', N'123456', 2, N'User9', N'Test', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (11, N'user10', N'123456', 2, N'User 10', N'Test', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (12, N'user11', N'123456', 2, N'User 11', N'Test', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (13, N'user12', N'123456', 2, N'User 12', N'Test', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (14, N'user13', N'123456', 2, N'User 13', N'Test', N'1', NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (16, N'hungnq1', N'123456', 2, N'Hung', N'Nguyen', N'1', CAST(0x0000A65E00A22ECF AS DateTime), CAST(0x0000A65E00A22ECF AS DateTime))
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (17, N'hungnq2', N'123456', 2, N'Hùng', N'Nguen', N'2978C3E7-B198-47EF-BDF7-A6DF5FEEB7B0', CAST(0x0000A65E00AA6862 AS DateTime), CAST(0x0000A65E00AA6862 AS DateTime))
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (18, N'hungnq3', N'123456', 2, N'hhhh', N'nnn', N'2978C3E7-B198-47EF-BDF7-A6DF5FEEB7B0', CAST(0x0000A65E00AB8B35 AS DateTime), CAST(0x0000A65E00AB8B35 AS DateTime))
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (19, N'hungnq4', N'123456', 2, N'hhhh', N'nnn', N'2978C3E7-B198-47EF-BDF7-A6DF5FEEB7B0', CAST(0x0000A65E00AB975B AS DateTime), CAST(0x0000A65E00AB975B AS DateTime))
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (20, NULL, NULL, 2, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([UserId], [Username], [Password], [UserRole], [FirstName], [LastName], [CreatedBy], [CreatedDate], [UpdatedDate]) VALUES (23, N'hungnq12', N'123456', NULL, N'Hung', N'Nguyen', N'2978C3E7-B198-47EF-BDF7-A6DF5FEEB7B0', CAST(0x0000A65E010EEF6F AS DateTime), CAST(0x0000A65E010EEF6F AS DateTime))
SET IDENTITY_INSERT [dbo].[Users] OFF
