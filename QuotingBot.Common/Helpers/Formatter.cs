using System;

namespace QuotingBot.Common.Helpers
{
    [Serializable]
    public class Formatter
    {
        public Formatter() { }

        public string CapitilzeFirstLetter(string value)
        {
            char[] characters = value.ToCharArray();
            characters[0] = char.ToUpper(characters[0]);

            return new string(characters);
        }
    }
}