namespace CT_MKWII_WPF.Models.GameData;

public class Friend : BasePlayer
{
    public required string Name { get; set; }
    public uint Wins { get; set; }
    public uint Losses { get; set; }

    // MiiBinaryData is now in the base class
    public new string? MiiBinaryData
    {
        get => base.MiiBinaryData;
        set => base.MiiBinaryData = value;
    }
}
