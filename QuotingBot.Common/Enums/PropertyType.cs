using System;
using System.ComponentModel;

namespace QuotingBot.Common.Enums
{
    [Serializable]
    public enum PropertyType
    {   
        [Description("Bungalow")]
        Bungalow,
        [Description("Detached House")]
        DetachedHouse,
        [Description("Flat")]
        Flat,
        [Description("Semi-Detached House")]
        SemiDetachedHouse,
        [Description("Terraced House")]
        TerracedHouse
    }
}