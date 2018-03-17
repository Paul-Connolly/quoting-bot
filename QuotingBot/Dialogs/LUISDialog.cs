using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using QuotingBot.Models;
using System;
using System.Threading.Tasks;

namespace QuotingBot.Dialogs
{
    [LuisModel("fdc7ea4d-1480-467f-978e-61d4a3345051", "0caa7c26506c48bb801e68daca06077c")]
    [Serializable]
    public class LUISDialog : LuisDialog<MotorQuote>
    {
        private readonly BuildFormDelegate<MotorQuote> GetQuote;

        public LUISDialog(BuildFormDelegate<MotorQuote> getQuote) => GetQuote = getQuote;

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry, I don't know what you mean.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            context.Call(new RootDialog(), Callback);
        }

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }

        [LuisIntent("MotorQuote")]
        public async Task QuoteType(IDialogContext context, LuisResult result)
        {
            var quotationForm = new FormDialog<MotorQuote>(new MotorQuote(), this.GetQuote, FormOptions.PromptInStart);
            context.Call<MotorQuote>(quotationForm, Callback);
        }
    }
}