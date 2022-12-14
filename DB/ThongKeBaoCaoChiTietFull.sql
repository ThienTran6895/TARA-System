-- =============================================
--	Name				Date			Description
--	vunn			02/01/2018		  Thống kê theo điện thoại viên
--  ThongKeBaoCaoChiTietFull '395DE479-4E2F-44F2-BC2D-822B1D582EE5' ,1,null, null,3003,20,1
-- =============================================
CREATE PROCEDURE [dbo].[ThongKeBaoCaoChiTietFull]
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
									t.*,
									CAST(NULL AS DATETIME) CD1,
									CAST(NULL AS NVARCHAR(255)) CStatus1,
									CAST(NULL AS NVARCHAR(255)) CUser1,
									CAST(NULL AS INT) CID1,
									CAST(NULL AS INT) CLID1,
									CAST(NULL AS DATETIME) CD2,
									CAST(NULL AS NVARCHAR(255)) CStatus2,
									CAST(NULL AS NVARCHAR(255)) CUser2,
									CAST(NULL AS INT) CID2,
									CAST(NULL AS INT) CLID2,
									CAST(NULL AS DATETIME) CD3,
									CAST(NULL AS NVARCHAR(255)) CStatus3,
									CAST(NULL AS NVARCHAR(255)) CUser3,
									CAST(NULL AS INT) CID3,
									CAST(NULL AS INT) CLID3,
									CAST(NULL AS DATETIME) CD4,
									CAST(NULL AS NVARCHAR(255)) CStatus4,
									CAST(NULL AS NVARCHAR(255)) CUser4,
									CAST(NULL AS INT) CID4,
									CAST(NULL AS INT) CLID4,
									CAST(NULL AS DATETIME) CD5,
									CAST(NULL AS NVARCHAR(255)) CStatus5,
									CAST(NULL AS NVARCHAR(255)) CUser5,
									CAST(NULL AS INT) CID5,
									CAST(NULL AS INT) CLID5,
									CAST(NULL AS DATETIME) CD6,
									CAST(NULL AS NVARCHAR(255)) CStatus6,
									CAST(NULL AS NVARCHAR(255)) CUser6,
									CAST(NULL AS INT) CID6,
									CAST(NULL AS INT) CLID6
						  INTO ##Customers
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
        EXEC ( @Sql + ' ' + @SqlPaging);

		-- Get all related call
				SELECT	cu.MobilePhone,		
                sc1.Name AS CallStatus,
                p1.UserName,
                c1.CallID,
                c1.CallLogID,
				c1.CreatedDate
        INTO    #Calls
        FROM    dbo.telesales_CallLog c1
				INNER JOIN telesales_Call c ON c1.CallID = c.CallID
				INNER JOIN dbo.telesales_Customer cu ON cu.CustomerID = c.CustomerID
                INNER JOIN dbo.telesales_StatusCall sc1 ON sc1.StatusCallID = c1.StatusCallID
                INNER JOIN dbo.telesales_Users p1 ON p1.Id = c1.UserId
        WHERE   c1.ProjectId = 3025 AND cu.MobilePhone IN (SELECT MobilePhone FROM ##Customers)
		ORDER BY MobilePhone, CreatedDate

		-- Interate through all calls 
		DECLARE @MobilePhone AS NVARCHAR(255)
		DECLARE @CD AS DATETIME
		DECLARE @CStatus AS NVARCHAR(255)
		DECLARE @CUser AS NVARCHAR(255)
		DECLARE @CID  AS INT
		DECLARE @CLID  AS INT
 
		DECLARE @CallCursor as CURSOR;
 
		SET @CallCursor = CURSOR FOR
		SELECT MobilePhone,		
               CallStatus,
               UserName,
               CallID,
               CallLogID,
			   CreatedDate
		 FROM #Calls ORDER BY MobilePhone, CreatedDate;
 
		OPEN @CallCursor;
		FETCH NEXT FROM @CallCursor INTO @MobilePhone, @CStatus, @CUser, @CID, @CLID, @CD;
 
 		 DECLARE @Count  AS INT; SET @Count = 1;
		 DECLARE @CallNo  AS INT;
		 DECLARE @LastNumber  AS NVARCHAR(255);
		 DECLARE @SqlUpdateTemplate AS NVARCHAR(2000);
		 DECLARE @SqlUpdate AS NVARCHAR(2000);
		 SET @CallNo = 1;
		 SET @LastNumber = @MobilePhone
		 SET @SqlUpdateTemplate = 'UPDATE ##Customers SET CD[ColumnNo] = ''[1]'', CStatus[ColumnNo] = ''[2]'', CUser[ColumnNo] = ''[3]'', CID[ColumnNo] = [4], CLID[ColumnNo] = [5] WHERE MobilePhone = ''[MobilePhone]'';'
		WHILE @@FETCH_STATUS = 0
		BEGIN
			 -- Check if there are further calls for this number
			 WHILE @LastNumber = @MobilePhone AND @CallNo <= 6
			 BEGIN
				-- Update the call info into correspondance colume
				SET @SqlUpdate = REPLACE(@SqlUpdateTemplate,'[ColumnNo]', CAST(@CallNo AS VARCHAR(1)));
				SET @SqlUpdate = REPLACE(@SqlUpdate,'[1]', CAST(@CD AS NVARCHAR(256)));
				SET @SqlUpdate = REPLACE(@SqlUpdate,'[2]', @CStatus);
				SET @SqlUpdate = REPLACE(@SqlUpdate,'[3]', @CUser);
				SET @SqlUpdate = REPLACE(@SqlUpdate,'[4]', CAST(@CID AS NVARCHAR(32)));
				SET @SqlUpdate = REPLACE(@SqlUpdate,'[5]', CAST(@CLID AS NVARCHAR(32)));
				SET @SqlUpdate = REPLACE(@SqlUpdate,'[MobilePhone]', @MobilePhone);

				-- Set CD1, CD2, CD3...for the number
				PRINT @SqlUpdate;
				EXEC (@SqlUpdate);

				-- Next call
				FETCH NEXT FROM @CallCursor INTO  @MobilePhone, @CStatus, @CUser, @CID, @CLID, @CD;
				SET @CallNo = @CallNo + 1;

				SET @Count = @Count + 1;
				PRINT @Count	

			 END
		PRINT @Count
		SET @CallNo = 1;
		 SET @LastNumber = @MobilePhone
		 -- Fetch calls of another phone number
		 -- FETCH NEXT FROM @CallCursor INTO  @MobilePhone, @CStatus, @CUser, @CID, @CLID, @CD;
		END
 
		CLOSE @CallCursor;
		DEALLOCATE @CallCursor;
		
		-- Drop temp table
		--SELECT * FROM #Calls
		DROP TABLE #Calls

		-- Drop temp table
		SELECT * FROM ##Customers
		DROP TABLE ##Customers
    END