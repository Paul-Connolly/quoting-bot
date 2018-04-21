using System;
using System.Globalization;
using QuotingBot.Enums;
using QuotingBot.Logging;
using QuotingBot.RelayFullCycleMotorService;

namespace QuotingBot.Models.Motor
{
    [Serializable]
    public class MotorQuote
    {
        public static Error errorLogging = new Error();
        public static RelayFullCycleMotorService.RelayFullCycleMotorService motorService = new RelayFullCycleMotorService.RelayFullCycleMotorService();
        public static EnumConverters enumConverters = new EnumConverters();
        public string VehicleRegistration;
        public string VehicleValue;
        public string AreaVehicleIsKept;
        public string FirstName;
        public string LastName;
        public string DateOfBirth;
        public LicenceType? LicenceType;
        public NoClaimsDiscount? NoClaimsDiscount;
        public string PrimaryContactNumber;
        public string EmailAddress;
        public Vehicle Vehicle = new Vehicle();

        public static Vehicle GetVehicle(string vehicleRegistration)
        {
            var vehicle = new Vehicle();
            try
            {
                vehicle.AbiCode = motorService.GetVehicleLookup(
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

                if (!string.IsNullOrEmpty(vehicle.AbiCode))
                {
                    vehicle = GetVehicleDetails(vehicle.AbiCode);
                }
            }
            catch (Exception exception)
            {
                errorLogging.Log(DateTime.Now.ToString(), exception.InnerException.ToString());
                throw;
            }

            return vehicle;
        }

        private static Vehicle GetVehicleDetails(string ABICode)
        {
            var vehicle = new Vehicle();

            try
            {
                var vehicleLookupItem = motorService.GetVehicleDetailsABI(ABICode);
                vehicle.AbiCode = ABICode;
                vehicle.Description = vehicleLookupItem.Description;
                vehicle.Manufacturer = vehicleLookupItem.Manufacturer;
                vehicle.Model = vehicleLookupItem.Model;
                vehicle.BodyType = vehicleLookupItem.BodyType;
                vehicle.EngineCapacity = vehicleLookupItem.EngineCapacity;
                vehicle.NumberOfDoors = vehicleLookupItem.NumberDoors;
                vehicle.FuelType = vehicleLookupItem.FuelType;
                vehicle.YearOfFirstManufacture = vehicleLookupItem.YearOfFirstManufacture;
            }
            catch(Exception exception)
            {
                errorLogging.Log(DateTime.Now.ToString(), exception.InnerException.ToString());
                throw;
            }

            return vehicle;
        }

        public static IrishMQRiskInfo BuildIrishMQRiskInfo(MotorQuote state)
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
                LicenceType = enumConverters.ConvertLicenceType(state.LicenceType),
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
                DateOfBirth = Convert.ToDateTime(state.DateOfBirth, new CultureInfo("en-GB")),
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
                        OccupationCode = "SBB",
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
                DateOfBirth = Convert.ToDateTime(state.DateOfBirth, new CultureInfo("en-GB")),
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
                    ClaimedYearsEarned = enumConverters.ConvertNoClaimsDiscount(state.NoClaimsDiscount),
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
                Value = Convert.ToInt32(state.VehicleValue),
                AnnualMilage = 10000,
                BusinessMileage = 0,
                PleasureMileage = 10000,
                NonStandardAudioValue = 0,
                CarPhoneValue = 0,
                NoDriversFullLicence = 1,
                NoOfSeats = 5,
                ManufacturedYear = 2005,
                FirstRegdYear = 2005,
                ModelCode = state.Vehicle.AbiCode,
                ModelName = state.Vehicle.Description,
                KeptAt = "HA",
                AreaKeptAt = "DX11",
                CubicCapacity = state.Vehicle.EngineCapacity.ToString(),
                BodyType = "5",
                OvernightLocation = "2",
                AreaRating = "DX11",
                Owner = "1",
                RegistrationNo = state.VehicleRegistration,
                RegisteredKeeper = "1",
                DateManufactured = new DateTime(state.Vehicle.YearOfFirstManufacture, 01, 01, 02, 00, 00),
                DateFirstRegistered = new DateTime(state.Vehicle.YearOfFirstManufacture, 01, 01, 02, 00, 00),
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
                StartDate = DateTime.Now.AddDays(1),
                ExpiryDate = DateTime.Now.AddYears(1),
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

        public static MessageRequestInfo BuildMessageRequestInfo()
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