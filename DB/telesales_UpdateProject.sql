-- =============================================
--	Name				Date			Description
--	phuocph1			20/09/2016		Cập nhật Project
-- =============================================
CREATE PROCEDURE [dbo].[telesales_UpdateProject] 
	-- Add the parameters for the stored procedure here
	@UserId uniqueidentifier,
	@AppId  int,
	@ProjectID int,
	@CampaignID	int,	
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
	UPDATE telesales_Project
	SET CampaignID = @CampaignID, Code = @Code, Name = @Name, Greeting = @Greeting, Conclusion = @Conclusion, TotalPlan = @TotalPlan, TotalTarget = @TotalTarget, MonthlyPlan = @MonthlyPlan, MonthlyTarget = @MonthlyTarget,DailyPlan = @DailyPlan, DailyTarget = @DailyTarget,  UpdatedDate = GETDATE(), UpdatedBy = @UserId, Visiable = @Visiable, IsDeleted = @IsDeleted
	Where ProjectID = @ProjectID
END