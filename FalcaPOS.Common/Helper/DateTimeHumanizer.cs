using Humanizer;
using Humanizer.Localisation;
using System;


namespace FalcaPOS.Common.Helper
{
    public static class DateTimeHumanizer
    {

        public static string ToHumanReadableString(this DateTime dateTime)
        {
            if (dateTime == null) return string.Empty;
            if (dateTime.Date != DateTime.Now.Date)
            {
                var timespan = dateTime.Date - DateTime.UtcNow;

                return timespan.Duration().Humanize(maxUnit: TimeUnit.Year, precision: 2, minUnit: TimeUnit.Day) + " before";

            }
            else
                return "Today";

        }

        public static string HumanizeString(this DateTime dateTime)
        {
            if (dateTime == null) return string.Empty;
            return dateTime.Humanize();

        }
    }
}
