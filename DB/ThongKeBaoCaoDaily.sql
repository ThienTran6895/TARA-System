-- =============================================
--	Name				Date			Description
--	vunn			08/01/2018		  Thống kê overview
--  ThongKeBaoCaoDaily '395DE479-4E2F-44F2-BC2D-822B1D582EE5' ,1,3003
-- =============================================
CREATE PROCEDURE [dbo].[ThongKeBaoCaoDaily]
    @UserId UNIQUEIDENTIFIER ,
    @AppId INT ,
	@ProjectID INT,
	@DateFrom DATETIME = NULL ,
    @DateEnd DATETIME = NULL
AS
    BEGIN
				DECLARE @TotalDays AS INT
				DECLARE @SqlUpdateTemplate AS NVARCHAR(MAX);
				DECLARE @SqlUpdate AS NVARCHAR(MAX);

				SELECT @TotalDays = DATEDIFF(DAY, @DateFrom, @DateEnd) + 1

				SELECT sc.StatusID,sc.Name AS StatusName, COUNT(*) AS Total
				INTO #Overall
				FROM dbo.telesales_Call c
									INNER JOIN dbo.telesales_StatusCall sc ON sc.StatusCallID = c.StatusCallID
									INNER JOIN dbo.telesales_Status st ON st.StatusID = sc.StatusID
				WHERE c.ProjectID = @ProjectID AND c.CreatedDate >= @DateFrom AND c.CreatedDate <= @DateEnd
				GROUP BY sc.StatusID, sc.Name
				ORDER BY sc.StatusID
				--SELECT * FROM #Overall ORDER BY StatusID
				

				SELECT  sc.StatusID,sc.Name AS StatusName, COUNT(*) AS Total, DATEDIFF(DAY, @DateEnd , c.CreatedDate) AS DateOfCall
				INTO #DailyReport
				FROM      dbo.telesales_Call c
									INNER JOIN dbo.telesales_StatusCall sc ON sc.StatusCallID = c.StatusCallID
									INNER JOIN dbo.telesales_Status st ON st.StatusID = sc.StatusID
				WHERE c.ProjectID = @ProjectID  AND c.CreatedDate >= @DateFrom AND c.CreatedDate <= @DateEnd
				GROUP BY sc.StatusID, sc.Name, DATEDIFF(DAY, @DateEnd , c.CreatedDate)
				ORDER BY DATEDIFF(DAY, @DateEnd , c.CreatedDate), sc.StatusID

				--SELECT * FROM #DailyReport ORDER BY StatusID, StatusName, DateOfCall

				-- Create temp report table
				SET @SqlUpdate = '
					CREATE TABLE ##Report 
					(
						StatusID int, 
						StatusName nvarchar(255),
						Total int,
						SL int, 
						Vol nvarchar(10), 
				'
				DECLARE @Count INT
				SET @Count = 1
				SET @SqlUpdateTemplate = '
						Total[ColumnNo] int, 
						SL[ColumnNo] int, 
						Vol[ColumnNo] nvarchar(10) 
					'
				WHILE (@Count <= @TotalDays)
				BEGIN					
					SET @SqlUpdate = @SqlUpdate + REPLACE(@SqlUpdateTemplate,'[ColumnNo]', CAST(@Count AS VARCHAR(10)));
					-- new line
					IF( @Count < @TotalDays)
					BEGIN
						SET @SqlUpdate = @SqlUpdate  + ', ';
					END
					SET @Count = @Count + 1					
				END
				SET @SqlUpdate = @SqlUpdate  + ');';
				EXEC (@SqlUpdate);

			-- Interate through all status
			DECLARE @StatusID  AS INT
			DECLARE @StatusName AS NVARCHAR(255)
			DECLARE @Total  AS INT

			DECLARE @TotalOverall  AS INT
			DECLARE @Total7Days  AS INT
			SELECT @TotalOverall = SUM(Total) FROM #Overall
			SELECT @Total7Days = SUM(Total) FROM #DailyReport
 
			DECLARE @StatusCursor as CURSOR; 
			SET @StatusCursor = CURSOR FOR
			SELECT StatusID,		
				   StatusName,
				   Total
			 FROM #Overall ORDER BY StatusID;
 
			OPEN @StatusCursor;
			FETCH NEXT FROM @StatusCursor INTO @StatusID, @StatusName, @Total
 
			-- Overall 
			WHILE @@FETCH_STATUS = 0
			BEGIN
				INSERT INTO ##Report (StatusID, StatusName, Total, SL, Vol) 
				VALUES (@StatusID, @StatusName, @TotalOverall, @Total, CAST(CAST((@Total*1.0/@TotalOverall)*100 AS DECIMAL(18,2)) AS VARCHAR(10)) + '%')
			 -- Fetch calls of another phone number
			 FETCH NEXT FROM @StatusCursor INTO @StatusID, @StatusName, @Total
			END
			 
			CLOSE @StatusCursor;
			DEALLOCATE @StatusCursor;

			-- Sum overall
			INSERT INTO ##Report (StatusID, StatusName, Total, SL, Vol) 
				VALUES (0, 'Total data calling', @TotalOverall, @TotalOverall, '100%')

			-- Calculating sum by date
			DECLARE @TotalOfDay INT
			DECLARE @DateCount INT
			DECLARE @DateAdded DATETIME

			SELECT @DateAdded = @DateFrom
			SET @SqlUpdateTemplate = 'UPDATE ##Report SET SL[ColumnNo] = [1], Vol[ColumnNo] = ''[2]'' WHERE StatusID = 0;'
			SET @DateCount = 1
			WHILE @DateAdded < @DateEnd
			BEGIN									
				SELECT @TotalOfDay = COUNT(*) FROM telesales_Call 
				WHERE ProjectID = @ProjectID  AND DATEDIFF(DAY, @DateAdded, CreatedDate) = 0
				GROUP BY CAST(CreatedDate as Date)

				
				-- Update the total calls to the temp report
				SET @SqlUpdate = REPLACE(@SqlUpdateTemplate,'[ColumnNo]', CAST(@DateCount AS VARCHAR(10)));
				SET @SqlUpdate = REPLACE(@SqlUpdate,'[1]', CAST(@TotalOfDay AS NVARCHAR(32)));
				SET @SqlUpdate = REPLACE(@SqlUpdate,'[2]', '100%');

				EXEC (@SqlUpdate);

				-- Update the total column
				SET @SqlUpdate = REPLACE('UPDATE ##Report SET Total[ColumnNo] = ','[ColumnNo]', CAST(@DateCount AS VARCHAR(10))) + CAST(@TotalOfDay AS NVARCHAR(32))
				EXEC (@SqlUpdate);

				-- Next date
				SET @DateAdded = DATEADD(DAY, 1, @DateAdded) 
				SET @DateCount = @DateCount + 1
				SET @TotalOfDay = 0
			END

			
			-- Last 7 days
			DECLARE @SID  AS INT
			DECLARE @SName AS NVARCHAR(255)
			DECLARE @STotal  AS INT
			DECLARE @STotalCall  AS INT
			DECLARE @SDay  AS INT
			DECLARE @SCursor as CURSOR; 
			SET @SCursor = CURSOR FOR
			SELECT *
			FROM #DailyReport ORDER BY StatusID, StatusName, DateOfCall

			DECLARE @LastID  AS INT
			DECLARE @LastName AS NVARCHAR(255)
			DECLARE @CurrentDay  AS INT
			DECLARE @DayTotal  AS INT
			OPEN @SCursor;
			FETCH NEXT FROM @SCursor INTO @SID, @SName, @STotal, @SDay
			-- Save the last status and day
			SELECT @LastID = @SID, @LastName = @SName, @CurrentDay = @SDay

			
			SET @SqlUpdateTemplate = 'UPDATE ##Report SET SL[ColumnNo] = [1], Vol[ColumnNo] = CAST(CAST(([2]*1.0/Total[ColumnNo])*100 AS DECIMAL(18,2)) AS VARCHAR(5)) + ''%'' WHERE StatusID = [StatusID] AND StatusName = N''[StatusName]'';'
			WHILE @@FETCH_STATUS = 0
			BEGIN				
				 -- Check if there are same status in last 7 days
				 WHILE @LastID = @SID AND @LastName = @SName AND @@FETCH_STATUS = 0
				 BEGIN
					-- Update the call info into correspondance colume
					SET @SqlUpdate = REPLACE(@SqlUpdateTemplate,'[ColumnNo]', CAST(@TotalDays+@SDay AS VARCHAR(10)));
					SET @SqlUpdate = REPLACE(@SqlUpdate,'[1]', CAST(@STotal AS NVARCHAR(32)));
					SET @SqlUpdate = REPLACE(@SqlUpdate,'[2]', CAST(@STotal AS NVARCHAR(32)));

					SET @SqlUpdate = REPLACE(@SqlUpdate,'[StatusID]', @SID);
					SET @SqlUpdate = REPLACE(@SqlUpdate,'[StatusName]', @SName);

					-- Set SL1, SL2, SL3...for the number
					--PRINT @SqlUpdate;
					EXEC (@SqlUpdate);
					-- Next call
					FETCH NEXT FROM @SCursor INTO @SID, @SName, @STotal, @SDay
				END
				-- New status
				SELECT @LastID = @SID, @LastName = @SName, @CurrentDay = @SDay

			END
 
			CLOSE @SCursor;
			DEALLOCATE @SCursor;
			
			SELECT * FROM ##Report
			DROP TABLE 	#DailyReport
			DROP TABLE 	#Overall
			DROP TABLE 	##Report
    END;