using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuotingBot.Models
{
    public enum QuoteTypeOptions
    {
        Motor,
        Home
    }

    public enum CarType
    {
        Hatchback,
        Saloon,
        Estate,
        Jeep
    }

    [Serializable]
    public class QuoteType
    {
        public QuoteTypeOptions? Quote;
        public int? NumberOfDrivers;
        public int? CarValue;
        public DateTime? EffectiveDate;
        public List<CarType> Car;

        public static IForm<QuoteType> BuildForm()
        {
            return new FormBuilder<QuoteType>()
                .Message("Welcome!")
                .Build();
        }
    }
}