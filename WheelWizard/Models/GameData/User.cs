using WheelWizard.Models.RRInfo;
using System.Collections.Generic;

namespace WheelWizard.Models.GameData;

public class User : BasePlayer
{
    public required uint TotalRaceCount { get; set; }
    public required uint TotalWinCount { get; set; }
    public List<Friend> Friends { get; set; } = new List<Friend>();
    
}
