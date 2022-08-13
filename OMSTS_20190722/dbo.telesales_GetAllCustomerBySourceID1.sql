-- =============================================
--	Name				Date			Description
--	sonldt1			    17/11/2016	    L?y Danh sách customer theo ngu?n.
--  spa_telesales_CustomerByStatus '395DE479-4E2F-44F2-BC2D-822B1D582EE5' ,1,@ProjectID=3031,@SourceID=0,@statusId=0,@statusCallId=0,@DateStart='',@DateEnd='',@PageSize=40 , @Page=1
-- =============================================
ALTER PROCEDURE [dbo].[spa_telesales_CustomerByStatus] 
	-- Add the parameters for the stored procedure here
    @UserId UNIQUEIDENTIFIER ,
    @AppId INT ,
    @ProjectID INT ,
    @SourceID INT = 0 ,
    @statusId INT = 0 ,
    @statusCallId INT = 0 ,
    @DateStart DATETIME = NULL ,
    @DateEnd DATETIME = NULL ,
    @PageSize INT = 40 ,
    @Page INT = 1
AS
    BEGIN	

        WITH    tbData
                  AS ( SELECT 
                                cur.CustomerID ,
                                cur.MobilePhone ,
                                sr.Name AS SourceName ,
                                st.Name AS StatusName ,
                                stc.Name AS StatusCall_Name ,
                                cl.UpdateDate AS LastCallDate ,
                                ur.FirstName AS CallBy ,
                                COUNT(*) OVER ( ) AS TotalRow
                       FROM     dbo.telesales_Customer cur
                                JOIN dbo.telesales_Sources sr ON sr.SourceID = cur.SourceID
                                JOIN dbo.telesales_Call cl ON cl.CustomerID = cur.CustomerID
                                JOIN dbo.telesales_StatusCall stc ON stc.StatusCallID = cl.StatusCallID
                                JOIN dbo.telesales_Status st ON st.StatusID = stc.StatusID
                                JOIN dbo.telesales_Users ur ON ur.Id = cl.UserId
                       WHERE    ( ISNULL(@SourceID, '') = ''
                                  OR cur.SourceID = @SourceID
                                )
                                AND ( ISNULL(@DateStart, '') = ''
                                      OR cl.UpdateDate >= @DateStart
                                    )
                                AND ( ISNULL(@DateEnd, '') = ''
                                      OR cl.UpdateDate <= @DateEnd
                                    )
                                AND ( @statusCallId = 0
                                      OR cl.StatusCallID = @statusCallId
                                    )
                                AND ( @statusId = 0
                                      OR stc.StatusID = @statusCallId
                                    )
                     )
            SELECT  *
            FROM    tbData
            ORDER BY tbData.LastCallDate DESC
                    OFFSET @pageSize * ( @page - 1 ) ROWS
			FETCH NEXT @pageSize ROWS ONLY;
    END


