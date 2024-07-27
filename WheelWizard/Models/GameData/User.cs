using CT_MKWII_WPF.Services.WiiManagement.GameData;
using System.Collections.Generic;

namespace CT_MKWII_WPF.Models.GameData;

public class User
{
    public string Name { get; set; }
    public string FriendCode { get; set; }
    public GameDataLoader.MiiData MiiData { get; set; }
    public uint Vr { get; set; }
    public uint Br { get; set; }
    public int TotalRaceCount { get; set; }
    public List<Friend> Friends { get; set; }

    public User()
    {
        Friends = new List<Friend>();
    }
}
