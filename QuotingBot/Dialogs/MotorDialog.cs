﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Threading.Tasks;
using QuotingBot.Models.Motor;

namespace QuotingBot.Dialogs
{
    [Serializable]
    public class MotorDialog : IDialog<MotorQuote>
    {
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
                        var result = new ValidateResult
                        {
                            IsValid = true,
                            Value = value.ToString().ToUpper(),
                            Feedback = MotorQuote.GetVehicle(value.ToString()).Description
                        };
                        return result;
                    }
                )
                .Confirm(generateMessage: async (state) => new PromptAttribute("Is this your car?"))
                .AddRemainingFields()
                .Confirm("Do you want to request a quote using the following details?" +
                         "Car Registration: {VehicleRegistration}")
                .OnCompletion(getMotorQuotes)
                .Build();
        }

        private async Task ResumeAfterMotorQuoteFormDialog(IDialogContext context, IAwaitable<MotorQuote> result)
        {
            var state = await result;

            try
            {
                var motorService = new RelayFullCycleMotorService.RelayFullCycleMotorService();
                
                var riskData = MotorQuote.BuildIrishMQRiskInfo(state);
                var messageRequestInfo = MotorQuote.BuildMessageRequestInfo();
                
                var quotes = motorService.GetNewBusinessXBreakDownsSpecified(riskData, 5, true, null, messageRequestInfo);

                await context.PostAsync($"Here are your quotes...€{quotes.Quotations[0].Premium.TotalPremium}");
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