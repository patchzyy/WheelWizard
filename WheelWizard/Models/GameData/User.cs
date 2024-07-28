using CT_MKWII_WPF.Services.WiiManagement.GameData;
using System.Collections.Generic;

namespace CT_MKWII_WPF.Models.GameData;

public class User
{
    public required string FriendCode { get; set; }
    public required GameDataLoader.MiiData MiiData { get; set; }
    public required uint Vr { get; set; }
    public required uint Br { get; set; }
    public required int TotalRaceCount { get; set; }
    
    public required int TotalWinCount { get; set; }
    public List<Friend> Friends { get; set; } = new List<Friend>();
}
