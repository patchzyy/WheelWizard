using CT_MKWII_WPF.Models.GameData;
using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Services.LiveData;
using CT_MKWII_WPF.Utilities.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;

//big big thanks to https://kazuki-4ys.github.io/web_apps/FaceThief/ for the JS implementation of reading the rksys file
//Things to keep in mind when working with the rksys.dat file:
// everything is big endian!!!!


namespace CT_MKWII_WPF.Services.WiiManagement.GameData;

public class GameDataLoader
{
    public static GameDataLoader Instance { get; } = new GameDataLoader();
    private static string SaveFilePath => Path.Combine(PathManager.RiivolutionWhWzFolderPath, "riivolution", "save");
    private byte[]? _saveData;

    public Models.GameData.GameData GameData { get; }

    private const int RksysSize = 0x2BC000;
    private const string RksysMagic = "RKSD0006";
    private const int MaxPlayerNum = 4;
    private const int RkpdSize = 0x8CC0;
    private const string RkpdMagic = "RKPD";
    private const int MaxFriendNum = 30;
    private const int FriendDataOffset = 0x56D0;
    private const int FriendDataSize = 0x1C0;
    private const int MiiSize = 0x4A;
    
    public User getCurrentUser => Instance.GameData.Users[Instance.GameData.CurrentUserIndex];
    public string getCurrentUsername => Instance.GameData.Users[Instance.GameData.CurrentUserIndex].MiiData.mii.Name;
    public string getCurrentFriendCode => Instance.GameData.Users[Instance.GameData.CurrentUserIndex].FriendCode;
    public uint getCurrentVr => Instance.GameData.Users[Instance.GameData.CurrentUserIndex].Vr;
    public uint getCurrentBr => Instance.GameData.Users[Instance.GameData.CurrentUserIndex].Br;
    public uint getCurrentTotalRaceCount => Instance.GameData.Users[Instance.GameData.CurrentUserIndex].TotalRaceCount;
    public uint getCurrentTotalWinCount => Instance.GameData.Users[Instance.GameData.CurrentUserIndex].TotalWinCount;
    public List<Friend> getCurrentFriends => Instance.GameData.Users[Instance.GameData.CurrentUserIndex].Friends;
    public MiiData getCurrentMiiData => Instance.GameData.Users[Instance.GameData.CurrentUserIndex].MiiData;
    public bool isCurrentUserOnline => Instance.GameData.Users[Instance.GameData.CurrentUserIndex].IsOnline;
    public Models.GameData.GameData getGameData => Instance.GameData;
    public User GetCurrentUser => GameData.Users[GameData.CurrentUserIndex];
    public List<User> GetAllUsers => GameData.Users;
    public User GetUserData(int index) => GameData.Users[index];

    
    private GameDataLoader()
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
        _saveData = LoadSaveDataFile();
        if (_saveData != null && ValidateMagicNumber())
        {
            ParseUsers();
            return;
        }
        CreateDummyUser();
    }
    
    private void CreateDummyUser()
    {
        var dummyUser = new User
        {
            FriendCode = "0000-0000-0000",
            MiiData = new MiiData
            {
                mii = new Mii
                {
                    Name = "Not Logged In",
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
            IsOnline = false
        };
        GameData.Users.Add(dummyUser);
    }

    private void ParseUsers()
    {
        for (var i = 0; i < MaxPlayerNum; i++)
        {
            var rkpdOffset = RksysMagic.Length + i * RkpdSize;
            if (Encoding.ASCII.GetString(_saveData, rkpdOffset, RkpdMagic.Length) != RkpdMagic) continue;
            var user = ParseUser(rkpdOffset);
            GameData.Users.Add(user);
        }
        
        if (GameData.Users.Count == 0)
            CreateDummyUser();
    }

    private User ParseUser(int offset)
    {
        var user = new User
        {
            MiiData = ParseMiiData(offset + 0x14),
            FriendCode = FriendCodeGenerator.GetFriendCode(_saveData, offset + 0x5C),
            Vr = BigEdianBinaryReader.BufferToUint16(_saveData, offset + 0xB0),
            Br = BigEdianBinaryReader.BufferToUint16(_saveData, offset + 0xB2),
            TotalRaceCount = BigEdianBinaryReader.BufferToUint32(_saveData, offset + 0xB4),
            TotalWinCount = BigEdianBinaryReader.BufferToUint32(_saveData, offset + 0xDC),
        };
        
        ParseFriends(user, offset);
        return user;
    }
    
    private MiiData ParseMiiData(int offset)
    {
        var miiData =  new MiiData
        {
            mii = new Mii
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
            for (int i = 0; i < 4; i++)
                AvatarId |= (uint)(mii[0x18 + i] << (8 * i));
            
            if (AvatarId == ClientId)
                return mii;
        }
        return Array.Empty<byte>();
    }
    
         
    public class MiiData
    {
        public Mii mii { get; set; }
        public uint AvatarId { get; set; }
        public uint ClientId { get; set; }

    }

    private void ParseFriends(User user, int userOffset)
    {
        var friendOffset = userOffset + FriendDataOffset;
        var onlinePlayers = new List<Player>();
        onlinePlayers = RRLiveRooms.Instance.CurrentRooms.SelectMany(room => room.Players.Values).ToList();
        for (var i = 0; i < MaxFriendNum; i++)
        {
            var currentOffset = friendOffset + i * FriendDataSize;
            if (!CheckMiiData(currentOffset + 0x1A)) continue;
            var friend = new Friend
            {
                Vr = 0,
                Br = 0,
                // Name = BigEdianBinaryReader.GetUtf16String(_saveData, currentOffset + 0x1C, 10),
                FriendCode = FriendCodeGenerator.GetFriendCode(_saveData, currentOffset + 4),
                Wins = BitConverter.ToUInt16(_saveData, currentOffset + 0x14),
                Losses = BitConverter.ToUInt16(_saveData, currentOffset + 0x12),
                // MiiBinaryData = Convert.ToBase64String(_saveData.AsSpan(currentOffset + 0x1A, MiiSize)),
                MiiData = new MiiData
                {
                    mii = new Mii
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
            if (_saveData[offset + i] != 0)
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
        var saveFile = Directory.GetFiles(SaveFilePath, "rksys.dat", SearchOption.AllDirectories);
        return saveFile.Length == 0 ? null : File.ReadAllBytes(saveFile[0]);
    }
}
