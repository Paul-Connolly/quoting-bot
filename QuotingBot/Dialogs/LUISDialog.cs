using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using QuotingBot.Models;
using System;

namespace QuotingBot.Dialogs
{
    [LuisModel("fdc7ea4d-1480-467f-978e-61d4a3345051", "0caa7c26506c48bb801e68daca06077c")]
    [Serializable]
    public class LUISDialog : LuisDialog<QuoteType>
    {
    }
}