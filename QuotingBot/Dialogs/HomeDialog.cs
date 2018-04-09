using System;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using QuotingBot.DAL.Quotes;
using QuotingBot.DAL.Repository.Conversations;
using QuotingBot.DAL.Repository.Errors;
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
                var quoteRepository = new QuoteRepository();
                var conversationRepository = new ConversationRepository();
                var homeService = new Household();
                var homeWebServiceRequest = HomeQuote.BuildHomeWebServiceRequest(state);

                var response = homeService.GetQuotes(homeWebServiceRequest);
                var quotes = response.Quotes;

                quoteRepository.StoreQuote
                    (
                        context.Activity.Conversation.Id,
                        response.Quotes[0].RelayQuoteId,
                        new JavaScriptSerializer().Serialize(quotes)
                    );

                await context.PostAsync($"Here are your quotes...€{quotes[0].HousePremium}");

                conversationRepository.StoreConversation
                    (
                        context.Activity.Conversation.Id,
                        context.Activity.From.Id,
                        DateTime.Now.ToString(),
                        new JavaScriptSerializer().Serialize(context.ConversationData)
                    );
            }
            catch (Exception exception)
            {
                var errorRepository = new ErrorRepository();
                errorRepository.LogError(context.Activity.Conversation.Id, context.Activity.From.Id, DateTime.Now.ToString(), context.ConversationData.ToString(), exception.InnerException.ToString());
                throw;
            }
            finally
            {
                context.Done(state);
            }
        }
    }
}