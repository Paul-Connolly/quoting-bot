using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using QuotingBot.Models;
using System;
using System.Threading.Tasks;

namespace QuotingBot.Dialogs.Motor
{
    [LuisModel("fdc7ea4d-1480-467f-978e-61d4a3345051", "0caa7c26506c48bb801e68daca06077c")]
    [Serializable]
    public class MotorDialog : LuisDialog<MotorQuote>
    {
        private readonly BuildFormDelegate<MotorQuote> GetMotorQuote;
        public MotorDialog(BuildFormDelegate<MotorQuote> getMotorQuote) => GetMotorQuote = getMotorQuote;

        [LuisIntent("MotorQuote")]
        public async Task MotorQuote(IDialogContext context, LuisResult result)
        {
            var quotationForm = new FormDialog<MotorQuote>(new MotorQuote(), this.GetMotorQuote, FormOptions.PromptInStart);
            context.Call<MotorQuote>(quotationForm, Callback);
        }

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }
    }
}