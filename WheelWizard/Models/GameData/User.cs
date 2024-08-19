using CT_MKWII_WPF.Models.RRInfo;
using System.Collections.Generic;

namespace CT_MKWII_WPF.Models.GameData;

public class User : BasePlayer
{
    public required uint TotalRaceCount { get; set; }
    public required uint TotalWinCount { get; set; }
    public List<Friend> Friends { get; set; } = new List<Friend>();
    
}
