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
                    MessageRequestInfo: BuildMessageRequestInfo());
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
            riskInfo.Driver = new IrishDriverInfo[1];
            riskInfo.Vehicle = new IrishVehicleInfo[1];
            riskInfo.Cover = new IrishCoverInfo[1];

            var driver = new IrishDriverInfo
            {
                PRN = 1,
                RelationshipToProposer = "Z",
                DriverLicenceNumber = "550956042",
                Title = "005",
                Forename = state.FirstName,
                Surname = state.LastName,
                Sex = "M",
                MaritalStatus = "M",
                LicenceType = "B",
                LicenceCountry = "IE",
                ProsecutionPending = false,
                LicenceRestrictionInd = false,
                QualificationsInd = false,
                NonMotoringConviction = false,
                PrevRefusedCover = false,
                OtherVehicleOwned = false,
                PrevRestrictiveTerms = false,
                RegisteredDisabled = false,
                ClaimsIndicator = false,
                PenaltyPointsIndicator = false,
                ConvictionsInd = false,
                MedicalConditionsInd = false,
                ResidentOutsideIreland = false,
                PermResident = true,
                NonDrinker = false,
                TempAdditionalDriver = false,
                DateOfBirth = new DateTime(1979, 06, 04, 02, 00, 00),
                IrelandResidencyDate = new DateTime(2000, 04, 11, 02, 00, 00),
                IrelandLicenceDate = new DateTime(2014, 08, 28, 02, 00, 00),
                NameddriverNCDClaimedYears = 6,
                ResidentWithProposer = false,
                FullTimeUseOfOtherCar = false,
                IsResidentWithProposer = false,
                PrevImposedTerms = false,
                Occupation = new IrishOccupationInfo[]
                {
                    new IrishOccupationInfo
                    {
                        FullTimeEmployment = true,
                        OccupationCode = "SSB",
                        EmployersBusiness = "120",
                        EmploymentType = "E"
                    }
                },
                DrivesVehicle = new IrishDrivesVehicleInfo[]
                {
                    new IrishDrivesVehicleInfo
                    {
                        VehicleReferenceNumber = 1,
                        DrivingFrequency = "M",
                        Use = "4"
                    }
                }
            };

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

            riskInfo.Driver[0] = driver;
            riskInfo.Vehicle[0] = new IrishVehicleInfo
            {
                PRN = 1,
                Value = 4000,
                AnnualMilage = 10000,
                BusinessMileage = 0,
                PleasureMileage = 10000,
                NonStandardAudioValue = 0,
                CarPhoneValue = 0,
                NoDriversFullLicence = 1,
                NoOfSeats = 5,
                ManufacturedYear = 2005,
                FirstRegdYear = 2005,
                ModelCode = "95012085",
                ModelName = "AUDI A3 1.6 ATTRACTION SPORTPACK",
                KeptAt = "HA",
                AreaKeptAt = "DX11",
                CubicCapacity = "1595",
                BodyType = "5",
                OvernightLocation = "2",
                AreaRating = "DX11",
                Owner = "1",
                RegistrationNo = "05OY3466",
                RegisteredKeeper = "1",
                DateManufactured = new DateTime(2005, 12, 15, 02, 00, 00),
                DateFirstRegistered = new DateTime(2005, 12, 15, 02, 00, 00),
                DatePurchased = new DateTime(2017, 05, 01, 02, 00, 00),
                ModifiedInd = false,
                IrelandRegistered = false,
                Imported = false,
                SecurityDeviceInd = true,
                TrailerInd = false,
                SecondCarInd = false,
                TemporaryAddVehicle = false,
                TemporarySubInd = false,
                LeftOrRightHandDrive = (char) 82,
                ReferenceNumber = 1,
                Security = new IrishSecurityInfo
                {
                    Type = "1002"
                },
                Uses = new IrishUsesInfo
                {
                    Code = "4"
                },
                DrivenBy = new IrishDrivenByInfo[]
                {
                    new IrishDrivenByInfo
                    {
                        DriverReferenceNumber = 1,
                        DrivingFrequency = "M"
                    },
                    new IrishDrivenByInfo()
                    {
                        DriverReferenceNumber = 2,
                        DrivingFrequency = "F"
                    }
                },
                VehicleType = 0
            };
            riskInfo.Cover[0] = new IrishCoverInfo
            {
                Code = "01",
                PeriodUnits = "2",
                Period = "12",
                CertificateNumber = "0",
                StartTime = "000100",
                StartDate = new DateTime(2017, 07, 10, 00, 00, 00),
                ExpiryDate = new DateTime(2018, 07, 09, 01, 00, 00),
                RequiredDrivers = "5",
                VehicleRefNo = 1,
                TotalTempMTA = 0,
                TotalTempMTAInForce = 0,
                TotalTempAddDriverInForce = 0,
                TotalTempAddDriver = 0,
                TotalTempAddVehicle = 0,
                TotalTempSub = 0,
                VoluntaryExcess = 300,
                WindscreenLimit = 0
            };
            riskInfo.Intermediary = new IntermediaryInfo
            {
                Name = "RE0668",
                Number = 0,
                RIAccountIdentifier = "relay1:0099"
            };
            riskInfo.TransactionDetail = new TransactionDetails
            {
                BrokerFee = 0
            };
            riskInfo.DiscountInfo = new IrishDiscountInfo
            {
                IsWebQuote = false,
                WebDiscountPercentage = 0
            };
            return riskInfo;
        }

        private static MessageRequestInfo BuildMessageRequestInfo()
        {
            var messageRequestInfo = new MessageRequestInfo
            {
                BreakdownsSpecified1 = new BreakdownsSpecified
                {
                    BreakdownSpecified1 = new BreakdownType[]
                    {
                        BreakdownType.ExcessItems
                    }
                }
            };

            return messageRequestInfo;
        }
    }
}