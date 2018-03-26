using Microsoft.Bot.Builder.FormFlow;
using System;

namespace QuotingBot.Models
{
    [Serializable]
    public class QuickQuote
    {
        public static IForm<QuickQuote> BuildQuickQuoteForm()
        {
            return new FormBuilder<QuickQuote>()
                .Message("Let's get started \U0001F600")
                .Build();
        }
    }
}