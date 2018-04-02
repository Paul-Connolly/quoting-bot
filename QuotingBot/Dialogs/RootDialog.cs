using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace QuotingBot.Dialogs
{
    [Serializable]
    public sealed class RootDialog : IDialog<object>
    {
        private const string MotorInsuranceOption = "Motor insurance \U0001F698";
        private const string HomeInsuranceOption = "Home insurance \U0001F3E1";

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hi, I'm Ava - your friendly quoting bot!");
            context.Wait(MessageReceivedAsync);
        }

        private void ShowQuoteOptions(IDialogContext context)
        {
            PromptDialog.Choice
            (
                context, 
                OnOptionSelected, 
                new List<string> { MotorInsuranceOption, HomeInsuranceOption }, 
                "What can I get you a quote for today?",
                "Hmmm...that's not a valid option.  Please choose an option from the list."
            );
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var optionSelected = await result;

                switch (optionSelected)
                {
                    case MotorInsuranceOption:
                        context.Call(new MotorDialog(), ResumeAfterOptionDialog);
                        break;
                    case HomeInsuranceOption:
                        context.Call(new HomeDialog(), ResumeAfterOptionDialog);
                        break;
                }
            }
            catch (Exception)
            {
                await context.PostAsync($"Oops! Something went wrong.");

                context.Wait(MessageReceivedAsync);
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
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            ShowQuoteOptions(context);
        }
    }
}