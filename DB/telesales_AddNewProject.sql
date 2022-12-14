-- =============================================
--	Name				Date			Description
--	phuocph1			20/09/2016		Tạo Project
-- =============================================
ALTER PROCEDURE [dbo].[telesales_AddNewProject] 
	-- Add the parameters for the stored procedure here
	@UserId uniqueidentifier,
	@AppId  int,
	@CampaignID int,
	@Code	nvarchar(100),
	@Name	nvarchar(100),	
	@Greeting	nvarchar(MAX),
	@Conclusion nvarchar(Max),
	@TotalPlan	int,	
	@TotalTarget	int,	
	@MonthlyPlan	int,	
	@MonthlyTarget	int,
	@DailyPlan	int,	
	@DailyTarget	int,
	@Visiable	bit,
	@IsDeleted bit
AS
BEGIN	
INSERT INTO telesales_Project(CampaignID,Code,Name,Greeting,Conclusion,TotalPlan,TotalTarget,MonthlyPlan,MonthlyTarget,DailyPlan,DailyTarget,CreatedDate,CreatedBy,Visiable,IsDeleted) Output Inserted.ProjectID
VAlUES (@CampaignID,@Code,@Name,@Greeting,@Conclusion,@TotalPlan,@TotalTarget,@MonthlyPlan,@MonthlyTarget,@DailyPlan,@DailyTarget,GETDATE(),@UserId,@Visiable,@IsDeleted)

-- vunn 01/16/2018 Tự động gán dự án cho người tạo
DECLARE @ProjectID INT
SELECT @ProjectID = SCOPE_IDENTITY();
EXEC telesales_AddNewProjectUsers @UserId,1, @ProjectID , @UserId;
END
