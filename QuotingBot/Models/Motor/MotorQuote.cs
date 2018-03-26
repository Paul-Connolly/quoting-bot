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
        OverNine
    }

    [Serializable]
    public class MotorQuote
    {
        public static RelayFullCycleMotorService.RelayFullCycleMotorService motorService = new RelayFullCycleMotorService.RelayFullCycleMotorService();
        public string VehicleRegistration;
        public int? VehicleValue;
        public string AreaVehicleIsKept;
        public DateTime? EffectiveDate;
        public string FirstName;
        public string LastName;
        public DateTime? DateOfBirth;
        public LicenceTypes? LicenceType;
        public NoClaimsDiscount? NoClaimsDiscount;
        public string PrimaryContactNumber;
        public string EmailAddress;

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
                .Confirm("Would you to request a quote using the following details?\n" +
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

            riskInfo.Proposer = new IrishProposerInfo
            {
                ProposerType = enmProposerType.eIndividual,
                TitleCode = "005",
                Title = "Mr.",
                ForeName = state.FirstName,
                SurName = state.LastName,
                Sex = "M",
                MaritalStatus = "M",
                DateOfBirth = (DateTime)state.DateOfBirth,
                Address = new IrishAddressInfo
                {
                    Line1 = "1 Main Street",
                    Line2 = "Donegal",
                    Line3 = "County Donegal",
                    GeoCode = "38443614",
                    MatchType = "subbuilding",
                    MatchLevel = "100",
                    RatingFactor = "1.391",
                    MRACode = "MRA268067012",
                    SmallAreaIdentifier = 5354,
                    EcadIdentifier = 1700378046,
                    EcadMatchLevelCode = "2",
                    EcadMatchResultCode = "100",
                    Eircode = "D11F6E5",
                    ProvidedBy = AddressProvider.Gamma
                },
                Contact = new IrishContactInfo
                {
                    Home = state.PrimaryContactNumber,
                    Email = state.EmailAddress
                },
                NCD = new IrishNCDInfo
                {
                    ClaimedYearsEarned = 5,
                    DrivingExperienceYears = 0,
                    ClaimedCountry = "IE",
                    ClaimedInsurer = "029",
                    PreviousPolicyNumber = "123456789",
                    DrivingExperiencePolicyExpiryDate = DateTime.Now.AddDays(1),
                    ClaimedDiscountType = "S",
                    ClaimedBonusProtectionType = "F",
                    ClaimedProtectedInd = false,
                    ProtectionRequiredInd = true,
                    DrivingExperienceProvenInd = true,
                    ClaimedProvenInd = false,
                    PreviousPolicyExpiryDate = DateTime.Now.AddDays(-5),
                    RebrokeYearsProvided = false,
                    RebrokeYears = 0
                },
                YearsAtHomeAddress = 0,
                HomeownerInd = "N"
            };
            riskInfo.Policy = new IrishPolicyInfo
            {
                PolicyNumber = "QWERTY12345",
                StartTime = "000100",
                EndTime = "120000",
                PreviousInsurer = "029",
                QuoteAuthor = "RLY",
                CurrencyRequired = "EUR",
                InceptionDate = DateTime.Now,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(1),
                CurrentYear = DateTime.Now.Year,
                PreviouslyInsuredInd = true,
                SecondCarQuotationInd = false
            };
            riskInfo.Driver[0] = new IrishDriverInfo
            {
                PRN = 1,
                RelationshipToProposer = "Z",
                DriverLicenceNumber = "550956042",
                Title = "005",
                Forename = state.FirstName,
                Surname = state.LastName
            };

            return riskInfo;
        }
    }
}