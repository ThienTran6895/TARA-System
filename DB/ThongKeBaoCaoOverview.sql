-- =============================================
--	Name				Date			Description
--	vunn			08/01/2018		  Thống kê overview
--  ThongKeBaoCaoOverview '395DE479-4E2F-44F2-BC2D-822B1D582EE5' ,1,null, null,3003,20,1
-- =============================================
ALTER PROCEDURE [dbo].[ThongKeBaoCaoOverview]
    @UserId UNIQUEIDENTIFIER ,
    @AppId INT ,
	@ProjectID INT
AS
    BEGIN
				-- OVERVIEW
                SELECT  'Overview    ' AS TimePeriod ,
						p.TotalPlan AS XPlan,
						p.TotalTarget AS XTarget,
                        /*s.SourceID ,
                        MONTH(s.CreatedDate) AS Thang ,*/
                        ( SELECT    COUNT(1)
                          FROM      dbo.telesales_Customer
                          WHERE     SourceID = s.SourceID
                        ) AS TongSo ,/*
                        ( SELECT    COUNT(1)
                          FROM      dbo.telesales_ProjectCustomer
                          WHERE     CustomerID IN (
                                    SELECT  CustomerID
                                    FROM    dbo.telesales_Customer
                                    WHERE   SourceID = s.SourceID )
                        ) AS DaPhanCong ,*/
                        ( SELECT    COUNT(1)
                          FROM      dbo.telesales_ProjectCustomer
                          WHERE     CustomerID IN (
                                    SELECT  CustomerID
                                    FROM    dbo.telesales_Customer
                                    WHERE   SourceID = s.SourceID )
                                    AND IsCall = 1
                        ) AS DaXuLy ,
						( SELECT    COUNT(1)
						  FROM      dbo.telesales_Call c
									INNER JOIN dbo.telesales_StatusCall sc ON sc.StatusCallID = c.StatusCallID
									INNER JOIN dbo.telesales_Status st ON st.StatusID = sc.StatusID
						  WHERE     c.IsSuccess = 1
									AND st.StatusID = 1
						) AS KHThanhCong ,/*
                        ( SELECT    COUNT(1)
                          FROM      dbo.telesales_ProjectCustomer
                          WHERE     CustomerID IN (
                                    SELECT  CustomerID
                                    FROM    dbo.telesales_Customer
                                    WHERE   SourceID = s.SourceID )
                                    AND IsCall = 0
                        ) AS ChuaXuLy ,
                        ( SELECT    COUNT(1)
                          FROM      dbo.telesales_ProjectCustomer pc
                                    INNER JOIN dbo.telesales_Call c ON c.CustomerID = pc.CustomerID
                                    INNER JOIN dbo.telesales_StatusCall sc ON sc.StatusCallID = c.StatusCallID
                                    INNER JOIN dbo.telesales_Status st ON st.StatusID = sc.StatusID
                          WHERE     pc.CustomerID IN (
                                    SELECT  CustomerID
                                    FROM    dbo.telesales_Customer
                                    WHERE   SourceID = s.SourceID )
                                    AND IsCall = 1
                                    AND c.IsSuccess = 1
                                    AND st.StatusID = 4
                        ) AS DangHenGoiLai,	*/					
						( SELECT    COUNT(1)
						  FROM      dbo.telesales_Call c
									INNER JOIN dbo.telesales_StatusCall sc ON sc.StatusCallID = c.StatusCallID
									INNER JOIN dbo.telesales_Status st ON st.StatusID = sc.StatusID
									INNER JOIN dbo.telesales_Users u ON c.UserId = u.Id
						  WHERE     c.IsSuccess = 0
									AND st.StatusID = 3
						) AS KHSaiSo/* ,
						(SELECT COUNT(1) FROM dbo.telesales_Sources)AS Total */
                INTO #Overview
				FROM    dbo.telesales_Sources s
				INNER JOIN dbo.telesales_Project p ON p.ProjectID = @ProjectID

				-- TODAY
				INSERT INTO #Overview
                SELECT 'Today' AS TimePeriod ,
						p.DailyPlan AS XPlan,
						p.DailyTarget AS XTarget,
                        ( SELECT    COUNT(1)
                          FROM      dbo.telesales_Customer
                          WHERE     SourceID = s.SourceID AND CreatedDate = CAST(CURRENT_TIMESTAMP AS DATE) 
                        ) AS TongSo ,
                        ( SELECT    COUNT(1)
                          FROM      dbo.telesales_ProjectCustomer
                          WHERE     CustomerID IN (
                                    SELECT  CustomerID
                                    FROM    dbo.telesales_Customer
                                    WHERE   SourceID = s.SourceID )
                                    AND IsCall = 1  AND CreatedDate = CAST(CURRENT_TIMESTAMP AS DATE) 
                        ) AS DaXuLy ,
						( SELECT    COUNT(1)
						  FROM      dbo.telesales_Call c
									INNER JOIN dbo.telesales_StatusCall sc ON sc.StatusCallID = c.StatusCallID
									INNER JOIN dbo.telesales_Status st ON st.StatusID = sc.StatusID
						  WHERE     c.IsSuccess = 1
									AND st.StatusID = 1  AND c.CreatedDate = CAST(CURRENT_TIMESTAMP AS DATE) 
						) AS KHThanhCong ,			
						( SELECT    COUNT(1)
						  FROM      dbo.telesales_Call c
									INNER JOIN dbo.telesales_StatusCall sc ON sc.StatusCallID = c.StatusCallID
									INNER JOIN dbo.telesales_Status st ON st.StatusID = sc.StatusID
									INNER JOIN dbo.telesales_Users u ON c.UserId = u.Id
						  WHERE     c.IsSuccess = 0
									AND st.StatusID = 3  AND c.CreatedDate = CAST(CURRENT_TIMESTAMP AS DATE) 
						) AS KHSaiSo
				FROM    dbo.telesales_Sources s
				INNER JOIN dbo.telesales_Project p ON p.ProjectID = @ProjectID

				-- YESTERDAY
				INSERT INTO #Overview
                SELECT 'Yesterday' AS TimePeriod ,
						p.DailyPlan AS XPlan,
						p.DailyTarget AS XTarget,
                        ( SELECT    COUNT(1)
                          FROM      dbo.telesales_Customer
                          WHERE     SourceID = s.SourceID AND CreatedDate = CAST(CURRENT_TIMESTAMP AS DATE) 
                        ) AS TongSo ,
                        ( SELECT    COUNT(1)
                          FROM      dbo.telesales_ProjectCustomer
                          WHERE     CustomerID IN (
                                    SELECT  CustomerID
                                    FROM    dbo.telesales_Customer
                                    WHERE   SourceID = s.SourceID )
                                    AND IsCall = 1  AND CreatedDate = CAST(CURRENT_TIMESTAMP AS DATE) 
                        ) AS DaXuLy ,
						( SELECT    COUNT(1)
						  FROM      dbo.telesales_Call c
									INNER JOIN dbo.telesales_StatusCall sc ON sc.StatusCallID = c.StatusCallID
									INNER JOIN dbo.telesales_Status st ON st.StatusID = sc.StatusID
						  WHERE     c.IsSuccess = 1
									AND st.StatusID = 1  AND c.CreatedDate = CAST(CURRENT_TIMESTAMP AS DATE) 
						) AS KHThanhCong ,			
						( SELECT    COUNT(1)
						  FROM      dbo.telesales_Call c
									INNER JOIN dbo.telesales_StatusCall sc ON sc.StatusCallID = c.StatusCallID
									INNER JOIN dbo.telesales_Status st ON st.StatusID = sc.StatusID
									INNER JOIN dbo.telesales_Users u ON c.UserId = u.Id
						  WHERE     c.IsSuccess = 0
									AND st.StatusID = 3  AND c.CreatedDate = CAST(CURRENT_TIMESTAMP AS DATE) 
						) AS KHSaiSo
				FROM    dbo.telesales_Sources s
				INNER JOIN dbo.telesales_Project p ON p.ProjectID = @ProjectID

				-- THIS MONTH
				INSERT INTO #Overview
                SELECT 'This Month' AS TimePeriod ,
						p.DailyPlan AS XPlan,
						p.DailyTarget AS XTarget,
                        ( SELECT    COUNT(1)
                          FROM      dbo.telesales_Customer
                          WHERE     SourceID = s.SourceID AND CreatedDate = CAST(CURRENT_TIMESTAMP AS DATE) 
                        ) AS TongSo ,
                        ( SELECT    COUNT(1)
                          FROM      dbo.telesales_ProjectCustomer
                          WHERE     CustomerID IN (
                                    SELECT  CustomerID
                                    FROM    dbo.telesales_Customer
                                    WHERE   SourceID = s.SourceID )
                                    AND IsCall = 1  AND CreatedDate = CAST(CURRENT_TIMESTAMP AS DATE) 
                        ) AS DaXuLy ,
						( SELECT    COUNT(1)
						  FROM      dbo.telesales_Call c
									INNER JOIN dbo.telesales_StatusCall sc ON sc.StatusCallID = c.StatusCallID
									INNER JOIN dbo.telesales_Status st ON st.StatusID = sc.StatusID
						  WHERE     c.IsSuccess = 1
									AND st.StatusID = 1  AND c.CreatedDate = CAST(CURRENT_TIMESTAMP AS DATE) 
						) AS KHThanhCong ,			
						( SELECT    COUNT(1)
						  FROM      dbo.telesales_Call c
									INNER JOIN dbo.telesales_StatusCall sc ON sc.StatusCallID = c.StatusCallID
									INNER JOIN dbo.telesales_Status st ON st.StatusID = sc.StatusID
									INNER JOIN dbo.telesales_Users u ON c.UserId = u.Id
						  WHERE     c.IsSuccess = 0
									AND st.StatusID = 3  AND c.CreatedDate = CAST(CURRENT_TIMESTAMP AS DATE) 
						) AS KHSaiSo
				FROM    dbo.telesales_Sources s
				INNER JOIN dbo.telesales_Project p ON p.ProjectID = @ProjectID

				SELECT 
				TimePeriod,
				XPlan, 
				XTarget, 
				CAST(CAST((XTarget*1.0/XPlan)*100 AS DECIMAL(18,2)) AS VARCHAR(5)) + '%' AS PercentTarget,  
				TongSo AS TotalReceived, 
				CAST(CAST((TongSo*1.0/XPlan)*100 AS DECIMAL(18,2)) AS VARCHAR(5)) + '%' AS PercentReceived,  
				DaXuLy AS TotalHandled,
				IIF(TongSo <> 0, CAST(CAST((DaXuLy*1.0/TongSo)*100 AS DECIMAL(18,2)) AS VARCHAR(5)), '0') + '%' AS PercentHandled, 
				KHSaiSo AS TotalIncorrect, 
				IIF(TongSo <> 0, CAST(CAST((KHSaiSo*1.0/TongSo)*100 AS DECIMAL(18,2)) AS VARCHAR(5)), '0') + '%' AS PercentIncorrect,  
				KHThanhCong AS TotalOrder,
				CAST(CAST((KHThanhCong*1.0/XTarget)*100 AS DECIMAL(18,2)) AS VARCHAR(5)) + '%' AS PercentOrder,
				CAST(CAST((XTarget*1.0/XPlan)*100 - (KHThanhCong*1.0/XTarget)*100 AS DECIMAL(18,2)) AS VARCHAR(5)) + '%' AS GAP
				FROM #Overview

				DROP TABLE #Overview
    END;