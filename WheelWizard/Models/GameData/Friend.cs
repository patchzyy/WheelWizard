namespace CT_MKWII_WPF.Models.GameData;

public class Friend
{
    public string? Name { get; set; }
    public string? FriendCode { get; set; }
    public uint Wins { get; set; }
    public uint Losses { get; set; }
    
    public uint Vr { get; set; }
    
    public uint Br { get; set; }
    public string? MiiData { get; set; }
}
