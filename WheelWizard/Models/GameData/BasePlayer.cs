using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Services.LiveData;
using System.ComponentModel;
using System.Linq;

namespace CT_MKWII_WPF.Models.GameData;

public abstract class BasePlayer : INotifyPropertyChanged
{
    public required string FriendCode { get; init; }
    public required uint Vr { get; init; }
    public required uint Br { get; init; }
    public required uint RegionId { get; init; } 
    public required MiiData? MiiData { get; set; }
    
    public string RegionName => Humanizer.GetRegionName(RegionId);
    public Mii? Mii => MiiData?.Mii;
    
    public bool IsOnline
    {
        get
        {
            var currentRooms = RRLiveRooms.Instance.CurrentRooms;
            if (currentRooms.Count <= 0) 
                return false;

            var onlinePlayers = currentRooms.SelectMany(room => room.Players.Values).ToList();
            return onlinePlayers.Any(player => player.Fc == FriendCode);
        }
        set
        {
            if (value == IsOnline) 
                return;
            
            OnPropertyChanged(nameof(IsOnline));
        }
    }
    
    public string MiiName
    {
        get => MiiData?.Mii?.Name ?? "";
        set
        {
            if (MiiData == null)
            {
                MiiData = new MiiData
                {
                    Mii = new Mii { Data = "", Name = value }
                };
            }
            else if (MiiData.Mii == null)
                MiiData.Mii = new Mii { Data = "", Name = value };
            else
                MiiData.Mii.Name = value;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
