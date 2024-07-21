using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CT_MKWII_WPF.Models.RRInfo;

public class Room
{
    public required string Id { get; set; }
    public string? Game { get; set; } // it always exists, but we dont care since we dont use it (and its always "mariokartwii")
    public required DateTime Created { get; set; }
    public required string Type { get; set; }
    public required bool Suspend { get; set; }
    public required string Host { get; set; } // the key of player in the players map (that started the room)
    public string? Rk { get; set; } // RK does not exists in private rooms
    public required Dictionary<string, Player> Players { get; set; }

    public int PlayerCount => Players.Sum(p => p.Value.PlayerCount);

    public string TimeOnline => Humanizer.HumanizeTimeSpan(DateTime.UtcNow - Created + TimeSpan.FromHours(2));
    public bool IsPublic => Type == "public";
    public string GameMode => Rk switch
    {
        "vs_10" => "VS",
        "vs_11" => "TT",
        "vs_751" => "VS",
        _ => "??"
    };
}
