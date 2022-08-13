-- =============================================
--	Name				Date			Description
--	vunn			08/11/2018		Lay ProjectUsers theo id
--	telesales_GetProjectByUser '395DE479-4E2F-44F2-BC2D-822B1D582EE5', 1,'8EC7006E-6679-45F4-AEFD-5B244722D4F8'
-- =============================================
CREATE PROCEDURE [dbo].[telesales_GetProjectByUser] 
	-- Add the parameters for the stored procedure here
	@UserId uniqueidentifier,
	@AppId  int,
	@UsersId uniqueidentifier = null
AS
DECLARE @SQL NVARCHAR(4000)
BEGIN	
	SELECT p.*, pu.UserId FROM telesales_Project p
	INNER JOIN telesales_ProjectUsers pu ON  p.ProjectID = pu.ProjectID AND pu.UserId = @UsersId
END