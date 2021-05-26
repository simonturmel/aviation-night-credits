using System;

namespace NightCredits
{
    public static class DateTimeExtensions
    {
        public static DateTime RoundToMinutes(this DateTime source)
        {
            if (source.Second >= 30)
            {
                source = source.AddMinutes(1);
            }

            source = source.AddSeconds(-source.Second);

            return source;
        }
    }
}
