USE [master]
GO
/****** Object:  Database [DEMOOMS]    Script Date: 8/16/2016 3:32:26 PM ******/
CREATE DATABASE [DEMOOMS]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DEMOOMS', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\DEMOOMS.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'DEMOOMS_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\DEMOOMS_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [DEMOOMS] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DEMOOMS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DEMOOMS] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DEMOOMS] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DEMOOMS] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DEMOOMS] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DEMOOMS] SET ARITHABORT OFF 
GO
ALTER DATABASE [DEMOOMS] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DEMOOMS] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [DEMOOMS] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DEMOOMS] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DEMOOMS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DEMOOMS] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DEMOOMS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DEMOOMS] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DEMOOMS] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DEMOOMS] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DEMOOMS] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DEMOOMS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DEMOOMS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DEMOOMS] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DEMOOMS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DEMOOMS] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DEMOOMS] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DEMOOMS] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DEMOOMS] SET RECOVERY FULL 
GO
ALTER DATABASE [DEMOOMS] SET  MULTI_USER 
GO
ALTER DATABASE [DEMOOMS] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DEMOOMS] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DEMOOMS] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DEMOOMS] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'DEMOOMS', N'ON'
GO
USE [DEMOOMS]
GO
/****** Object:  StoredProcedure [dbo].[GetListCustomerForSource]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		phuocph1
-- Create date: 10/08/2016
-- Description:	Lấy danh sách hàng theo nguồn
-- =============================================
CREATE PROCEDURE [dbo].[GetListCustomerForSource]
	(
		@SourceId INT
	)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT c.*
	FROM oms_Sources s JOIN oms_Customer c ON s.SourceID = c.SourceID
	WHERE s.SourceID = @SourceId
END

GO
/****** Object:  StoredProcedure [dbo].[oms_AddNewQuestion]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
--	Name				Date			Description
--	phuocph1			10/08/2016		Tạo Question
-- oms_AddNewQuestion '395DE479-4E2F-44F2-BC2D-822B1D582EE5', 1, 3, sds, sdadasd, 1, 4
-- =============================================
CREATE PROCEDURE [dbo].[oms_AddNewQuestion] 
	-- Add the parameters for the stored procedure here
	@UserId varchar(50),
	@AppId  int,
	@CampaignID int,
	@Code	varchar(100),
	@Name	nvarchar(max),
	@IsActive	bit,
	@DisplayOrder	int
AS
BEGIN	
INSERT INTO oms_Questions(CampaignID,Code,Name,CreatedDate,CreatedBy,IsActive,DisplayOrder) Output Inserted.QuestionID
VAlUES (@CampaignID,@Code,@Name,GETDATE(),@UserId,@IsActive,@DisplayOrder)
END


GO
/****** Object:  StoredProcedure [dbo].[oms_DeleteQuestion]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
--	Name				Date			Description
--	phuocph1			15/08/2016		Delete Question
-- oms_DeleteQuestion 22
-- =============================================
CREATE PROCEDURE [dbo].[oms_DeleteQuestion] 
	-- Add the parameters for the stored procedure here
	@UserId varchar(50),
	@AppId  int,
	@QuestionID	int
AS
BEGIN	
	DELETE FROM oms_Questions
	Where QuestionID = @QuestionID
END

GO
/****** Object:  StoredProcedure [dbo].[oms_GetAllQuestions]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
--	Name				Date			Description
--	phuocph1			09/08/2016		Lay danh sach questions phân trang
--	oms_GetAllQuestions 'dieulq' ,1, 20, 1, N'<p>dgdg</p>', N'code'
-- =============================================
CREATE PROCEDURE [dbo].[oms_GetAllQuestions] 
	-- Add the parameters for the stored procedure here
	@UserId varchar(50),
	@AppId  int,
	@PageSize  int = null,
	@Page int   = null,
	@Name nvarchar(max) = null,
	@Code varchar(50) = null
AS
BEGIN	
DECLARE @sql NVARCHAR(4000)
DECLARE @condition NVARCHAR(4000)

SET @condition = ''

IF(@Name is not null)
BEGIN
	SET @condition = @condition + ' AND Name LIKE N''' + @Name + ''' '
END

IF(@Code is not null)
BEGIN
	SET @condition = @condition + ' AND Code LIKE N''' + @Code + ''' '
END

SET @sql ='Declare @total int
	  SELECT @total = count(1) FROM oms_Questions WHERE 1=1' + @condition

  IF(@PageSize is null)
   SET @sql =@sql+'
   SELECT *,@total as Total FROM oms_Questions WHERE 1=1' + @condition +'
					ORDER  BY QuestionID'
  ELSE
  SET @sql =@sql + '
		SELECT *,@total AS Total
		FROM oms_Questions WHERE 1=1' + @condition + '
		ORDER BY QuestionID
		OFFSET @PageSize * (@Page - 1) ROWS FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE)'
		EXEC sp_executesql @sql, N'@PageSize INT, @Page INT',@PageSize, @Page
END




GO
/****** Object:  StoredProcedure [dbo].[oms_GetQuestionById]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
--	Name				Date			Description
--	phuocph1			12/08/2016		Lay question theo id
-- =============================================
CREATE PROCEDURE [dbo].[oms_GetQuestionById] 
	-- Add the parameters for the stored procedure here
	@UserId varchar(50),
	@AppId  int,
	@Id  int = null

AS
BEGIN	
		SELECT *
		FROM oms_Questions
		WHERE QuestionID = @Id
END

GO
/****** Object:  StoredProcedure [dbo].[oms_GetStaticData]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

--	Name				Date			Description

--	hungnq1				09/08/2016		Lay danh sach user phân trang

-- =============================================

CREATE PROCEDURE [dbo].[oms_GetStaticData] 

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
/****** Object:  StoredProcedure [dbo].[oms_UpdateQuestion]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
--	Name				Date			Description
--	phuocph1			10/08/2016		Cập nhật Question
-- =============================================
CREATE PROCEDURE [dbo].[oms_UpdateQuestion] 
	-- Add the parameters for the stored procedure here
	@UserId varchar(50),
	@AppId  int,
	@QuestionID	int,
	@CampaignID	int,
	@Code	varchar(50),
	@Name	nvarchar(Max),
	@IsActive	bit,
	@DisplayOrder	int
AS
BEGIN	
	UPDATE oms_Questions
	SET CampaignID = @CampaignID, Code = @Code, Name = @Name, UpdatedDate = GETDATE(), UpdatedBy = @UserId, IsActive = @IsActive, DisplayOrder = @DisplayOrder
	Where QuestionID = @QuestionID
END



GO
/****** Object:  Table [dbo].[oms_Agents]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[oms_Agents](
	[AgentID] [int] IDENTITY(1,1) NOT NULL,
	[UserCallCenter] [varchar](100) NULL,
	[PassCallCenter] [varchar](100) NULL,
	[Extension] [int] NULL,
	[SIP] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_oms_Agents] PRIMARY KEY CLUSTERED 
(
	[AgentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[oms_Applications]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_Applications](
	[ApplicationId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationName] [nvarchar](256) NOT NULL,
	[LoweredApplicationName] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](256) NULL,
 CONSTRAINT [PK_oms_Applications] PRIMARY KEY CLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_Call]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_Call](
	[CallID] [int] IDENTITY(1,1) NOT NULL,
	[CallParentID] [int] NULL,
	[CustomerID] [int] NULL,
	[CampaignID] [int] NULL,
	[UserId] [uniqueidentifier] NULL,
	[StatusID] [int] NULL,
	[Behavior] [nvarchar](100) NULL,
	[Feedback] [nvarchar](max) NULL,
	[RecallDate] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
	[IsSuccess] [bit] NULL,
 CONSTRAINT [PK_oms_Call] PRIMARY KEY CLUSTERED 
(
	[CallID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_CallSurvey]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_CallSurvey](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CallID] [int] NOT NULL,
	[QuestionName] [nvarchar](max) NULL,
	[SurveyContent] [nvarchar](max) NULL,
 CONSTRAINT [PK_oms_CallSurvey] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_CampaignCustomer]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_CampaignCustomer](
	[CampaignID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[IsCall] [bit] NULL,
	[CallBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_oms_CampaignCustomer] PRIMARY KEY CLUSTERED 
(
	[CampaignID] ASC,
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_CampaignCustomerField]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_CampaignCustomerField](
	[CampaignID] [int] NOT NULL,
	[CustomerFieldID] [int] NOT NULL,
	[DisplayOrder] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreateBy] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
	[IsEdit] [bit] NULL,
 CONSTRAINT [PK_oms_CampaignCustomerField] PRIMARY KEY CLUSTERED 
(
	[CampaignID] ASC,
	[CustomerFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_Campaigns]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[oms_Campaigns](
	[CampaignID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NULL,
	[Code] [nvarchar](100) NULL,
	[Name] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
	[Greeting] [nvarchar](max) NULL,
	[Conclusion] [nvarchar](max) NULL,
	[HashCode] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_oms_Campaigns] PRIMARY KEY CLUSTERED 
(
	[CampaignID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[oms_CampaignStatus]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_CampaignStatus](
	[CampaignID] [int] NOT NULL,
	[StatusID] [int] NOT NULL,
 CONSTRAINT [PK_oms_CampaignStatus] PRIMARY KEY CLUSTERED 
(
	[CampaignID] ASC,
	[StatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_CampaignUser]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_CampaignUser](
	[CampaignID] [int] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_oms_CampaignUser] PRIMARY KEY CLUSTERED 
(
	[CampaignID] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_Customer]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[oms_Customer](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[SourceID] [int] NULL,
	[Phone] [varchar](20) NULL,
	[Behavior] [nvarchar](100) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_oms_Customer] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[oms_CustomerError]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[oms_CustomerError](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[SourceID] [int] NULL,
	[Phone] [varchar](20) NULL,
	[RowError] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_oms_CustomerError] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[oms_CustomerErrorFieldValue]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_CustomerErrorFieldValue](
	[CustomerFieldID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[FieldValue] [nvarchar](1000) NULL,
 CONSTRAINT [PK_oms_CustomerErrorFieldValue] PRIMARY KEY CLUSTERED 
(
	[CustomerFieldID] ASC,
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_CustomerExist]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[oms_CustomerExist](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[SourceID] [int] NULL,
	[Phone] [varchar](20) NULL,
	[RowExist] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_oms_CustomerExist] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[oms_CustomerExistFieldValue]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_CustomerExistFieldValue](
	[CustomerFieldID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[FieldValue] [nvarchar](1000) NULL,
 CONSTRAINT [PK_oms_CustomerExistFieldValue] PRIMARY KEY CLUSTERED 
(
	[CustomerFieldID] ASC,
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_CustomerField]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[oms_CustomerField](
	[CustomerFieldID] [int] IDENTITY(1,1) NOT NULL,
	[FieldCode] [varchar](100) NOT NULL,
	[FieldName] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NULL,
	[IsEdit] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_oms_CustomerField] PRIMARY KEY CLUSTERED 
(
	[CustomerFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[oms_CustomerFieldValue]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_CustomerFieldValue](
	[CustomerFieldID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[FieldValue] [nvarchar](1000) NULL,
 CONSTRAINT [PK_oms_CustomerFieldValue] PRIMARY KEY CLUSTERED 
(
	[CustomerFieldID] ASC,
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_Modules]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_Modules](
	[ModuleID] [int] IDENTITY(1,1) NOT NULL,
	[ParentID] [int] NULL,
	[Name] [nvarchar](100) NULL,
	[Link] [nvarchar](1000) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [int] NULL,
	[IsVisible] [bit] NULL,
	[IsDelete] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_oms_Modules] PRIMARY KEY CLUSTERED 
(
	[ModuleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_PermissionProject]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_PermissionProject](
	[UserId] [uniqueidentifier] NOT NULL,
	[ProjectID] [int] NOT NULL,
 CONSTRAINT [PK_oms_PermissionProject] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_Permissions]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_Permissions](
	[RoleId] [uniqueidentifier] NOT NULL,
	[ModuleID] [int] NOT NULL,
 CONSTRAINT [PK_oms_Permissions] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[ModuleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_Projects]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[oms_Projects](
	[ProjectID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
	[HashCode] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_oms_Projects] PRIMARY KEY CLUSTERED 
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[oms_Questions]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[oms_Questions](
	[QuestionID] [int] IDENTITY(1,1) NOT NULL,
	[CampaignID] [int] NULL,
	[Code] [varchar](50) NULL,
	[Name] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
	[DisplayOrder] [int] NULL,
 CONSTRAINT [PK_oms_Questions] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[oms_ReCall]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_ReCall](
	[ReCallID] [int] IDENTITY(1,1) NOT NULL,
	[CallID] [int] NULL,
	[IsCall] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[ReCallBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_oms_ReCall] PRIMARY KEY CLUSTERED 
(
	[ReCallID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_Roles]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_Roles](
	[RoleId] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
	[LoweredRoleName] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](256) NULL,
	[ApplicationId] [int] NOT NULL,
 CONSTRAINT [PK_oms_Roles] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_Sources]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[oms_Sources](
	[SourceID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Link] [varchar](500) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_oms_Sources] PRIMARY KEY CLUSTERED 
(
	[SourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[oms_StatusCall]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oms_StatusCall](
	[StatusID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[DisplayOrder] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
	[IsRecall] [bit] NULL,
	[IsFail] [bit] NULL,
	[IsWrong] [bit] NULL,
	[IsConnect] [bit] NULL,
 CONSTRAINT [PK_oms_StatusCall] PRIMARY KEY CLUSTERED 
(
	[StatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oms_Survey]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[oms_Survey](
	[SurveyID] [int] IDENTITY(1,1) NOT NULL,
	[QuestionID] [int] NULL,
	[SurveyContent] [nvarchar](300) NULL,
	[Code] [varchar](100) NULL,
	[DisplayOrder] [int] NULL,
	[NextQuestionID] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[SurveyType] [int] NULL,
 CONSTRAINT [PK_oms_Survey] PRIMARY KEY CLUSTERED 
(
	[SurveyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[oms_Users]    Script Date: 8/16/2016 3:32:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[oms_Users](
	[UserId] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[Password] [varchar](100) NULL,
	[AgentID] [int] NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[Gender] [nvarchar](10) NULL,
	[BirthDay] [datetime] NULL,
	[Address] [nvarchar](200) NULL,
	[Email] [varchar](100) NULL,
	[Phone] [varchar](20) NULL,
	[Mobile] [varchar](20) NULL,
	[Company] [nvarchar](200) NULL,
	[ImageUrl] [varchar](200) NULL,
	[Website] [varchar](100) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
	[Visible] [bit] NULL,
 CONSTRAINT [PK_oms_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StaticData]    Script Date: 8/16/2016 3:32:26 PM ******/
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
SET IDENTITY_INSERT [dbo].[oms_Agents] ON 

