-- =============================================
--	Name				Date			Description
--	thientc			    26/07/2019	    Lấy thuộc tính cột dữ liệu Customer.
--  spa_telesales_CustomerByStatus '395DE479-4E2F-44F2-BC2D-822B1D582EE5' ,1,@ProjectID=3031,@SourceID=0,@statusId=0,@statusCallId=0,@DateStart='',@DateEnd='',@PageSize=40 , @Page=1
-- =============================================
create proc [spa_telesales_CustomerField_GetAll]
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
as  
	begin
		create table #tbCustomer
		(
			CustomerID UNIQUEIDENTIFIER ,
              MobilePhone VARCHAR(20) ,
              SourceID INT ,
              SourceName NVARCHAR(200) ,
              CreatedDate DATETIME ,
              StatusName NVARCHAR(500) ,
              StatusCallName NVARCHAR(500) ,
              Visiable BIT ,
              IsDeleted BIT
		);
		insert into #tbCustomer
		(
			CustomerID ,
            MobilePhone ,
            SourceID ,
            SourceName ,
            CreatedDate ,
            StatusName ,
            StatusCallName ,
            Visiable ,
            IsDeleted		
		)
		SELECT  c.CustomerID ,
                        c.MobilePhone ,
                        c.SourceID ,
                        sr.Name ,
                        c.CreatedDate ,
                        st.Name , 
                        stc.Name ,
                        c.Visiable ,
                        c.IsDeleted
                FROM    dbo.telesales_Customer c
                        LEFT JOIN dbo.telesales_Call cll ON cll.CustomerID = c.CustomerID
                        JOIN dbo.telesales_StatusCall stc ON stc.StatusCallID = cll.StatusCallID
                        JOIN dbo.telesales_Status st ON st.StatusID = stc.StatusID
                        JOIN dbo.telesales_Sources sr ON sr.SourceID = c.SourceID
                WHERE   ( ISNULL(@SourceID, '') = ''
                          OR c.SourceID = @SourceID
                        )
                        AND ( ISNULL(@Phone, '') = ''
                              OR c.MobilePhone LIKE N'%' + @Phone + '%'
                            )
                        AND ( ISNULL(@DateFrom, '') = ''
                              OR c.CreatedDate >= @DateFrom
                            )
                        AND ( ISNULL(@DateEnd, '') = ''
                              OR c.CreatedDate <= @DateEnd
                            )
                        AND ( ISNULL(@Visible, '') = ''
                              OR c.Visiable = @Visible
                            );	   
					