using Microsoft.Bot.Builder.FormFlow;
using System;
using QuotingBot.RelayFullCycleMotorService;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace QuotingBot.Models
{
    public enum LicenceTypes
    {
        FullIrish,
        ProvisionalIrish,
        FullEU,
        FullUK,
        Foreign,
        InternationalLicence,
        LearnerPermit
        // value expected in XML request
        // <option value = "C" > Full Irish</option>
        //<option value = "B" > Provisional Irish</option>
        // <option value = "F" > Full EU</option>
        // <option value = "D" > Full UK</option>
        // <option value = "I" > Foreign </ option >
        // <option value = "N" > International Licence</option>
        // <option value = "G" > Learner Permit</option>

    }
    public enum NoClaimsDiscount
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        MoreThanNine
    }

    [Serializable]
    public class MotorQuote
    {
        public static RelayFullCycleMotorService.RelayFullCycleMotorService motorService = new RelayFullCycleMotorService.RelayFullCycleMotorService();
        public string VehicleRegistration;
        public int? VehicleValue;
        //public string AreaVehicleIsKept;
        //public DateTime? EffectiveDate;
        public string FirstName;
        //public string LastName;
        //public DateTime? DateOfBirth;
        //public LicenceTypes? LicenceType;
        //public NoClaimsDiscount? NoClaimsDiscount;
        //public string PrimaryContactNumber;
        //public string EmailAddress;

        public static IForm<MotorQuote> BuildQuickQuoteForm()
        {
            OnCompletionAsyncDelegate<MotorQuote> getQuote = async (context, state) =>
            {
                var quotes = motorService.GetQuickQuotesSpecified(
                    RiskData: BuildIrishMQRiskInfo(state), 
                    EnableValidation: true, 
                    TimeTravelDate: null, 
                    MessageRequestInfo: null);
                await context.PostAsync("Getting your quotes!");
            };

            return new FormBuilder<MotorQuote>()
                .Message("No problem!")
                .Message("Let's get started \U0001F600")
                .Field(nameof(VehicleRegistration),
                    validate: async (state, value) =>
                    {
                        var result = new ValidateResult { IsValid = true };
                        result.Value = value.ToString().ToUpper();
                        result.Feedback = GetVehicle(value.ToString()).Description;
                        return result;
                    }
                )
                .Confirm(async (state) =>
                    {
                        return new PromptAttribute("Is this your car?");
                    }
                )
                .AddRemainingFields()
                .Confirm("Would you to request a quote using the following details?" +
                    "Car Registration: {VehicleRegistration}")
                .Message("Going to get your quotes...")
                .OnCompletion(getQuote)
                .Build();
        }        

        public static Vehicle GetVehicle(string vehicleRegistration)
        {
            var result = new ValidateResult();
            var vehicle = new Vehicle();

            vehicle.ABICode = motorService.GetVehicleLookup(
                vehicleRegistration,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                "RE0098",
                "relay1:0099",
                VehicleLookup.Motor).ABICode;

            vehicle = GetVehicleDetails(vehicle.ABICode);

            return vehicle = GetVehicleDetails(vehicle.ABICode);
        }

        private static Vehicle GetVehicleDetails(string ABICode)
        {
            var vehicle = new Vehicle();
            var vehicleLookupItem = motorService.GetVehicleDetailsABI(ABICode);
            vehicle.ABICode = ABICode;
            vehicle.Description = vehicleLookupItem.Description;
            vehicle.Manufacturer = vehicleLookupItem.Manufacturer;
            vehicle.Model = vehicleLookupItem.Model;
            vehicle.BodyType = vehicleLookupItem.BodyType;
            vehicle.EngineCapacity = vehicleLookupItem.EngineCapacity;
            vehicle.NumberOfDoors = vehicleLookupItem.NumberDoors;
            vehicle.FuelType = vehicleLookupItem.FuelType;
            vehicle.YearOfFirstManufacture = vehicleLookupItem.YearOfFirstManufacture;

            return vehicle;
        }

        private static IrishMQRiskInfo BuildIrishMQRiskInfo(MotorQuote state)
        {
            var riskInfo = new IrishMQRiskInfo();
            riskInfo.Driver[0].Forename = state.FirstName;

            return riskInfo;
        }
    }
}