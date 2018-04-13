using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using QuotingBot.DAL.Quotes;
using QuotingBot.DAL.Repository.Conversations;
using QuotingBot.DAL.Repository.Errors;
using QuotingBot.Helpers;
using QuotingBot.Models.Home;
using QuotingBot.RelayHouseholdService;

namespace QuotingBot.Dialogs
{
    [Serializable]
    public class HomeDialog : IDialog<object>
    {
        private Validation validation = new Validation();
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
                .Field(nameof(HomeQuote.Town),
                    validate: async (state, value) =>
                    {
                        return validation.ValidateTown(value);
                    }
                )
                .Field(nameof(HomeQuote.County),
                    validate: async (state, value) =>
                    {
                        return validation.ValidateCounty(value);
                    }
                )
                .Field(nameof(HomeQuote.FirstName),
                    validate: async (state, value) =>
                    {
                        return validation.ValidateFirstName(value);
                    }
                )
                .Field(nameof(HomeQuote.LastName),
                    validate: async (state, value) =>
                    {
                        return validation.ValidateLastName(value);
                    }
                )
                .AddRemainingFields()
                .Field(nameof(HomeQuote.EmailAddress),
                    validate: async (state, value) =>
                    {
                        return validation.ValidateEmailAddress(value);
                    }
                )
                .AddRemainingFields()
                .Field(nameof(HomeQuote.YearBuilt),
                    validate: async (state, value) =>
                    {
                        return validation.ValidateYearBuilt(value);
                    }
                )
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

                quoteRepository.StoreQuote
                    (
                        context.Activity.Conversation.Id,
                        response.Quotes[0].RelayQuoteId,
                        new JavaScriptSerializer().Serialize(quotes)
                    );

                await context.PostAsync(reply);

                conversationRepository.StoreConversation
                    (
                        context.Activity.Conversation.Id,
                        context.Activity.From.Id,
                        DateTime.Now.ToString(),
                        new JavaScriptSerializer().Serialize(context)
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