using System;

namespace LocationCapture.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ExtractFirstDayOfMonth(this DateTime input)
        {
            return new DateTime(input.Year, input.Month, 1);
        }

        public static DateTime ExtractEndOfDay(this DateTime input)
        {
            return new DateTime(input.Year, input.Month, input.Day, 23, 59, 59);
        }

        public static DateTime ExtractLastDayOfMonth(this DateTime input)
        {
            return input.AddMonths(1)
                .ExtractFirstDayOfMonth()
                .AddDays(-1);
        }
    }
}
