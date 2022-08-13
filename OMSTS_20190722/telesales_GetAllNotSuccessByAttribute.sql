-- =============================================
--  Name        Date      Description
--  thientc     26/07/2019    Lay danh sach các cuộc gọi không thành công từ thuộc tính
--  telesales_GetAllCallNotSuccessByDTV '395DE479-4E2F-44F2-BC2D-822B1D582EE5' ,1,1,null, 20,1
-- =============================================
create proc [telesales_GetAllCallNotSuccessByAttribute]
@UserId uniqueidentifier,
@AppId int,
@ProjectID int,
@FieldName uniqueidentifier,
@PageSize int = null,
@Page int = null
as
	begin
		declare @sql nvarchar(4000)
		declare @condition nvarchar(4000)

		set @condition = ''
		if(@FieldName is not null)
			begin
				set @condition += ' and c.UserId = ''' + cast(@FieldName as nvarchar(max)) + ''''
			end
		set @sql = 'Declare @total int 
			select @total = count(1) from dbo.telesales_Call c
			inner join dbo.telesales_StatusCall sc on sc.StatusCallID = c.StatusCallID
			inner join dbo.telesales_Status st on st.StatusID = sc.StatusID