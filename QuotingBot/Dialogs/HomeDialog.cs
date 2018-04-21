using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using QuotingBot.Common.Email;
using QuotingBot.DAL.Quotes;
using QuotingBot.DAL.Repository.Conversations;
using QuotingBot.DAL.Repository.Errors;
using QuotingBot.Enums;
using QuotingBot.Helpers;
using QuotingBot.Models.Home;
using QuotingBot.Common.RelayHouseholdService;

namespace QuotingBot.Dialogs
{
    [Serializable]
    public class HomeDialog : IDialog<object>
    {
        private readonly Validation _validation = new Validation();
        private static readonly bool SendEmails = Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmails"]);
        private static readonly string Connection = ConfigurationManager.ConnectionStrings["QuotingBot"].ConnectionString;
        private static readonly ErrorRepository ErrorRepository = new ErrorRepository(Connection);

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"No worries - let's do it {Emoji.GrinningFace}");

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
                    validate: async (state, value) => _validation.ValidateTown(value))
                .Field(nameof(HomeQuote.County),
                    validate: async (state, value) => _validation.ValidateCounty(value))
                .Field(nameof(HomeQuote.FirstName),
                    validate: async (state, value) => _validation.ValidateFirstName(value))
                .Field(nameof(HomeQuote.LastName),
                    validate: async (state, value) => _validation.ValidateLastName(value))
                .AddRemainingFields()
                .Field(nameof(HomeQuote.EmailAddress),
                    validate: async (state, value) => _validation.ValidateEmailAddress(value))
                .AddRemainingFields()
                .Field(nameof(HomeQuote.YearBuilt),
                    validate: async (state, value) => _validation.ValidateYearBuilt(value))
                .Field(nameof(HomeQuote.NumberOfBedrooms),
                    prompt: "How many bedrooms are in the property? (0-9 bedrooms)",
                    validate: async (state, value) => _validation.ValidateNumberOfBedrooms(value))
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
                var quoteRepository = new QuoteRepository(Connection);
                var conversationRepository = new ConversationRepository(Connection);
                var reply = context.MakeMessage();

                var homeService = new Household();
                var homeWebServiceRequest = HomeQuote.BuildHomeWebServiceRequest(state);

                var quotes = new List<HomeQuoteWebServiceResult>();
                
                var response = homeService.GetQuotes(homeWebServiceRequest);

                if (response.Quotes != null)
                {
                    foreach (var quote in response.Quotes)
                    {
                        quotes.Add(quote);
                    }

                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    reply.Attachments = GetQuoteReceipts(quotes);

                    quoteRepository.StoreQuote
                    (
                        context.Activity.Conversation.Id,
                        response.Quotes[0].RelayQuoteId,
                        new JavaScriptSerializer().Serialize(quotes)
                    );
                }

                await context.PostAsync(reply);

                if (SendEmails)
                {
                    EmailHandler.SendEmail(state.EmailAddress, $"{state.FirstName} {state.LastName}", "");
                }

                conversationRepository.StoreConversation
                (
                    context.Activity.Conversation.Id,
                    context.Activity.From.Id,
                    DateTime.Now.ToString(new CultureInfo("en-GB")),
                    new JavaScriptSerializer().Serialize(context)
                );
            }
            catch (Exception exception)
            {
                var errorRepository = new ErrorRepository(Connection);
                errorRepository.LogError(context.Activity.Conversation.Id, context.Activity.From.Id, DateTime.Now.ToString(), context.ConversationData.ToString(), exception.InnerException.ToString());
                throw;
            }
            finally
            {
                context.Done(state);
            }
        }

        private static IList<Attachment> GetQuoteReceipts(List<HomeQuoteWebServiceResult> homeQuoteWebServiceResults)
        {
            var cards = new List<Attachment>();

            foreach (var result in homeQuoteWebServiceResults)
            {
                if (result.NetPremium > 0)
                {
                    cards.Add(GetReceiptCard(result));
                }
            }

            return cards;
        }

        private static Attachment GetReceiptCard(HomeQuoteWebServiceResult homeQuoteWebServiceResult)
        {
            try
            {
                var receiptCard = new ReceiptCard
                {
                    Title = $"{homeQuoteWebServiceResult.InsurerName}",
                    Facts = new List<Fact> { new Fact("Scheme", homeQuoteWebServiceResult.SchemeName) },
                    Tax = $"€{homeQuoteWebServiceResult.GovernmentLevyPremium}",
                    Total = $"€{homeQuoteWebServiceResult.NetPremium}",
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        (
                            ActionTypes.PostBack,
                            "Request a Callback"
                        )
                    }
                };

                return receiptCard.ToAttachment();
            }
            catch (Exception exception)
            {
                ErrorRepository.LogError(DateTime.Now.ToString(new CultureInfo("en-GB")), exception.ToString());
                throw;
            }
        }
    }
}