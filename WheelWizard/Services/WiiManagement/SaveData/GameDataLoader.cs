using WheelWizard.Resources.Languages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WheelWizard.Models.GameData;
using WheelWizard.Models.RRInfo;
using WheelWizard.Services.LiveData;
using WheelWizard.Services.Settings;
using WheelWizard.Utilities.Generators;
using WheelWizard.Utilities.RepeatedTasks;
using WheelWizard.WPFViews.Popups.Generic;

//big big thanks to https://kazuki-4ys.github.io/web_apps/FaceThief/ for the JS implementation of reading the rksys file
//reminder, big endian!!!!


namespace WheelWizard.Services.WiiManagement.SaveData;

public class GameDataLoader : RepeatedTaskManager
{
    public static GameDataLoader Instance { get; } = new();
    private static string SaveFilePath
    {
        get
        {
            var path = Path.Combine(PathManager.RiivolutionWhWzFolderPath, "riivolution", "save");
            if (Directory.Exists(path)) 
                return path;
            
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                // I rather not translate this message, makes it easier to check where a given error came from
                MessageBoxWindow.ShowDialog($"Error creating save directory: {ex.Message}");
            }
            return path;
        }
    }
    private byte[]? _saveData;

    private Models.GameData.GameData GameData { get; }

    private const int RksysSize = 0x2BC000;
    private const string RksysMagic = "RKSD0006";
    private const int MaxPlayerNum = 4;
    private const int RkpdSize = 0x8CC0;
    private const string RkpdMagic = "RKPD";
    private const int MaxFriendNum = 30;
    private const int FriendDataOffset = 0x56D0;
    private const int FriendDataSize = 0x1C0;
    private const int MiiSize = 0x4A;
    public User GetCurrentUser => Instance.GameData.Users[(int)SettingsManager.FOCUSSED_USER.Get()];
    public List<Friend> GetCurrentFriends => Instance.GameData.Users[(int)SettingsManager.FOCUSSED_USER.Get()].Friends;
    public Models.GameData.GameData GetGameData => Instance.GameData;
    public User GetUserData(int index) => GameData.Users[index];
    public bool HasAnyValidUsers => GameData.Users.Any(user => user.FriendCode != "0000-0000-0000");

    
    private GameDataLoader() : base(40)
    {
        GameData = new Models.GameData.GameData();
        LoadGameData();
    }
    
    
    public void RefreshOnlineStatus()
    {
        var currentRooms = RRLiveRooms.Instance.CurrentRooms;
        if (currentRooms.Count > 0)
        {
            var onlinePlayers = currentRooms.SelectMany(room => room.Players.Values).ToList();
            foreach (var user in GameData.Users)
            {
                user.IsOnline = onlinePlayers.Any(player => player.Fc == user.FriendCode);
            }
        }
        else
        {
            foreach (var user in GameData.Users)
            {
                user.IsOnline = false;
            }
        }
    }
    
    
    public void LoadGameData()
    {
        try
        {
            _saveData = LoadSaveDataFile();
            if (_saveData != null && ValidateMagicNumber())
            {
                ParseUsers();
                return;
            }

            GameData.Users.Clear();
            for (var i = 0; i < MaxPlayerNum; i++)
            {
                CreateDummyUser();
            }
        }
        catch (Exception e)
        {
            // I rather not translate this message, makes it easier to check where a given error came from
            MessageBoxWindow.ShowDialog($"An error occurred while loading the game data: {e.Message}");
        }
    }
    
    private void CreateDummyUser()
    {
        var dummyUser = new User
        {
            FriendCode = "0000-0000-0000",
            MiiData = new MiiData
            {
                Mii = new Mii
                {
                    Name = "no license",
                    Data = Convert.ToBase64String(new byte[MiiSize])
                },
                AvatarId = 0,
                ClientId = 0
            },
            Vr = 5000,
            Br = 5000,
            TotalRaceCount = 0,
            TotalWinCount = 0,
            Friends = new List<Friend>(),
            RegionId = 10, //10 will default to unknown
            IsOnline = false
        };
        GameData.Users.Add(dummyUser);
    }

    private void ParseUsers()
    {
        GameData.Users.Clear();
        if (_saveData == null) return;
        for (var i = 0; i < MaxPlayerNum; i++)
        {
            var rkpdOffset = RksysMagic.Length + i * RkpdSize;
            if (Encoding.ASCII.GetString(_saveData, rkpdOffset, RkpdMagic.Length) == RkpdMagic)
            {
                var user = ParseUser(rkpdOffset);
                GameData.Users.Add(user);
            }
            else
            {
                CreateDummyUser();
            }
        }
        if (GameData.Users.Count == 0)
            CreateDummyUser();
    }

    private User ParseUser(int offset)
    {
        if (_saveData == null) throw new ArgumentNullException(nameof(_saveData));
        var user = new User
        {
            MiiData = ParseMiiData(offset + 0x14),
            FriendCode = FriendCodeGenerator.GetFriendCode(_saveData, offset + 0x5C),
            Vr = BigEdianBinaryReader.BufferToUint16(_saveData, offset + 0xB0),
            Br = BigEdianBinaryReader.BufferToUint16(_saveData, offset + 0xB2),
            TotalRaceCount = BigEdianBinaryReader.BufferToUint32(_saveData, offset + 0xB4),
            TotalWinCount = BigEdianBinaryReader.BufferToUint32(_saveData, offset + 0xDC),
            RegionId = (BigEdianBinaryReader.BufferToUint16(_saveData, 0x23308 + 0x3802) / 4096)
        };
        ParseFriends(user, offset);
        return user;
    }
    
    
    private MiiData ParseMiiData(int offset)
    {
        if (_saveData == null) throw new ArgumentNullException(nameof(_saveData));
        var miiData =  new MiiData
        {
            Mii = new Mii
            {
                Name = BigEdianBinaryReader.GetUtf16String(_saveData,offset, 10),
                Data = Convert.ToBase64String(GetMiiData(BitConverter.ToUInt32(_saveData, offset + 0x14)))
            },
            
            AvatarId = BitConverter.ToUInt32(_saveData, offset + 0x10),
            ClientId = BitConverter.ToUInt32(_saveData, offset + 0x14),
        };
        return miiData;
    }

    private static byte[] GetMiiData(UInt32 ClientId)
    {
        List<Byte[]> miis =  InternalMiiManager.GetAllMiiData();
        foreach (var mii in miis)
        {
            uint AvatarId = 0;
            for (var i = 0; i < 4; i++)
            {
                AvatarId |= (uint)(mii[0x18 + i] << (8 * i));
            }

            if (AvatarId == ClientId)
                return mii;
        }
        return Array.Empty<byte>();
    }

    private void ParseFriends(User user, int userOffset)
    {
        var friendOffset = userOffset + FriendDataOffset;
        for (var i = 0; i < MaxFriendNum; i++)
        {
            var currentOffset = friendOffset + i * FriendDataSize;
            if (!CheckMiiData(currentOffset + 0x1A)) continue;
            var friend = new Friend
            {
                Vr = BigEdianBinaryReader.BufferToUint16(_saveData, currentOffset + 0x16), // peaks at 9999 so kinda useless
                Br = BigEdianBinaryReader.BufferToUint16(_saveData, currentOffset + 0x18), // same here
                FriendCode = FriendCodeGenerator.GetFriendCode(_saveData, currentOffset + 4),
                Wins = BigEdianBinaryReader.BufferToUint16(_saveData, currentOffset + 0x14),
                Losses = BigEdianBinaryReader.BufferToUint16(_saveData, currentOffset + 0x12),
                CountryCode = _saveData[currentOffset + 0x68],
                RegionId = _saveData[currentOffset + 0x69],
                MiiData = new MiiData
                {
                    Mii = new Mii
                    {
                        Name = BigEdianBinaryReader.GetUtf16String(_saveData, currentOffset + 0x1C, 10),
                        Data = Convert.ToBase64String(_saveData.AsSpan(currentOffset + 0x1A, MiiSize))
                    },
                    AvatarId = 0,
                    ClientId = 0
                },
            };
            user.Friends.Add(friend);
        }
    }

    private bool CheckMiiData(int offset)
    {
        for (var i = 0; i < MiiSize; i++)
        {
            if (_saveData != null && _saveData[offset + i] != 0)
                return true;
        }

        return false;
    }
    
    
    private bool ValidateMagicNumber()
    {
        return Encoding.ASCII.GetString(_saveData, 0, RksysMagic.Length) == RksysMagic;
    }

    private static byte[]? LoadSaveDataFile()
    {
        try
        {
            if (!Directory.Exists(SaveFilePath))
                return null;

            var saveFile = Directory.GetFiles(SaveFilePath, "rksys.dat", SearchOption.AllDirectories);
            return saveFile.Length == 0 ? null : File.ReadAllBytes(saveFile[0]);
        }
        catch (Exception ex)
        {
            return null;
        }
    }


    protected override Task ExecuteTaskAsync()
    {
        LoadGameData();
        return Task.CompletedTask;
    }
}
