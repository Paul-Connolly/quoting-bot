ALTER TABLE ErrorLog
DROP COLUMN ConversationId, UserId;

ALTER TABLE ErrorLog
ADD ConversationId nvarchar(MAX) NULL,
UserId nvarchar(MAX) NULL;