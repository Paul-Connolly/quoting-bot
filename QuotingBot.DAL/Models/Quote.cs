namespace QuotingBot.Models
{
    public class Quote
    {
        public string ConversationId { get; set; }
        public string QuoteId { get; set; }
        public string QuoteInfo { get; set; }
        public Quote(string conversationId, string quoteId, string quoteInfo)
        {
            ConversationId = conversationId;
            QuoteId = quoteId;
            QuoteInfo = quoteInfo;
        }
    }
}