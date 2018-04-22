using System;
using System.ComponentModel;

namespace QuotingBot.Common.Enums
{
    [Serializable]
    public enum Emoji
    {
        [Description("\U0001F604")]
        GrinningFace,
        [Description("\U0001F914")]
        ThinkingFace,
        [Description("\U0001F44D")]
        ThumbsUp,
        [Description("\U0001F698")]
        Car,
        [Description("\U0001F3E1")]
        House,
        [Description("\U0001F4E7")]
        Email
    }
}