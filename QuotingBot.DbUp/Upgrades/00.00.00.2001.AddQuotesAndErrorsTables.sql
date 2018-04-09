PRINT N'Creating Table [dbo].[Quotes]...';
GO

CREATE TABLE [dbo].[Quotes]
(
	[ConversationId] [nvarchar](MAX) NOT NULL,
	[QuoteId] [nvarchar](MAX) NOT NULL,
	[QuoteInfo] [nvarchar](MAX) NULL
);
GO

PRINT N'Granting permissions on [dbo].[Quotes]...';
GO

GRANT SELECT, INSERT ON [dbo].[Quotes] TO [ApplicationRole] AS [dbo];
GO

PRINT N'Creating Procedure usp_Add_Quote...';
GO

CREATE PROCEDURE usp_Add_Quote
	@ConversationId nvarchar(MAX),
	@QuoteId nvarchar(MAX),
	@QuoteInfo nvarchar(MAX)
WITH EXECUTE AS OWNER
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [Quotes](ConversationId, QuoteId, QuoteInfo) 
	VALUES(@ConversationId, @QuoteId, @QuoteInfo)
END
GO

GRANT EXECUTE ON usp_Add_Quote TO [ApplicationRole] 
GO


PRINT N'Creating Table [dbo].[ErrorLog]...';
GO

CREATE TABLE [dbo].[ErrorLog]
(
	[ConversationId] [nvarchar](MAX) NOT NULL,
	[UserId] [nvarchar](MAX) NOT NULL,
	[ConversationDate] [nvarchar](30) NOT NULL,
	[ConversationLog] [nvarchar](MAX) NULL,
	[Error] [nvarchar](MAX) NOT NULL
);
GO

PRINT N'Granting permissions on [dbo].[ErrorLog]...';
GO

GRANT SELECT, INSERT ON [dbo].[ErrorLog] TO [ApplicationRole] AS [dbo];
GO

PRINT N'Creating Procedure usp_Add_Error...';
GO

CREATE PROCEDURE usp_Add_Error
	@ConversationId nvarchar(MAX),
	@UserId nvarchar(MAX),
	@ConversationDate nvarchar(30),
	@ConversationLog nvarchar(MAX),
	@Error nvarchar(MAX)
WITH EXECUTE AS OWNER
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [Conversation](ConversationId, UserId, ConversationDate, ConversationLog, Error) 
	VALUES(@ConversationId, @UserId, @ConversationDate, @ConversationLog, @Error)
END
GO

GRANT EXECUTE ON usp_Add_Error TO [ApplicationRole] 
GO