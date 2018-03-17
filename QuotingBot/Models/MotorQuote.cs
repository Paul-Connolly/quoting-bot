using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuotingBot.Models
{
    [Serializable]
    public class MotorQuote
    {
        public string VehicleRegistration;
        public int? VehicleValue;
        public string AreaVehicleIsKept;
        public DateTime? EffectiveDate;

        public static IForm<MotorQuote> BuildQuickQuoteForm()
        {
            return new FormBuilder<MotorQuote>()
                .Message("No problem! Let's get started \U0001F600")
                .Field(nameof(VehicleRegistration))
                .Field(nameof(VehicleValue))
                .Field(nameof(AreaVehicleIsKept))
                .Build();
        }
    }
}