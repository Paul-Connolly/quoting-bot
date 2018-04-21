using System;

namespace QuotingBot.Common.Enums
{
    [Serializable]
    public class EnumConverters
    {
        public EnumConverters() { }

        public string ConvertLicenceType(object value)
        {
            switch (value)
            {
                case LicenceType.FullIrish:
                    return "C";
                case LicenceType.ProvisionalIrish:
                    return "B";
                case LicenceType.FullEU:
                    return "F";
                case LicenceType.FullUK:
                    return "C";
                case LicenceType.Foreign:
                    return "I";
                case LicenceType.InternationalLicence:
                    return "N";
                case LicenceType.LearnerPermit:
                    return "G";
                default:
                    return string.Empty;
            }

        }

        public int ConvertNoClaimsDiscount(object value)
        {
            switch (value)
            {
                case NoClaimsDiscount.One:
                    return 1;
                case NoClaimsDiscount.Two:
                    return 2;
                case NoClaimsDiscount.Three:
                    return 3;
                case NoClaimsDiscount.Four:
                    return 4;
                case NoClaimsDiscount.Five:
                    return 5;
                case NoClaimsDiscount.Six:
                    return 6;
                case NoClaimsDiscount.Seven:
                    return 7;
                case NoClaimsDiscount.Eight:
                    return 8;
                case NoClaimsDiscount.NineAndMore:
                    return 9;
                default:
                    return 0;
            }
        }

        public RelayHouseholdService.PropertyType ConvertPropertyType(PropertyType? propertyType)
        {
            switch (propertyType)
            {
                case PropertyType.Bungalow:
                    return RelayHouseholdService.PropertyType.Bungalow;
                case PropertyType.DetachedHouse:
                    return RelayHouseholdService.PropertyType.DetachedHouse;
                case PropertyType.Flat:
                    return RelayHouseholdService.PropertyType.Flat;
                case PropertyType.SemiDetachedHouse:
                    return RelayHouseholdService.PropertyType.SemiDetachedHouse;
                case PropertyType.TerracedHouse:
                    return RelayHouseholdService.PropertyType.TerracedHouse;
                default:
                    return RelayHouseholdService.PropertyType.Unknown;
            }
        }

        public RelayHouseholdService.ResidenceType ConvertResidencyType(ResidenceType? residenceType)
        {
            switch (residenceType)
            {
                case ResidenceType.OwnerOccupied:
                    return RelayHouseholdService.ResidenceType.OwnerOccupied;
                case ResidenceType.RentedFamily:
                    return RelayHouseholdService.ResidenceType.RentedFamily;
                case ResidenceType.RentedStudents:
                    return RelayHouseholdService.ResidenceType.RentedStudents;
                default:
                    return RelayHouseholdService.ResidenceType.Unspecified;
            }
        }
    }
}