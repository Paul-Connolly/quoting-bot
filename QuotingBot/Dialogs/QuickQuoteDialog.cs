using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using QuotingBot.Dialogs.Motor;
using QuotingBot.Models;
using QuotingBot.Models.Home;
using System;
using System.Threading.Tasks;

namespace QuotingBot.Dialogs
{
    [LuisModel("fdc7ea4d-1480-467f-978e-61d4a3345051", "0caa7c26506c48bb801e68daca06077c")]
    [Serializable]
    public class QuickQuoteDialog : LuisDialog<QuickQuote>
    {
        private readonly BuildFormDelegate<QuickQuote> GetQuickQuote;
        private readonly BuildFormDelegate<MotorQuote> GetMotorQuote;
        private readonly BuildFormDelegate<HomeQuote> GetHomeQuote;
        public QuickQuoteDialog(BuildFormDelegate<QuickQuote> getQuickQuote) => GetQuickQuote = getQuickQuote;

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

        [LuisIntent("MotorQuote")]
        public async Task MotorQuote(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            //var quotationForm = new FormDialog<MotorQuote>(new MotorQuote(), this.MotorQuote, FormOptions.PromptInStart);
            context.Forward(this.MotorQuote, new MotorDialog(Models.MotorQuote.BuildMotorQuickQuoteForm), message, Callback);
        }

        [LuisIntent("HomeQuote")]
        public async Task HomeQuote(IDialogContext context, LuisResult result)
        {
            //var quotationForm = new FormDialog<HomeQuote>(new HomeQuote(), this.GetHomeQuote, FormOptions.PromptInStart);
            context.Forward(new HomeDialog(GetHomeQuote), Callback);
        }

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }
    }
}