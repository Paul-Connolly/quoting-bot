using System;

namespace QuotingBot.DAL.Models
{
    public class Error
    {
        public string ConversationId { get; set; }
        public string UserId { get; set; }
        public string ConversationDate { get; set; }
        public string ConversationLog { get; set; }
        public string ErrorMessage { get; set; }

        public Error(string conversationId, string userId, string conversationDate, string conversationLog, string errorMessage)
        {
            ConversationId = conversationId;
            UserId = userId;
            ConversationDate = conversationDate;
            ConversationLog = conversationLog;
            ErrorMessage = errorMessage;
        }
    }
}
