using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Services.WiiManagement.GameData;
using System.Collections.Generic;

namespace CT_MKWII_WPF.Models.GameData;

public class User : BasePlayer
{
    public required int TotalRaceCount { get; set; }
    public required int TotalWinCount { get; set; }
    public List<Friend> Friends { get; set; } = new List<Friend>();
    
}
