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
using System.Globalization;
using QuotingBot.Helpers;
using QuotingBot.Common.Email;
using QuotingBot.DAL.Repository.Conversations;

namespace QuotingBot.Dialogs
{
    [Serializable]
    public class MotorDialog : IDialog<MotorQuote>
    {
        private Validation validation = new Validation();
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("No problem!");
            await context.PostAsync("Let's get started \U0001F604");

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
                            result.Feedback = "Hmmm...I couldn't find a match for that registration \U0001F914 Please try again";
                        }
                        return result;
                    }
                )
                .Confirm(generateMessage: async (state) => new PromptAttribute("Is this your car?"))
                .Field(nameof(MotorQuote.VehicleValue),
                    validate: async (state, value) => validation.ValidateVehicleValue(value))
                .Field(nameof(MotorQuote.AreaVehicleIsKept),
                    validate: async (state, value) => validation.ValidateAreaVehicleIsKept(value))
                .Field(nameof(MotorQuote.FirstName),
                    validate: async (state, value) => validation.ValidateFirstName(value))
                .Field(nameof(MotorQuote.LastName),
                    validate: async (state, value) => validation.ValidateLastName(value))
                .Field(nameof(MotorQuote.DateOfBirth),
                    prompt: "What is your date of birth? Enter date in DD/MM/YYYY format please",
                    validate: async (state, value) => validation.ValidateDateOfBirth(value))
                .AddRemainingFields()
                .Field(nameof(MotorQuote.EmailAddress),
                    validate: async (state, value) => validation.ValidateEmailAddress(value))
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
                var connection = System.Configuration.ConfigurationManager.ConnectionStrings["QuotingBot"].ConnectionString;
                var quoteRepository = new QuoteRepository(connection);
                var conversationRepository = new ConversationRepository(connection);
                var motorService = new RelayFullCycleMotorService.RelayFullCycleMotorService();
                
                var riskData = MotorQuote.BuildIrishMQRiskInfo(state);
                var messageRequestInfo = MotorQuote.BuildMessageRequestInfo();
                
                var quotes = motorService.GetNewBusinessXBreakDownsSpecified(riskData, 100, true, null, messageRequestInfo);
                
                quoteRepository.StoreQuote
                    (
                        context.Activity.Conversation.Id, 
                        quotes.TransactionID, 
                        new JavaScriptSerializer().Serialize(quotes.Quotations[0])
                    );

                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                reply.Attachments = GetQuoteThumbnails(quotes.Quotations);

                EmailHandler.SendEmail(state.EmailAddress, $"{state.FirstName} {state.LastName}", "");
                await context.PostAsync(reply);

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
                var errorRepository = new ErrorRepository();
                errorRepository.LogError(context.Activity.Conversation.Id, context.Activity.From.Id, DateTime.Now.ToString(), context.ConversationData.ToString(), exception.ToString());
                throw;
            }
            finally
            {
                context.Done(state);
            }
        }

        private IList<Attachment> GetQuoteThumbnails(RelayFullCycleMotorService.IrishMQResultsBreakdown[] breakdowns)
        {
            var cards = new List<Attachment>();

            foreach(var breakdown in breakdowns)
            {
                cards.Add(
                    GetThumbnail(
                        breakdown.Premium.SchemeName,
                        breakdown.Premium.TotalPremium.ToString(),
                        null,
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
                Subtitle = '$' + subtitle
            };

            return thumbnail.ToAttachment();
        }
    }
}