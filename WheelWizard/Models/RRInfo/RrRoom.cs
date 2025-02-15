using WheelWizard.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using WheelWizard.Helpers;
using WheelWizard.Models.MiiImages;

namespace WheelWizard.Models.RRInfo;

public class RrRoom
{
    public required string Id { get; set; }
    public string? Game { get; set; } // it always exists, but we dont care since we dont use it (and its always "mariokartwii")
    public required DateTime Created { get; set; }
    public required string Type { get; set; }
    public required bool Suspend { get; set; }
    public required string Host { get; set; } // the key of player in the players map (that started the room)
    public string? Rk { get; set; } // RK does not exists in private rooms
    public required Dictionary<string, RrPlayer> Players { get; set; }

    public int PlayerCount => Players.Sum(p => p.Value.PlayerCount);

    public string TimeOnline => Humanizer.HumanizeTimeSpan(DateTime.UtcNow - Created);
    public bool IsPublic => Type == "public";
    public string GameModeAbbrev => Rk switch
    {
        "vs_10" => "RR",
        "vs_11" => "TT",
        "vs_12" => "200",
        "vs_751" => "VS",
        "vs_-1" => "Reg",
        _ => "??"
    };
    public string GameMode => Rk switch
    {
        "vs_10" => "Retro Rewind 150CC",
        "vs_11" => "Online Time Trails",
        "vs_12" => "Retro Rewind 200CC",
        "vs_751" => "Verses",
        "vs_-1" => "Reg?? idk what this is",
        _ => "Unknown Game Mode"
    };
    public Mii? HostMii => Players.GetValueOrDefault(Host)?.FirstMii;
}
