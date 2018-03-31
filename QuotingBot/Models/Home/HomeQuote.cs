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

            request.RelayNumber = "RE0844";
            request.Password = "AWU/ZRg*BfY&amp;t/";
            request.BrokerId = "5016";
            request.LoginId = "eQuote";
            request.BrokerName = "First Ireland Risk Management";
            request.ClientVersion = 0;
            request.BusinessProcess = BusinessProcess.NewBusiness;
            request.ProcessingType = InsurerConfirmationProcessingType.Standard;

            request.Policy.EffectiveStartDate = DateTime.Now;
            request.Policy.VoluntaryExcess = 0;
            request.Policy.BrokerPolicyReference = "NOV17-Y8AONF";
            request.Policy.CorrespondenceContact.Title = PersonTitle.Unknown;
            request.Policy.CorrespondenceContact.DateOfBirth = new DateTime(0001, 01, 01, 00, 00, 00);
            request.PolicyHolders[0] = policyHolder;
            request.Occupancy.ResidenceType = (RelayHouseholdService.ResidenceType)state.ResidenceType;
            request.Occupancy.ProposerType = ProposerType.Unspecified;
            request.Occupancy.YearsLivingAtAddress = 0;
            request.Occupancy.NumberOfPayingGuests = 0;
            request.Occupancy.SocialWelfareLet = false;
            request.Occupancy.IsFurnished = false;
            request.Occupancy.NormalDaytimeOccupancy = false;
            request.Occupancy.NumberOfDaysUnoccupiedPerWeek = 0;
            request.Occupancy.NumberOfTimesLetInAYear = 0;
            request.Building.PropertyType = (RelayHouseholdService.PropertyType) state.PropertyType;
            request.Building.PropertySubType = PropertySubType.DetachedHouse;
            request.Building.ConstructionDate = new DateTime(2017, 01, 01, 00, 00, 00);
            request.Building.ListedBuilding = false;
            request.Building.RoofConstruction = RoofConstructionType.Standard;
            request.Building.WallConstruction = WallConstructionType.Unknown;
            request.Building.RoofPercentage = 0;
            request.Building.NumberOfBedrooms = (int) state.NumberOfBedrooms;
            request.Building.NumberOfBathrooms = 3;
            request.Building.NumberOfSmokeDetectors = 2;
            request.Building.Alarm = new Alarm { AlarmType = AlarmType.Unspecified };
            request.Building.Locks = true;
            request.Building.NeighbourhoodWatchInArea = true;
            request.Building.Basement = false;
            request.Building.HeatingType = HeatingType.Electric;
            request.Building.BuildingSize = 0;
            request.Building.BuildingSizeUnitOfMeasurement = UnitOfMeasurement.Unknown;
            request.Building.GarageSize = 0;
            request.Building.GarageSizeUnitOfMeasurement = UnitOfMeasurement.Unknown;
            request.Building.FreeFromFlooding = true;
            request.Building.FreeFromGroundHeave = true;
            request.Building.FreeFromLandslip = true;
            request.Building.FreeFromSubsidence = true;
            request.Building.GoodRepair = true;
            request.Building.SafeInstalled = false;
            request.Building.UndertakeToMaintain = false;

            return request;
        }
    }
}