ALTER PROCEDURE [dbo].[usp_Add_Conversation]
	@ConversationId nvarchar(MAX),
	@UserId nvarchar(MAX),
	@ConversationDate nvarchar(30),
	@ConversationLog nvarchar(MAX)
WITH EXECUTE AS OWNER
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [Conversation](ConversationId, UserId, ConversationDate, ConversationLog) 
	VALUES(@ConversationId, @UserId, @ConversationDate, @ConversationLog)
END
GO