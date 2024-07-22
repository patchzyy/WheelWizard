using System;

namespace CT_MKWII_WPF.Helpers;

public static class Humanizer
{
    public static string HumanizeTimeSpan(TimeSpan timeSpan)
    {
        if (Math.Abs(timeSpan.TotalDays) >= 1)
            return $"{timeSpan.Days} day{P(timeSpan.Days)} {Math.Abs(timeSpan.Hours)} hour{P(timeSpan.Hours)}";
        if (Math.Abs(timeSpan.TotalHours) >= 1)
            return $"{timeSpan.Hours} hour{P(timeSpan.Hours)} {Math.Abs(timeSpan.Minutes)} minute{P(timeSpan.Minutes)}";
        if (Math.Abs(timeSpan.TotalMinutes) >= 1)
            return $"{timeSpan.Minutes} minute{P(timeSpan.Minutes)} {Math.Abs(timeSpan.Seconds)} second{P(timeSpan.Seconds)}";

        return $"{timeSpan.Seconds} second{P(timeSpan.Seconds)}";

        // internal method to simplify the pluralization of words
        string P(int count) => Math.Abs(count) != 1 ? "s" : "";
    }
}
