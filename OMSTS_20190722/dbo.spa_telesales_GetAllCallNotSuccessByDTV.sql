USE [OMSTS_DBONL_0719]
GO
/****** Object:  StoredProcedure [dbo].[spa_telesales_GetAllCallNotSuccessByDTV]    Script Date: 7/29/2019 5:14:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
--	Name				Date			Description
--	thientc			    29/07/2019	    lấy danh sách cuộc gọi không thành công
--  spa_telesales_GetAllCallNotSuccessByDTV @UserId='395DE479-4E2F-44F2-BC2D-822B1D582EE5' ,@AppId=1,@ProjectID=3031,@DTV=null,@statusCallId=0,@MobilePhone=null,@PageSize=40 , @Page=1
-- =============================================

ALTER proc [dbo].[spa_telesales_GetAllCallNotSuccessByDTV]
	@UserId uniqueidentifier,
	@AppId  int,
	@ProjectID INT,    
	@DTV UNIQUEIDENTIFIER,	
	@StatusCallID int,
	@MobilePhone nvarchar(max) = null,
	@PageSize  int = null,
	@Page int   = NULL
as
	begin
		with tbData
			as (SELECT 
						cus.MobilePhone,
						st.Name AS StatusName ,
                        stc.Name ,
                        c.UpdateDate AS LastCallDate ,
                        ur.FirstName AS CallBy ,	
						(
							SELECT COUNT(1) 
							FROM dbo.telesales_CallLog
							WHERE CallID = c.CallID
						) AS NumCall,					
						COUNT(*) OVER () AS TotalRow								
									FROM 								
										dbo.telesales_Call c                                
										JOIN dbo.telesales_Customer cus ON cus.CustomerID = c.CustomerID
										JOIN dbo.telesales_StatusCall stc ON stc.StatusCallID = c.StatusCallID
										JOIN dbo.telesales_Status st ON st.StatusID = stc.StatusID
										JOIN dbo.telesales_Users ur ON ur.Id = c.UserId
										JOIN dbo.telesales_CallLog cl ON cl.CallID = c.CallID
									WHERE    ( @statusCallId = 0
										OR c.StatusCallID = @statusCallId
											)
										AND c.ProjectID = @ProjectID
										AND c.IsSuccess = 0
										AND c.UserId = '' + CAST(@DTV AS NVARCHAR(MAX))
										AND (cus.MobilePhone LIKE N'%' + @MobilePhone + '%' OR (ISNULL(@MobilePhone,'') = ''))
										AND st.StatusID IN (2,3)
                              )

			SELECT  *
            FROM    tbData
            ORDER BY tbData.LastCallDate DESC
                    OFFSET @pageSize * ( @page - 1 ) ROWS
			FETCH NEXT @pageSize ROWS ONLY;	
	end