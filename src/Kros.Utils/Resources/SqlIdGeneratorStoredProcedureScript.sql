CREATE PROCEDURE [spGetNewId]
(
	@TableName nvarchar(100) = '',
	@NumberOfItems int = 1
)
AS
BEGIN
	set nocount on

	begin transaction
	save transaction transSavePoint

	begin try

		declare @lastId int

		SELECT @lastId = LastId FROM IdStore WITH (XLOCK) WHERE (TableName = @TableName)

		if (@lastId is null)
		begin
			INSERT INTO IdStore (TableName, LastId) VALUES (@TableName, @NumberOfItems)
			set @lastId = 1
		end
		else
		begin
			UPDATE IdStore SET LastId = @lastId + @NumberOfItems WHERE (TableName = @TableName)
			set @lastId = @lastId + 1
		end

		select @lastId

	end try
	begin catch

		if @@TRANCOUNT > 0
		begin
			rollback transaction transSavePoint
		end

		declare @errorMessage nvarchar(4000)
		declare @errorSeverity int
		declare @errorState int

		select @errorMessage = ERROR_MESSAGE()
		select @errorSeverity = ERROR_SEVERITY()
		select @errorState = ERROR_STATE()

		raiserror (@errorMessage, @errorSeverity, @errorState)

	end catch

	commit transaction
END
