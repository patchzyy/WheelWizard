using System.Collections.Generic;
using WheelWizard.Models.Enums;
using WheelWizard.Utilities;

namespace WheelWizard.Models.RRInfo;

public class RrPlayer 
{
    // These variables should not be renamed since they are directly mapped to the JSON object
    public required string Count { get; set; } // you can have one Wii that with 2 players (and hence the Mii list)
    public required string Pid { get; set; }
    public required string Name { get; set; }
    public required string ConnMap { get; set; } // its always there, but we dont use 
    public required string ConnFail { get; set; }
    public required string Suspend { get; set; }
    public required string Fc { get; set; }
    public required string Ev { get; set; } = "--"; // private games don't have EV and EB
    public required string Eb { get; set; } = "--";
    public required List<Mii> Mii { get; set; } = new List<Mii>();

    public int PlayerCount => int.Parse(Count);
    public Mii? FirstMii => Mii.Count <= 0 ? null : Mii[0];
    
    public int Vr
    {
        get
        {
            int evValue;
            return int.TryParse(Ev, out evValue) ? evValue : -1;
        }
    }
    
    public PlayerWinPosition WinPosition => FriendCodeManager.GetWinPosition(Fc);
}