INSERT [dbo].[oms_Agents] ([AgentID], [UserCallCenter], [PassCallCenter], [Extension], [SIP], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsActive]) VALUES (1, N'3978', N'TUJCUE8xMjM0NTY=', 3978, N'192.169.1.1', CAST(0x0000A57B016B070E AS DateTime), N'admin', CAST(0x0000A57B016B070E AS DateTime), N'admin', 1)
SET IDENTITY_INSERT [dbo].[oms_Agents] OFF
SET IDENTITY_INSERT [dbo].[oms_Campaigns] ON 

INSERT [dbo].[oms_Campaigns] ([CampaignID], [ProjectID], [Code], [Name], [IsActive], [Greeting], [Conclusion], [HashCode], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, 1, N'MBB_VN', N'Việt Nhật', 1, N'<div>
	Em ch&agrave;o Anh/chị, c&oacute; phải l&agrave; số điện thoại của Anh/chị ... kh&ocirc;ng ạ?</div>
<div>
	Dạ em l&agrave;&hellip;.gọi đến từ Japan Circle (Xơ Cồ) - Dự &aacute;n kết nối giao thương giữa Việt Nam-Nhật Bản v&agrave; c&aacute;c nước Asean . C&aacute;m ơn Anh/chị đ&atilde; đăng k&yacute; l&agrave; th&agrave;nh vi&ecirc;n của JPC trong thời gian qua.</div>
<div>
	Nhằm n&acirc;ng cao chất lượng phục vụ cho c&aacute;c doanh nghiệp được tốt hơn trong thời gian sắp tới , em xin ph&eacute;p hỏi thăm Anh/chị một số &yacute; kiến. Anh/chị c&oacute; thể d&agrave;nh ch&uacute;t thời gian trao đổi v&agrave; chia sẻ với em qua 3 c&acirc;u hỏi th&ocirc;i được kh&ocirc;ng ạ?&nbsp;</div>
', N'C&aacute;m ơn Anh/chị đ&atilde; d&agrave;nh thời gian trao đổi với em . Ch&uacute;c Anh/chị nhiều sức khỏe v&agrave; th&agrave;nh c&ocirc;ng trong c&ocirc;ng việc v&agrave; cuộc sống ạ . Em xin ch&agrave;o Anh/chị.', N'616A27A5C8C1ED8E12254315E0DCA3CA', CAST(0x0000A601009DAF80 AS DateTime), N'admin', CAST(0x0000A601009DAF80 AS DateTime), N'admin')
SET IDENTITY_INSERT [dbo].[oms_Campaigns] OFF
SET IDENTITY_INSERT [dbo].[oms_Projects] ON 

INSERT [dbo].[oms_Projects] ([ProjectID], [Name], [IsActive], [HashCode], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Telesale', 1, N'1F96D8DCA68ADFB04CC35FE11DBDA137', CAST(0x0000A5BA01339CB2 AS DateTime), N'admin', CAST(0x0000A5BA01339CB2 AS DateTime), N'admin')
SET IDENTITY_INSERT [dbo].[oms_Projects] OFF
SET IDENTITY_INSERT [dbo].[oms_Questions] ON 

INSERT [dbo].[oms_Questions] ([QuestionID], [CampaignID], [Code], [Name], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsActive], [DisplayOrder]) VALUES (70, 3, N'yhyh', N'<ol><li><b>hyhhfgh</b><br></li><li>t<u>hfhd</u><i><u></u></i><u>fh<small></small></u></li></ol>', CAST(0x0000A66400B17FF2 AS DateTime), N'395DE479-4E2F-44F2-BC2D-822B1D582EE5', CAST(0x0000A66400ECCA69 AS DateTime), N'395DE479-4E2F-44F2-BC2D-822B1D582EE5', 1, 3)
INSERT [dbo].[oms_Questions] ([QuestionID], [CampaignID], [Code], [Name], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsActive], [DisplayOrder]) VALUES (71, 3, N'?gegee', N'<p>grgegeg</p>', CAST(0x0000A66400DBDF77 AS DateTime), N'395DE479-4E2F-44F2-BC2D-822B1D582EE5', NULL, NULL, 0, 0)
INSERT [dbo].[oms_Questions] ([QuestionID], [CampaignID], [Code], [Name], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsActive], [DisplayOrder]) VALUES (72, 3, N'dgdgdgg', N'<p>gdgdgdg</p>', CAST(0x0000A66400F208DF AS DateTime), N'395DE479-4E2F-44F2-BC2D-822B1D582EE5', NULL, NULL, 0, 0)
INSERT [dbo].[oms_Questions] ([QuestionID], [CampaignID], [Code], [Name], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsActive], [DisplayOrder]) VALUES (73, 3, N'dgdg', N'<p>dgdg</p>', CAST(0x0000A66400F218FA AS DateTime), N'395DE479-4E2F-44F2-BC2D-822B1D582EE5', NULL, NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[oms_Questions] OFF
INSERT [dbo].[oms_Users] ([UserId], [UserName], [Password], [AgentID], [FirstName], [LastName], [Gender], [BirthDay], [Address], [Email], [Phone], [Mobile], [Company], [ImageUrl], [Website], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsActive], [Visible]) VALUES (N'395de479-4e2f-44f2-bc2d-822b1d582ee5', N'admin', N'QnBvQG9tcyEj', 1, N'Phạm Hữu', N'Phước', N'Nam', CAST(0x00007E8800000000 AS DateTime), N'Lý Chính Thắng', N'tienntm@matbao.com', N'0938492396', N'0938492396', N'Công ty cổ phần Mắt Bão', N'', N'', CAST(0x0000A53300000000 AS DateTime), N'admin', CAST(0x0000A5BA0133E707 AS DateTime), N'admin', 1, 1)
ALTER TABLE [dbo].[oms_Call] ADD  CONSTRAINT [DF_oms_Call_IsSuccess]  DEFAULT ((0)) FOR [IsSuccess]
GO
ALTER TABLE [dbo].[oms_CampaignCustomer] ADD  CONSTRAINT [DF_oms_CampaignCustomer_IsCall]  DEFAULT ((0)) FOR [IsCall]
GO
ALTER TABLE [dbo].[oms_CampaignCustomerField] ADD  CONSTRAINT [DF_oms_CampaignCustomerField_IsEdit]  DEFAULT ((0)) FOR [IsEdit]
GO
ALTER TABLE [dbo].[oms_CustomerField] ADD  CONSTRAINT [DF_oms_CustomerField_IsEdit]  DEFAULT ((0)) FOR [IsEdit]
GO
ALTER TABLE [dbo].[oms_Modules] ADD  CONSTRAINT [DF_oms_Modules_DisplayOrder]  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[oms_Modules] ADD  CONSTRAINT [DF_oms_Modules_IsDelete]  DEFAULT ((1)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[oms_Roles] ADD  CONSTRAINT [DF_oms_Roles_RoleId]  DEFAULT (newid()) FOR [RoleId]
GO
ALTER TABLE [dbo].[oms_StatusCall] ADD  CONSTRAINT [DF_oms_StatusCall_IsWrong]  DEFAULT ((0)) FOR [IsWrong]
GO
ALTER TABLE [dbo].[oms_StatusCall] ADD  CONSTRAINT [DF_oms_StatusCall_IsConnect]  DEFAULT ((0)) FOR [IsConnect]
GO
ALTER TABLE [dbo].[oms_Call]  WITH CHECK ADD  CONSTRAINT [FK_oms_Call_oms_Call] FOREIGN KEY([CallParentID])
REFERENCES [dbo].[oms_Call] ([CallID])
GO
ALTER TABLE [dbo].[oms_Call] CHECK CONSTRAINT [FK_oms_Call_oms_Call]
GO
ALTER TABLE [dbo].[oms_Call]  WITH CHECK ADD  CONSTRAINT [FK_oms_Call_oms_Campaigns] FOREIGN KEY([CampaignID])
REFERENCES [dbo].[oms_Campaigns] ([CampaignID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_Call] CHECK CONSTRAINT [FK_oms_Call_oms_Campaigns]
GO
ALTER TABLE [dbo].[oms_Call]  WITH CHECK ADD  CONSTRAINT [FK_oms_Call_oms_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[oms_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_Call] CHECK CONSTRAINT [FK_oms_Call_oms_Customer]
GO
ALTER TABLE [dbo].[oms_Call]  WITH CHECK ADD  CONSTRAINT [FK_oms_Call_oms_StatusCall] FOREIGN KEY([StatusID])
REFERENCES [dbo].[oms_StatusCall] ([StatusID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_Call] CHECK CONSTRAINT [FK_oms_Call_oms_StatusCall]
GO
ALTER TABLE [dbo].[oms_CallSurvey]  WITH CHECK ADD  CONSTRAINT [FK_oms_CallSurvey_oms_Call] FOREIGN KEY([CallID])
REFERENCES [dbo].[oms_Call] ([CallID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CallSurvey] CHECK CONSTRAINT [FK_oms_CallSurvey_oms_Call]
GO
ALTER TABLE [dbo].[oms_CampaignCustomer]  WITH CHECK ADD  CONSTRAINT [FK_oms_CampaignCustomer_oms_Campaigns] FOREIGN KEY([CampaignID])
REFERENCES [dbo].[oms_Campaigns] ([CampaignID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CampaignCustomer] CHECK CONSTRAINT [FK_oms_CampaignCustomer_oms_Campaigns]
GO
ALTER TABLE [dbo].[oms_CampaignCustomer]  WITH CHECK ADD  CONSTRAINT [FK_oms_CampaignCustomer_oms_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[oms_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CampaignCustomer] CHECK CONSTRAINT [FK_oms_CampaignCustomer_oms_Customer]
GO
ALTER TABLE [dbo].[oms_CampaignCustomerField]  WITH CHECK ADD  CONSTRAINT [FK_oms_CampaignCustomerField_oms_Campaigns] FOREIGN KEY([CampaignID])
REFERENCES [dbo].[oms_Campaigns] ([CampaignID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CampaignCustomerField] CHECK CONSTRAINT [FK_oms_CampaignCustomerField_oms_Campaigns]
GO
ALTER TABLE [dbo].[oms_CampaignCustomerField]  WITH CHECK ADD  CONSTRAINT [FK_oms_CampaignCustomerField_oms_CustomerField] FOREIGN KEY([CustomerFieldID])
REFERENCES [dbo].[oms_CustomerField] ([CustomerFieldID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CampaignCustomerField] CHECK CONSTRAINT [FK_oms_CampaignCustomerField_oms_CustomerField]
GO
ALTER TABLE [dbo].[oms_Campaigns]  WITH CHECK ADD  CONSTRAINT [FK_oms_Campaigns_oms_Projects] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[oms_Projects] ([ProjectID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_Campaigns] CHECK CONSTRAINT [FK_oms_Campaigns_oms_Projects]
GO
ALTER TABLE [dbo].[oms_CampaignStatus]  WITH CHECK ADD  CONSTRAINT [FK_oms_CampaignStatus_oms_Campaigns] FOREIGN KEY([CampaignID])
REFERENCES [dbo].[oms_Campaigns] ([CampaignID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CampaignStatus] CHECK CONSTRAINT [FK_oms_CampaignStatus_oms_Campaigns]
GO
ALTER TABLE [dbo].[oms_CampaignStatus]  WITH CHECK ADD  CONSTRAINT [FK_oms_CampaignStatus_oms_StatusCall] FOREIGN KEY([StatusID])
REFERENCES [dbo].[oms_StatusCall] ([StatusID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CampaignStatus] CHECK CONSTRAINT [FK_oms_CampaignStatus_oms_StatusCall]
GO
ALTER TABLE [dbo].[oms_CampaignUser]  WITH CHECK ADD  CONSTRAINT [FK_oms_CampaignUser_oms_Campaigns] FOREIGN KEY([CampaignID])
REFERENCES [dbo].[oms_Campaigns] ([CampaignID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CampaignUser] CHECK CONSTRAINT [FK_oms_CampaignUser_oms_Campaigns]
GO
ALTER TABLE [dbo].[oms_CampaignUser]  WITH CHECK ADD  CONSTRAINT [FK_oms_CampaignUser_oms_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[oms_Users] ([UserId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CampaignUser] CHECK CONSTRAINT [FK_oms_CampaignUser_oms_Users]
GO
ALTER TABLE [dbo].[oms_Customer]  WITH CHECK ADD  CONSTRAINT [FK_oms_Customer_oms_Sources] FOREIGN KEY([SourceID])
REFERENCES [dbo].[oms_Sources] ([SourceID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_Customer] CHECK CONSTRAINT [FK_oms_Customer_oms_Sources]
GO
ALTER TABLE [dbo].[oms_CustomerError]  WITH CHECK ADD  CONSTRAINT [FK_oms_CustomerError_oms_Sources] FOREIGN KEY([SourceID])
REFERENCES [dbo].[oms_Sources] ([SourceID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CustomerError] CHECK CONSTRAINT [FK_oms_CustomerError_oms_Sources]
GO
ALTER TABLE [dbo].[oms_CustomerErrorFieldValue]  WITH CHECK ADD  CONSTRAINT [FK_oms_CustomerErrorFieldValue_oms_CustomerError] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[oms_CustomerError] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CustomerErrorFieldValue] CHECK CONSTRAINT [FK_oms_CustomerErrorFieldValue_oms_CustomerError]
GO
ALTER TABLE [dbo].[oms_CustomerErrorFieldValue]  WITH CHECK ADD  CONSTRAINT [FK_oms_CustomerErrorFieldValue_oms_CustomerField] FOREIGN KEY([CustomerFieldID])
REFERENCES [dbo].[oms_CustomerField] ([CustomerFieldID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CustomerErrorFieldValue] CHECK CONSTRAINT [FK_oms_CustomerErrorFieldValue_oms_CustomerField]
GO
ALTER TABLE [dbo].[oms_CustomerExist]  WITH CHECK ADD  CONSTRAINT [FK_oms_CustomerExist_oms_Sources] FOREIGN KEY([SourceID])
REFERENCES [dbo].[oms_Sources] ([SourceID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CustomerExist] CHECK CONSTRAINT [FK_oms_CustomerExist_oms_Sources]
GO
ALTER TABLE [dbo].[oms_CustomerExistFieldValue]  WITH CHECK ADD  CONSTRAINT [FK_oms_CustomerExistFieldValue_oms_CustomerExist] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[oms_CustomerExist] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CustomerExistFieldValue] CHECK CONSTRAINT [FK_oms_CustomerExistFieldValue_oms_CustomerExist]
GO
ALTER TABLE [dbo].[oms_CustomerExistFieldValue]  WITH CHECK ADD  CONSTRAINT [FK_oms_CustomerExistFieldValue_oms_CustomerField] FOREIGN KEY([CustomerFieldID])
REFERENCES [dbo].[oms_CustomerField] ([CustomerFieldID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CustomerExistFieldValue] CHECK CONSTRAINT [FK_oms_CustomerExistFieldValue_oms_CustomerField]
GO
ALTER TABLE [dbo].[oms_CustomerFieldValue]  WITH CHECK ADD  CONSTRAINT [FK_oms_CustomerFieldValue_oms_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[oms_Customer] ([CustomerID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CustomerFieldValue] CHECK CONSTRAINT [FK_oms_CustomerFieldValue_oms_Customer]
GO
ALTER TABLE [dbo].[oms_CustomerFieldValue]  WITH CHECK ADD  CONSTRAINT [FK_oms_CustomerFieldValue_oms_CustomerField] FOREIGN KEY([CustomerFieldID])
REFERENCES [dbo].[oms_CustomerField] ([CustomerFieldID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_CustomerFieldValue] CHECK CONSTRAINT [FK_oms_CustomerFieldValue_oms_CustomerField]
GO
ALTER TABLE [dbo].[oms_PermissionProject]  WITH CHECK ADD  CONSTRAINT [FK_oms_PermissionProject_oms_Projects] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[oms_Projects] ([ProjectID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_PermissionProject] CHECK CONSTRAINT [FK_oms_PermissionProject_oms_Projects]
GO
ALTER TABLE [dbo].[oms_PermissionProject]  WITH CHECK ADD  CONSTRAINT [FK_oms_PermissionProject_oms_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[oms_Users] ([UserId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_PermissionProject] CHECK CONSTRAINT [FK_oms_PermissionProject_oms_Users]
GO
ALTER TABLE [dbo].[oms_Permissions]  WITH CHECK ADD  CONSTRAINT [FK_oms_Permissions_oms_Modules] FOREIGN KEY([ModuleID])
REFERENCES [dbo].[oms_Modules] ([ModuleID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_Permissions] CHECK CONSTRAINT [FK_oms_Permissions_oms_Modules]
GO
ALTER TABLE [dbo].[oms_Permissions]  WITH CHECK ADD  CONSTRAINT [FK_oms_Permissions_oms_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[oms_Roles] ([RoleId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_Permissions] CHECK CONSTRAINT [FK_oms_Permissions_oms_Roles]
GO
ALTER TABLE [dbo].[oms_Questions]  WITH CHECK ADD  CONSTRAINT [FK_oms_Questions_oms_Campaigns] FOREIGN KEY([CampaignID])
REFERENCES [dbo].[oms_Campaigns] ([CampaignID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_Questions] CHECK CONSTRAINT [FK_oms_Questions_oms_Campaigns]
GO
ALTER TABLE [dbo].[oms_ReCall]  WITH CHECK ADD  CONSTRAINT [FK_oms_ReCall_oms_Call] FOREIGN KEY([CallID])
REFERENCES [dbo].[oms_Call] ([CallID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_ReCall] CHECK CONSTRAINT [FK_oms_ReCall_oms_Call]
GO
ALTER TABLE [dbo].[oms_Roles]  WITH CHECK ADD  CONSTRAINT [FK_oms_Roles_oms_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[oms_Applications] ([ApplicationId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_Roles] CHECK CONSTRAINT [FK_oms_Roles_oms_Applications]
GO
ALTER TABLE [dbo].[oms_Survey]  WITH CHECK ADD  CONSTRAINT [FK_oms_Survey_oms_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[oms_Questions] ([QuestionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oms_Survey] CHECK CONSTRAINT [FK_oms_Survey_oms_Questions]
GO
ALTER TABLE [dbo].[oms_Users]  WITH CHECK ADD  CONSTRAINT [FK_oms_Users_oms_Agents] FOREIGN KEY([AgentID])
REFERENCES [dbo].[oms_Agents] ([AgentID])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[oms_Users] CHECK CONSTRAINT [FK_oms_Users_oms_Agents]
GO
USE [master]
GO
ALTER DATABASE [DEMOOMS] SET  READ_WRITE 
GO
