using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
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
                var reply = context.MakeMessage();

                var homeService = new Household();
                var homeWebServiceRequest = HomeQuote.BuildHomeWebServiceRequest(state);

                var quotes = new List<HomeQuoteWebServiceResult>();
                
                var response = homeService.GetQuotes(homeWebServiceRequest);
                foreach(var quote in response.Quotes)
                {
                    quotes.Add(quote);
                }

                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                reply.Attachments = GetQuoteThumbnails(quotes);

                await context.PostAsync(reply);
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

        private static IList<Attachment> GetQuoteThumbnails(List<HomeQuoteWebServiceResult> results)
        {
            var cards = new List<Attachment>();

            foreach (var result in results)
            {
                cards.Add(
                    GetThumbnail(
                        result.InsurerName,
                        result.SchemeName,
                        result.NetPremium.ToString(),
                        null,
                        null
                    )
                );
            }

            return cards;
        }

        private static Attachment GetThumbnail(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var thumbnail = new ThumbnailCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = '$' + text
            };

            return thumbnail.ToAttachment();
        }
    }
}