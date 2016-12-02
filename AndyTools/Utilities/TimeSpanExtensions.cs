namespace AndyTools.Utilities
{
    using System;

    public static class TimeSpanExtensions
    {
        public static string ToHourMinSec(this TimeSpan timeSpan)
        {
            var timeSpanString = "";
            var hours = (timeSpan.Days * 24) + timeSpan.Hours;
            timeSpanString += hours > 0 ? $"{hours}h " : "";
            timeSpanString += $"{timeSpan.Minutes}m ";
            timeSpanString += $"{timeSpan.Seconds}s";

            return timeSpanString;
        }
    }
}
