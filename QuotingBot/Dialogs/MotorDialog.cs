using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using QuotingBot.Models;
using QuotingBot.Models.Home;
using System;
using System.Threading.Tasks;

namespace QuotingBot.Dialogs
{
    [Serializable]
    public class MotorDialog : IDialog<MotorQuote>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var quotationForm = new FormDialog<MotorQuote>(new MotorQuote(), this.GetMotorQuote, FormOptions.PromptInStart);
            context.Call(quotationForm, Callback);
        }
        private readonly BuildFormDelegate<MotorQuote> GetMotorQuote;

        public MotorDialog(BuildFormDelegate<MotorQuote> getMotorQuote) => GetMotorQuote = getMotorQuote;

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Done(context);
        }

        
    }
}