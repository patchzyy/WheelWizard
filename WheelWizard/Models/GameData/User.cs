using System.Collections.Generic;

namespace CT_MKWII_WPF.Models.GameData;

public class User
{
    public string Name { get; set; }
    public string FriendCode { get; set; }
    public string MiiData { get; set; } // Base64 encoded
    public int Vr { get; set; }
    public int Br { get; set; }
    public int TotalRaceCount { get; set; }
    public List<Friend> Friends { get; set; }

    public User()
    {
        Friends = new List<Friend>();
    }
}
