using System.ComponentModel;
using System.Linq;
using WheelWizard.Helpers;
using WheelWizard.Models.Enums;
using WheelWizard.Models.MiiImages;
using WheelWizard.Models.RRInfo;
using WheelWizard.Models.Settings;
using WheelWizard.Services.LiveData;
using WheelWizard.Utilities;

namespace WheelWizard.Models.GameData;

public abstract class GameDataPlayer : INotifyPropertyChanged
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

    public PlayerWinPosition WinPosition => FriendCodeManager.GetWinPosition(FriendCode);
    
    public string MiiName
    {
        get => MiiData?.Mii?.Name ?? SettingValues.NoName;
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
            {
                MiiData.Mii.Name = value;
                OnPropertyChanged(nameof(MiiName));
            }
        }
    }

    #region PropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
