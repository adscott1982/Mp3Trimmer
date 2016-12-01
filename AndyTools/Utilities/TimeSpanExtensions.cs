namespace AndyTools.Utilities
{
    using System;

    public static class TimeSpanExtensions
    {
        public static string ToHourMinSec(this TimeSpan timeSpan)
        {
            var timeSpanString = "";
            timeSpanString += timeSpan.Hours > 0 ? $"{timeSpan.Hours}h " : "";
            timeSpanString += $"{timeSpan.Minutes}m ";
            timeSpanString += $"{timeSpan.Seconds}s";

            return timeSpanString;
        }
    }
}
