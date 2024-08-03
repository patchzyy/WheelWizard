using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Services.WiiManagement.GameData;
using System.Collections.Generic;

namespace CT_MKWII_WPF.Models.GameData;

public class User : BasePlayer
{
    public required GameDataLoader.MiiData MiiData { get; set; }
    public required int TotalRaceCount { get; set; }
    public required int TotalWinCount { get; set; }
    public List<Friend> Friends { get; set; } = new List<Friend>();
    public new string? MiiBinaryData
    {
        get => MiiData.mii.Data;
        set
        {
            if (MiiData == null)
            {
                MiiData = new GameDataLoader.MiiData
                {
                    mii = new Mii { Data = value ?? "", Name = "" }
                };
            }
            else if (MiiData.mii == null)
            {
                MiiData.mii = new Mii { Data = value ?? "", Name = "" };
            }
            else
            {
                MiiData.mii.Data = value ?? "";
            }
        }
    }

    public string MiiName
    {
        get => MiiData?.mii?.Name ?? "";
        set
        {
            if (MiiData == null)
            {
                MiiData = new GameDataLoader.MiiData
                {
                    mii = new Mii { Data = "", Name = value }
                };
            }
            else if (MiiData.mii == null)
            {
                MiiData.mii = new Mii { Data = "", Name = value };
            }
            else
            {
                MiiData.mii.Name = value;
            }
        }
    }
}
