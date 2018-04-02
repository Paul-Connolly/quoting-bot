using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using QuotingBot.Models.Home;
using QuotingBot.RelayHouseholdService;

namespace QuotingBot.Dialogs
{
    [Serializable]
    public class HomeDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("No worries - let's do it \U0001F604");

            var homeQuoteFormDialog = FormDialog.FromForm(this.BuildHomeQuoteForm, FormOptions.PromptInStart);
            context.Call(homeQuoteFormDialog, ResumeAfterHomeQuoteFormDialog);
        }

        private IForm<HomeQuote> BuildHomeQuoteForm()
        {
            OnCompletionAsyncDelegate<HomeQuote> getHomeQuotes = async (context, state) =>
            {
                await context.PostAsync("Getting your quotes...");
            };

            return new FormBuilder<HomeQuote>()
                .Field(nameof(HomeQuote.FirstLineOfAddress))
                .Field(nameof(HomeQuote.Town))
                .Field(nameof(HomeQuote.County))
                .AddRemainingFields()
                .Confirm("Do you want to request a quote using the following details?" +
                         "Address: {FirstLineOfAddress}, {Town}, {County}")
                .OnCompletion(getHomeQuotes)
                .Build();
        }

        private static async Task ResumeAfterHomeQuoteFormDialog(IDialogContext context, IAwaitable<HomeQuote> result)
        {
            var state = await result;

            try
            {
                var homeService = new Household();
                var homeWebServiceRequest = HomeQuote.BuildHomeWebServiceRequest(state);

                var response = homeService.GetQuotes(homeWebServiceRequest);
                var quotes = response.Quotes;

                await context.PostAsync($"Here are your quotes...€{quotes[0].HousePremium}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                context.Done(state);
            }
        }
    }
}