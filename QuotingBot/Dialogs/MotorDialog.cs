using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Threading.Tasks;
using QuotingBot.Models.Motor;
using System.Web.Script.Serialization;
using QuotingBot.DAL.Quotes;
using QuotingBot.DAL.Repository.Errors;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using QuotingBot.Helpers;
using QuotingBot.Common.Email;
using QuotingBot.DAL.Repository.Conversations;
using QuotingBot.Enums;
using QuotingBot.RelayFullCycleMotorService;

namespace QuotingBot.Dialogs
{
    [Serializable]
    public class MotorDialog : IDialog<MotorQuote>
    {
        private readonly Validation _validation = new Validation();
        private static readonly bool SendEmails = Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmails"]);
        private static readonly string Connection = ConfigurationManager.ConnectionStrings["QuotingBot"].ConnectionString;
        private static readonly ErrorRepository ErrorRepository = new ErrorRepository(Connection);

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("No problem!");
            await context.PostAsync($"Let's get started {Emoji.GrinningFace}");

            var motorQuoteFormDialog = FormDialog.FromForm(this.BuildMotorQuoteForm, FormOptions.PromptInStart);
            context.Call(motorQuoteFormDialog, this.ResumeAfterMotorQuoteFormDialog);
        }

        private IForm<MotorQuote> BuildMotorQuoteForm()
        {
            OnCompletionAsyncDelegate<MotorQuote> getMotorQuotes = async (context, state) =>
            {
                await context.PostAsync("Getting your quotes...");
            };

            return new FormBuilder<MotorQuote>()
                .Field(nameof(MotorQuote.VehicleRegistration),
                    validate: async (state, value) =>
                    {
                        var result = new ValidateResult();
                        state.Vehicle = MotorQuote.GetVehicle(value.ToString());

                        if (!string.IsNullOrEmpty(state.Vehicle.Description))
                        {
                            result.IsValid = true;
                            result.Value = value.ToString().ToUpper();
                            result.Feedback = state.Vehicle.Description;
                        }
                        else
                        {
                            result.IsValid = false;
                            result.Feedback = $"Hmmm...I couldn't find a match for that registration {Emoji.ThinkingFace} Please try again";
                        }
                        return result;
                    }
                )
                .Confirm(generateMessage: async (state) => new PromptAttribute("Is this your car?"))
                .Field(nameof(MotorQuote.VehicleValue),
                    validate: async (state, value) => _validation.ValidateVehicleValue(value))
                .Field(nameof(MotorQuote.AreaVehicleIsKept),
                    validate: async (state, value) => _validation.ValidateAreaVehicleIsKept(value))
                .Field(nameof(MotorQuote.FirstName),
                    validate: async (state, value) => _validation.ValidateFirstName(value))
                .Field(nameof(MotorQuote.LastName),
                    validate: async (state, value) => _validation.ValidateLastName(value))
                .Field(nameof(MotorQuote.DateOfBirth),
                    prompt: "What is your date of birth? Enter date in DD/MM/YYYY format please",
                    validate: async (state, value) => _validation.ValidateDateOfBirth(value))
                .AddRemainingFields()
                .Field(nameof(MotorQuote.EmailAddress),
                    validate: async (state, value) => _validation.ValidateEmailAddress(value))
                .Confirm("Do you want to request a quote using the following details?" +
                         "Car Registration: {VehicleRegistration}")
                .OnCompletion(getMotorQuotes)
                .Build();
        }

        private async Task ResumeAfterMotorQuoteFormDialog(IDialogContext context, IAwaitable<MotorQuote> result)
        {
            var state = await result;
            var reply = context.MakeMessage();

            try
            {
                var quoteRepository = new QuoteRepository(Connection);
                var conversationRepository = new ConversationRepository(Connection);
                var motorService = new RelayFullCycleMotorService.RelayFullCycleMotorService();
                
                var riskData = MotorQuote.BuildIrishMQRiskInfo(state);
                var messageRequestInfo = MotorQuote.BuildMessageRequestInfo();
                
                var quotes = motorService.GetNewBusinessXBreakDownsSpecified(riskData, 100, true, null, messageRequestInfo);

                if (quotes.Quotations != null)
                {
                    quoteRepository.StoreQuote
                    (
                        context.Activity.Conversation.Id,
                        quotes.TransactionID,
                        new JavaScriptSerializer().Serialize(quotes.Quotations[0])
                    );

                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    reply.Attachments = GetQuoteReceipts(quotes.Quotations);

                    if (SendEmails)
                    {
                        EmailHandler.SendEmail(state.EmailAddress, $"{state.FirstName} {state.LastName}", "");
                    }

                    await context.PostAsync(reply);
                }
                else
                {
                    await context.PostAsync("Sorry, we were unable to get your a quote at this point.");
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
                ErrorRepository.LogError(context.Activity.Conversation.Id, context.Activity.From.Id, DateTime.Now.ToString(new CultureInfo("en-GB")), context.ConversationData.ToString(), exception.ToString());
                throw;
            }
            finally
            {
                context.Done(state);
            }
        }

        private IList<Attachment> GetQuoteReceipts(IrishMQResultsBreakdown[] breakdowns)
        {
            return breakdowns.Select(breakdown => GetReceiptCard(breakdown)).ToList();
        }

        private static Attachment GetReceiptCard(IrishMQResultsBreakdown breakdown)
        {
            try
            {
                var receiptCard = new ReceiptCard
                {
                    Title = $"Insurer: {breakdown.Premium.SchemeName}",
                    Facts = new List<Fact> { new Fact("Scheme", breakdown.Premium.Scheme.SchemeNumber) },
                    Tax = $"€{breakdown.Premium.PremiumAfterLevy - breakdown.Premium.PremiumBeforeLevy}",
                    Total = $"€{breakdown.Premium.TotalPremium}",
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