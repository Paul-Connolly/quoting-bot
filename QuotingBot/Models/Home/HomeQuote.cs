using System;
using QuotingBot.RelayFullCycleMotorService;
using QuotingBot.RelayHouseholdService;

namespace QuotingBot.Models.Home
{
    public enum PropertyType
    {
        Bungalow,
        DetachedHouse,
        Flat,
        SemiDetachedHouse,
        TerracedHouse
    }

    public enum ResidenceType
    {
        OwnerOccupied,
        RentedFamily,
        RentedStudents
    }

    [Serializable]
    public class HomeQuote
    {
        public string FirstLineOfAddress;
        public string Town;
        public string County;
        public string FirstName;
        public string LastName;
        public string PrimaryContactNumber;
        public string EmailAddress;
        public PropertyType? PropertyType;
        public ResidenceType? ResidenceType;
        public string YearBuilt;
        public int? NumberOfBedrooms;
        public int? BuildingsCover;
        public int? ContentsCover;

        public static HomeWebServiceRequest BuildHomeWebServiceRequest(HomeQuote state)
        {
            var request = new HomeWebServiceRequest();
            request.PolicyHolders = new PolicyHolder[1];
            request.Risks = new Risk[2];

            var policyHolder = new PolicyHolder
            {
                EffectivePrimaryPolicyHolder = true,
                OccupationType = OccupationType.QuantitySurveyor,
                EmployersBusinessType = EmployersBusinessType.Unknown,
                ProfessionalBodyType = ProfessionalBodyType.Unknown,
                MaritalStatus = MaritalStatus.Single,
                EmploymentType = EmploymentType.Employed,
                FirstTimeBuyer = false,
                Smoker = false,
                Cancelled = false,
                Declined = false,
                Conviction = false,
                DeclaredBankrupt = false,
                SpecialConditions = false,
                Relationship = RelationshipType.Unknown,
                Contact = new Contact
                {
                    Title = PersonTitle.Mr,
                    FirstName = state.FirstName,
                    Surname = state.LastName,
                    Address = new Address
                    {
                        BuildingName = "Dranagh",
                        StreetName = state.FirstLineOfAddress,
                        Town = state.Town,
                        County = state.County
                    },
                    DateOfBirth = new DateTime(1987, 03, 12, 00, 00, 00),
                    PhoneNumber = state.PrimaryContactNumber,
                    EmailAddress = state.EmailAddress
                }
            };

            request.RelayNumber = "RE0930";
            request.Password = "1IJ4^E?K]Syb>w";
            request.BrokerId = "5016";
            request.LoginId = "eQuote";
            request.BrokerName = "First Ireland Risk Management";
            request.ClientVersion = 0;
            request.BusinessProcess = BusinessProcess.NewBusiness;
            request.ProcessingType = InsurerConfirmationProcessingType.Standard;

            request.Policy = new Policy
            {
                EffectiveStartDate = DateTime.Now,
                VoluntaryExcess = 0,
                BrokerPolicyReference = "NOV17-Y8AONF",
                CorrespondenceContact = new Contact()
                {
                    Title = PersonTitle.Unknown,
                    DateOfBirth = new DateTime(0001, 01, 01, 00, 00, 00)
                }
            };

            request.PolicyHolders[0] = policyHolder;

            request.Occupancy = new Occupancy
            {
                ResidenceType = RelayHouseholdService.ResidenceType.OwnerOccupied,
                ProposerType = ProposerType.Unspecified,
                YearsLivingAtAddress = 0,
                NumberOfPayingGuests = 0,
                SocialWelfareLet = false,
                IsFurnished = false,
                NormalDaytimeOccupancy = false,
                NumberOfDaysUnoccupiedPerWeek = 0,
                NumberOfTimesLetInAYear = 0
            };

            request.Building = new Building
            {
                PropertyType = RelayHouseholdService.PropertyType.DetachedHouse,
                PropertySubType = PropertySubType.DetachedHouse,
                ConstructionDate = new DateTime(2017, 01, 01, 00, 00, 00),
                ListedBuilding = false,
                RoofConstruction = RoofConstructionType.Standard,
                WallConstruction = WallConstructionType.Unknown,
                RoofPercentage = 0,
                NumberOfBedrooms = (int)state.NumberOfBedrooms,
                NumberOfBathrooms = 3,
                NumberOfSmokeDetectors = 2,
                Alarm = new Alarm { AlarmType = AlarmType.Unspecified },
                Locks = true,
                NeighbourhoodWatchInArea = true,
                Basement = false,
                HeatingType = HeatingType.Electric,
                BuildingSize = 0,
                BuildingSizeUnitOfMeasurement = UnitOfMeasurement.Unknown,
                GarageSize = 0,
                GarageSizeUnitOfMeasurement = UnitOfMeasurement.Unknown,
                FreeFromFlooding = true,
                FreeFromGroundHeave = true,
                FreeFromLandslip = true,
                FreeFromSubsidence = true,
                GoodRepair = true,
                SafeInstalled = false,
                UndertakeToMaintain = false
            };

            request.RiskAddress = new Address
            {
                StreetName = state.FirstLineOfAddress,
                Town = state.Town,
                County = state.County,
                FreeText1 = $"{state.FirstLineOfAddress}, {state.Town}, {state.County}",
                AddressMatchResults = new AddressMatchResult[]
                {
                    new AddressMatchResult
                    {
                        ProvidedBy = AddressLookupProvider.Gamma,
                        GeoCode = "40291613",
                        MatchType = "region",
                        Reference = "4IZJT7KP6X734AQK",
                        MatchLevel = "700",
                        IsFallbackResult = false,
                        LookupResponse =
                            "<![CDATA[&lt;location type='region' territory='SPIKE_GAMMA' score='99.99' xmlns='http://service.autoaddress.ie/'&gt;&lt;point x='275383.86' y='138100.57' coord-sys='ING' /&gt;&lt;info ecadId='1110030370' eircode=' Autoaddressid='4IZJT7KP6X734AQK' geover='Q117' geotype='L' georef='40291613' name='' text='Dranagh,Saint Mullins,Co. Carlow' addr1='Dranagh' addr2='Saint Mullins' addr3='Co. Carlow' matchLevel='700' matchResult'100' aa2MatchLevel='7' aa2MatchResult='300' smallarea='017020001' ecadSmallarea='48' /&gt;&lt;/location&gt;]]>"
                    }
                }
            };

            request.Risks[0] = new Risk
            {
                Group = RateBreakdownGroup.HouseStructure,
                SumInsured = 300000
            };
            request.Risks[1] = new Risk
            {
                Group = RateBreakdownGroup.HouseContents,
                SumInsured = 25000
            };

            request.HomeRequestSource = HomeRequestSource.eQuote;
            request.QuoteReference = "NOV17-Y8AONF";
            request.FullQuoteRequest = false;

            return request;
        }
    }
}