using System;
using System.ComponentModel;

namespace QuotingBot.Common.Enums
{
    [Serializable]
    public enum ResidenceType
    {
        [Description("Owner Occupied")]
        OwnerOccupied,
        [Description("Family Rental")]
        RentedFamily,
        [Description("Student Rental")]
        RentedStudents
    }
}