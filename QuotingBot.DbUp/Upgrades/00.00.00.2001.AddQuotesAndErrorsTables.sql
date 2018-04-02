PRINT N'Creating Table [dbo].[Quotes]...';
GO

CREATE TABLE [dbo].[Quotes]
(
	[ConversationId] [uniqueidentifier] NOT NULL,
	[QuoteId] [varchar](12) NOT NULL,
	[Quote] [nvarchar](MAX) NULL
);
GO

PRINT N'Granting permissions on [dbo].[Quotes]...';
GO

GRANT SELECT, INSERT ON [dbo].[Quotes] TO [ApplicationRole] AS [dbo];
GO

PRINT N'Creating Procedure usp_Add_Quote...';
GO

CREATE PROCEDURE usp_Add_Quote
	@ConversationId uniqueidentifier,
	@QuoteId uniqueidentifier,
	@Quote varchar(MAX)
WITH EXECUTE AS OWNER
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [Quotes](ConversationId, QuoteId, Quote) 
	VALUES(@ConversationId, @QuoteId, @Quote)
END
GO

GRANT EXECUTE ON usp_Add_Quote TO [ApplicationRole] 
GO


PRINT N'Creating Table [dbo].[ErrorLog]...';
GO

CREATE TABLE [dbo].[ErrorLog]
(
	[ConversationId] [uniqueidentifier] NOT NULL,
	[Error] [nvarchar](MAX) NULL
);
GO

PRINT N'Granting permissions on [dbo].[ErrorLog]...';
GO

GRANT SELECT, INSERT ON [dbo].[ErrorLog] TO [ApplicationRole] AS [dbo];
GO

PRINT N'Creating Procedure usp_Add_Error...';
GO

CREATE PROCEDURE usp_Add_Error
	@ConversationId uniqueidentifier,
	@UserId uniqueidentifier,
	@ConversationDate varchar(30),
	@QuoteId varchar(12),
	@ConversationLog varchar(MAX)
WITH EXECUTE AS OWNER
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [Conversation](ConversationId, UserId, ConversationDate, QuoteId, ConversationLog) 
	VALUES(@ConversationId, @UserId, @ConversationDate, @QuoteId, @ConversationLog)
END
GO

GRANT EXECUTE ON usp_Add_Error TO [ApplicationRole] 
GO

--PRINT N'Creating Table [dbo].[RiskVariationPremiumAdjustment]...';
--GO

--CREATE TABLE [dbo].[RiskVariationPremiumAdjustment](
--	[RiskVariationId] [varchar](50) NOT NULL,
--	[Adjustment] [decimal](18,2) NOT NULL,
--	CONSTRAINT [PK_RiskVariationPremiumAdjustment] PRIMARY KEY CLUSTERED ([RiskVariationId] ASC)
--);
--GO

--PRINT N'Granting permissions on [dbo].[RiskVariationPremiumAdjustment]...';
--GO

--GRANT SELECT, INSERT ON [dbo].[RiskVariationPremiumAdjustment] TO [ApplicationRole] AS [dbo];
--GO

--PRINT N'Creating Procedure usp_Get_PremiumAdjustment...';
--GO

--CREATE PROCEDURE usp_Get_PremiumAdjustment
--	@RiskVariationId varchar(50)
--WITH EXECUTE AS OWNER
--AS
--BEGIN
--	SET NOCOUNT ON;

--	SELECT Adjustment
--	FROM [dbo].[RiskVariationPremiumAdjustment]
--	WHERE RiskVariationId = @RiskVariationId
--END
--GO

--GRANT EXECUTE ON usp_Get_PremiumAdjustment TO [ApplicationRole] 
--GO

--PRINT N'Creating Procedure usp_Upsert_PremiumAdjustment...';
--GO

--CREATE PROCEDURE usp_Upsert_PremiumAdjustment
--	@RiskVariationId varchar(50),
--	@Adjustment decimal(18,2)
--WITH EXECUTE AS OWNER
--AS
--BEGIN
--	SET NOCOUNT ON;

--	IF EXISTS (SELECT 1 FROM [dbo].[RiskVariationPremiumAdjustment] WHERE RiskVariationId = @RiskVariationId)
--		UPDATE [RiskVariationPremiumAdjustment]
--		SET Adjustment = @Adjustment
--		WHERE RiskVariationId = @RiskVariationId
--	ELSE
--		INSERT INTO [RiskVariationPremiumAdjustment](RiskVariationId, Adjustment) 
--		VALUES(@riskVariationId, @Adjustment)
--END
--GO

--GRANT EXECUTE ON usp_Upsert_PremiumAdjustment TO [ApplicationRole] 
--GO

--PRINT N'Creating Procedure [UnitTests].[ClearRiskVariationPremiumAdjustmentTableData]...';
--GO

--CREATE PROCEDURE [UnitTests].[ClearRiskVariationPremiumAdjustmentTableData]
--WITH EXECUTE AS OWNER
--AS
--BEGIN
--	TRUNCATE TABLE [dbo].[RiskVariationPremiumAdjustment];
--END
--GO

--PRINT N'Creating Procedure [UnitTests].[ClearAllQuotingTablesData]...';
--GO

--CREATE PROCEDURE [UnitTests].[ClearAllQuotingTablesData]
--WITH EXECUTE AS OWNER
--AS
--BEGIN
--	TRUNCATE TABLE [dbo].[QuoteRate];
--	TRUNCATE TABLE [dbo].[RiskVariationPremiumAdjustment];
--END
--GO