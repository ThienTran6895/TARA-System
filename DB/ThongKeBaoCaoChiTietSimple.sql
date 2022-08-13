-- =============================================
--	Name				Date			Description
--	sonldt			27/12/2016		  Thống kê theo điện thoại viên
--  ThongKeBaoCaoChiTietSimple '395DE479-4E2F-44F2-BC2D-822B1D582EE5' ,1,null, null,3003,20,1
-- =============================================
ALTER PROCEDURE [dbo].[ThongKeBaoCaoChiTietSimple]
    @UserId UNIQUEIDENTIFIER ,
    @AppId INT ,
    @DateFrom DATETIME = NULL ,
    @DateEnd DATETIME = NULL ,
    @ProjectID INT ,
    @PageSize INT = NULL ,
    @Page INT = NULL
AS
    BEGIN
------------------------------------------------------------------------------------------------
        DECLARE @ColumnName NVARCHAR(MAX);
        SELECT  @ColumnName = ISNULL(@ColumnName + ',', '')
                + QUOTENAME(FieldCode)
        FROM    ( SELECT DISTINCT
                            cf.FieldCode
                  FROM      dbo.telesales_CustomerField cf
                            INNER JOIN dbo.telesales_ProjectCustomerField pcf ON pcf.CustomerFieldID = cf.CustomerFieldID
                  WHERE     pcf.ProjectID = @ProjectID
                ) AS Courses;


---------------- sql----------------------------------------------------------------
        DECLARE @Condition NVARCHAR(MAX)= '';
   
        IF ( @DateFrom <> '' )
            BEGIN
                SET @Condition += ' AND c.UpdateDate >= '''
                    + CONVERT(VARCHAR(10), @DateFrom, 110) + ' 00:00:00'' ';
                
            END;
        IF ( @DateEnd <> '' )
            BEGIN
                SET @Condition += ' AND c.UpdateDate <= '''
                    + CONVERT(VARCHAR(10), @DateEnd, 110) + ' 23:59:59'' ';
                
            END;

        DECLARE @Sql NVARCHAR(MAX)= '';	
        SET @Sql = 'SELECT DISTINCT
									cu.MobilePhone ,
									c.UpdateDate AS NgayGoiSauCung ,
									sc.Name AS TinhTrangCuocGoiSauCung ,
									p.UserName AS NguoiGoiSauCung ,
									(SELECT TOP 1 (Note)  FROM telesales_CallLog WHERE CallID = c.CallID ORDER BY CallLogID DESC) AS GhiChuCuoiCung,
									( SELECT    COUNT(1)
									  FROM      dbo.telesales_CallLog
									  WHERE     CallID = c.CallID
									) AS TongSoLanGoi,
									c.CallID ,
									t.* 
						  FROM      dbo.telesales_Call c
									LEFT JOIN dbo.telesales_Customer cu ON cu.CustomerID = c.CustomerID
									LEFT JOIN dbo.telesales_StatusCall sc ON sc.StatusCallID = c.StatusCallID
									LEFT JOIN dbo.telesales_Users p ON p.Id = c.UserId
									LEFT JOIN ( SELECT *
												 FROM   ( SELECT  cfv.CustomerID ,
																  cf.FieldCode ,
																  cfv.FieldValue
														  FROM    dbo.telesales_CustomerFieldValue cfv
																  INNER JOIN dbo.telesales_CustomerField cf ON cf.CustomerFieldID = cfv.CustomerFieldID
														) x PIVOT ( MAX(FieldValue) FOR x.FieldCode IN ( '
            + @ColumnName
            + ') )AS PV
											   ) t ON t.CustomerID = c.CustomerID
						  WHERE     c.ProjectID = '
            + CONVERT(NVARCHAR(50), @ProjectID) + ' ' + @Condition + ' ORDER BY UpdateDate DESC;'

        DECLARE @SqlPaging NVARCHAR(MAX)

		 -- if co phan trang
		 IF @PageSize IS NOT NULL
		 BEGIN
			SET @SqlPaging = @SqlPaging + ' OFFSET  ' + CONVERT(VARCHAR(50), @PageSize * ( @Page - 1 ))
										+ ' ROWS FETCH NEXT ' + CONVERT(VARCHAR(50), @PageSize) + '
												ROWS ONLY
												OPTION  ( RECOMPILE );';
		 END

        PRINT ( @Sql );
        PRINT ( @SqlPaging );
        EXEC (@Sql + ' ' + @SqlPaging);
    END