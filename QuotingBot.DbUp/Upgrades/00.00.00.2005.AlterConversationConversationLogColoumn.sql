ALTER TABLE Conversations
DROP COLUMN ConversationLog;

ALTER TABLE Conversations
ADD ConversationLog nvarchar(MAX);