USE [EWAY]
GO
/****** Object:  StoredProcedure [dbo].[telesales_CustomerGetAllFieldValue]    Script Date: 12/20/2017 10:41:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Sonldt1>
-- Create date: <22/9/2016>
-- Description:	<Lấy tất cả các fieldValue cột dữ liệu customer>
-- telesales_CustomerGetAllFieldValue '395DE479-4E2F-44F2-BC2D-822B1D582EE5', 1, 1, null, null, null, 20, 1
-- =============================================
ALTER PROCEDURE [dbo].[telesales_CustomerGetAllFieldValue]
    
	@UserId UNIQUEIDENTIFIER ,
    @AppId INT ,
    @ProjectID INT = NULL ,
    @SourceID INT = NULL ,
    @Phone NVARCHAR(MAX) = NULL ,
	@DateFrom DATETIME = NULL ,
    @DateEnd DATETIME = NULL ,
    @Visible BIT = NULL ,
    @PageSize INT = NULL ,
    @Page INT = NULL
AS
    BEGIN
        DECLARE @cols AS NVARCHAR(MAX);
        DECLARE @query AS NVARCHAR(MAX);
        DECLARE @condition AS NVARCHAR(MAX);
        SELECT  @cols = STUFF((SELECT   ',' + QUOTENAME(c.FieldName)
                               FROM     dbo.telesales_ProjectCustomerField p
                                        INNER JOIN dbo.telesales_CustomerField c ON c.CustomerFieldID = p.CustomerFieldID
                               WHERE    p.ProjectID = @ProjectID
                               GROUP BY c.FieldName ,
										c.[Order],
                                        c.CustomerFieldID
                               ORDER BY c.[Order] ASC
                FOR           XML PATH('') ,
                                  TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 1, '');
       
        SET @condition = '';
	   
        SET @condition = @condition + 'AND cu.IsDeleted = 0 ';
        IF ( @Visible IS NOT NULL )
            BEGIN
                SET @condition = @condition + 'AND cu.Visiable = '
                    + CAST(@Visible AS NVARCHAR(1)) + '';
            END;
	
        IF ( @SourceID IS NOT NULL AND @SourceID > 0 )
            BEGIN
                SET @condition = @condition + ' AND s.SourceID = '
                    + CAST(@SourceID AS NVARCHAR(MAX)) + '';
            END;
		IF ( @DateFrom <> '' )
            BEGIN
                SET @condition += ' AND cu.CreatedDate >= '''
                    + CONVERT(VARCHAR(10), @DateFrom, 110) + ' 00:00:00'' ';
            END;
        IF ( @DateEnd <> '' )
            BEGIN
                SET @condition += ' AND cu.CreatedDate <= '''
                    + CONVERT(VARCHAR(10), @DateEnd, 110) + ' 23:59:59'' ';
            END;
--		ELSE
--			BEGIN
--			SET @condition = @condition + ' AND s.SourceID IN (SELECT  c.SourceID 
--FROM    dbo.telesales_Customer c
--WHERE   CustomerID IN ( SELECT  CustomerID
--                        FROM    dbo.telesales_ProjectCustomer
--                        WHERE   ProjectID = @ProjectID )
						
						
--GROUP BY c.SourceID)';
				
--			END;
        IF ( @Phone IS NOT NULL )
            BEGIN
                SET @condition = @condition + ' AND cu.MobilePhone LIKE ''%'
                    + CAST(@Phone AS VARCHAR(100)) + '%''';
            END;
		SET @query =' Declare @total INT

		SELECT @total = COUNT(1)
        
FROM    ( SELECT    *
          FROM      ( SELECT    a.CustomerID,
                                a.FieldValue ,
                                b.FieldName
								
                      FROM      dbo.telesales_CustomerFieldValue a
                                INNER JOIN dbo.telesales_CustomerField b ON b.CustomerFieldID = a.CustomerFieldID
                    ) x PIVOT ( MAX(x.FieldValue) FOR x.FieldName IN ( '
                + CONVERT(NVARCHAR(4000), @cols) + ')) AS pv
        ) x
        INNER JOIN dbo.telesales_Customer cu ON cu.CustomerID = x.CustomerID
		INNER JOIN dbo.telesales_Sources s ON s.SourceID = cu.SourceID
		WHERE cu.IsDeleted = 0 ' + @condition + '
		'
        IF ( @PageSize IS NULL )
            SET @query += '
		SELECT  cu.MobilePhone,cu.Visiable,s.SourceID,s.Name AS SourceName, @total AS Total,cu.CreatedDate, x.*
        
FROM    ( SELECT    *
          FROM      ( SELECT    a.CustomerID,
                                a.FieldValue ,
                                b.FieldName
								
                      FROM      dbo.telesales_CustomerFieldValue a
                                INNER JOIN dbo.telesales_CustomerField b ON b.CustomerFieldID = a.CustomerFieldID
                    ) x PIVOT ( MAX(x.FieldValue) FOR x.FieldName IN ( '
                + CONVERT(NVARCHAR(4000), @cols) + ')) AS pv
        ) x
        INNER JOIN dbo.telesales_Customer cu ON cu.CustomerID = x.CustomerID
		INNER JOIN dbo.telesales_Sources s ON s.SourceID = cu.SourceID
		WHERE cu.IsDeleted = 0 ' + @condition + '
		';
        ELSE
            SET @query += '
		SELECT cu.MobilePhone,cu.Visiable,s.SourceID,s.Name AS SourceName,@total AS Total, cu.CreatedDate,x.*
        
FROM    ( SELECT    *
          FROM      ( SELECT    a.CustomerID,
                                a.FieldValue ,
                                b.FieldName
								
                      FROM      dbo.telesales_CustomerFieldValue a
                                INNER JOIN dbo.telesales_CustomerField b ON b.CustomerFieldID = a.CustomerFieldID
                    ) x PIVOT ( MAX(x.FieldValue) FOR x.FieldName IN ( '
                + CONVERT(NVARCHAR(4000), @cols) + ')) AS pv
        ) x
        INNER JOIN dbo.telesales_Customer cu ON cu.CustomerID = x.CustomerID
		INNER JOIN dbo.telesales_Sources s ON s.SourceID = cu.SourceID
		WHERE cu.IsDeleted = 0 ' + @condition+ '
		ORDER BY CustomerID OFFSET @PageSize * (@Page - 1) ROWS FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE)';
		
		
		PRINT ( @query );
        EXEC sp_executesql @query, N'@PageSize INT, @Page INT, @ProjectID INT', @PageSize,
            @Page, @ProjectID;
       

       

        
    END;
