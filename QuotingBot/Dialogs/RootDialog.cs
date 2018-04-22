using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using QuotingBot.Enums;

namespace QuotingBot.Dialogs
{
    [Serializable]
    public sealed class RootDialog : IDialog<object>
    {
        private static readonly string MotorInsuranceOption = $"Motor insurance {Emoji.Car}";
        private static readonly string HomeInsuranceOption = $"Home insurance {Emoji.House}";
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hi, I'm Ava - your friendly quoting bot!");
            context.Wait(MessageReceivedAsync);
        }

        public static void ShowQuoteOptions(IDialogContext context)
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

        public static async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var optionSelected = await result;

                if (optionSelected == MotorInsuranceOption)
                {
                    context.Call(new MotorDialog(), ResumeAfterOptionDialog);
                }
                else
                { 
                    context.Call(new HomeDialog(), ResumeAfterOptionDialog);
                }
            }
            catch (Exception)
            {
                await context.PostAsync($"Oops! Something went wrong.");
            }
        }

        private static async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
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

        public static async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            ShowQuoteOptions(context);
        }
    }
}