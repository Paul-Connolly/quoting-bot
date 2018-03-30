using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using QuotingBot.Models;

namespace QuotingBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string MotorInsuranceOption = "Motor insurance";
        private const string HomeInsuranceOption = "Home insurance";

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hi, I'm Ava - your friendly quoting bot!");
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task Respond(IDialogContext context)
        {
            await context.PostAsync("What can I quote you for today?");
            ShowQuoteOptions(context);
        }

        private void ShowQuoteOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { MotorInsuranceOption, HomeInsuranceOption }, "What can I quote you for today?", "Hmmm...that's not a valid option.  Please choose an option from the list.", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var optionSelected = await result;

                switch (optionSelected)
                {
                    case MotorInsuranceOption:
                        context.Call(new FormDialog<MotorQuote>(new MotorQuote(), MotorQuote.BuildQuickQuoteForm, FormOptions.PromptInStart), ResumeAfterOptionDialog);
                        break;
                    case HomeInsuranceOption:
                        context.Call(new HomeDialog(), ResumeAfterOptionDialog);
                        break;
                }
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Ooops! Something went wrong.");

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            ShowQuoteOptions(context);
        }
    }
}