using System;

namespace CT_MKWII_WPF.Utilities;

public static class Humanizer
{
    public static string HumanizeGameMode(string? mode)
    {
        return mode switch
        {
            "vs_10" => "VS",
            "vs_11" => "TT",
            "vs_751" => "VS",
            _ => "??"
        };
    }
        
    public static string HumanizeTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalDays >= 1)
            return $"{timeSpan.Days} day{P(timeSpan.Days)} {timeSpan.Hours} hour{P(timeSpan.Hours)}";
        if (timeSpan.TotalHours >= 1)
            return $"{timeSpan.Hours} hour{P(timeSpan.Hours)} {timeSpan.Minutes} minute{P(timeSpan.Minutes)}";
        if (timeSpan.TotalMinutes >= 1)
            return $"{timeSpan.Minutes} minute{P(timeSpan.Minutes)} {timeSpan.Seconds} second{P(timeSpan.Seconds)}";

        return $"{timeSpan.Seconds} second{P(timeSpan.Seconds)}";

        // internal method to simplify the pluralization of words
        string P(int count) => count != 1 ? "s" : "";
    }
}