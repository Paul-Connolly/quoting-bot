namespace QuotingBot.DAL.Models
{
    public class Conversation
    {
        public string ConversationId { get; set; }
        public string UserId { get; set; }
        public string ConversationDate { get; set; }
        public string ConversationLog { get; set; }

        public Conversation(string conversationId, string userId, string conversationDate, string conversationLog)
        {
            ConversationId = conversationId;
            UserId = userId;
            ConversationDate = conversationDate;
            ConversationLog = conversationLog;
        }
    }
}
