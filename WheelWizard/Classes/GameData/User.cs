using System.Collections.Generic;

namespace CT_MKWII_WPF.Classes;

public class User
{
    public string Name { get; set; }
    public string FriendCode { get; set; }
    public string MiiData { get; set; } // Base64 encoded
    public int VR { get; set; }
    public int BR { get; set; }
    public int TotalRaceCount { get; set; }
    public List<Friend> Friends { get; set; }

    public User()
    {
        Friends = new List<Friend>();
    }
}