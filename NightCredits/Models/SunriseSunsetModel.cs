using NightCredits.Extensions;
using System;
using System.Globalization;

namespace NightCredits.Models
{
    public class SunriseSunsetModel
    {
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public DateTime TwilightSunrise => DateTime.Parse(Sunrise, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).AddMinutes(-30).RoundToMinutes();
        public DateTime TwilightSunset => DateTime.Parse(Sunset, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).AddMinutes(30).RoundToMinutes();
    }
}
