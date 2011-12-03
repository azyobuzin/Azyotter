using System;
using System.Globalization;

namespace Azyobuzi.TaskingTwLib.Util
{
    static class DateTimeUtil
    {
        public static DateTime Parse(string s)
        {
            return DateTime.ParseExact(
                s,
                new string[]
                {
                    "ddd MMM dd HH:mm:ss %zzzz yyyy",
                    "yyyy-MM-ddTHH:mm:ssZ"
                },
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces
            );
        }
        
        public static readonly DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
