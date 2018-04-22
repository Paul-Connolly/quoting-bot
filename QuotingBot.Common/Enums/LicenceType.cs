using System;
using System.ComponentModel;

namespace QuotingBot.Common.Enums
{
    [Serializable]
    public enum LicenceType
    {
        [Description("Full Irish")]
        FullIrish,
        [Description("Provisional Irish")]
        ProvisionalIrish,
        [Description("Full EU")]
        FullEU,
        [Description("Full UK")]
        FullUK,
        [Description("Foreign Licence")]
        Foreign,
        [Description("International Licence")]
        InternationalLicence,
        [Description("Learner Permit")]
        LearnerPermit
    }
}