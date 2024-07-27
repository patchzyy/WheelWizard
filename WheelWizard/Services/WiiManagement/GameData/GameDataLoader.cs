using CT_MKWII_WPF.Models.GameData;
using CT_MKWII_WPF.Utilities.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

//big big thanks to https://kazuki-4ys.github.io/web_apps/FaceThief/ for the JS implementation of all of this
//Things to keep in mind when working with the rksys.dat file:
// everything is big endian!!!!
// the save file is 0x2BC000 bytes long

namespace CT_MKWII_WPF.Services.WiiManagement.GameData;

public class GameDataLoader
{
    private static string SaveFilePath => Path.Combine(PathManager.RiivolutionWhWzFolderPath, "riivolution", "save");
    private byte[] _saveData;

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
    
    public GameDataLoader()
    {
        GameData = new Models.GameData.GameData();
    }
    
    public static uint BufferToUint32(byte[] data, int offset)
    {
        return (uint)((data[offset] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3]);
    }
    
    public static uint BufferToUint16(byte[] data, int offset)
    {
        return (uint)((data[offset] << 8) | data[offset + 1]);
    }
    
    public void LoadGameData()
    {
        _saveData = LoadSaveDataFile();
        if (_saveData is not { Length: RksysSize } || !ValidateMagicNumber())
            throw new InvalidDataException("Invalid save file data");
        ParseUsers();
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
    }

    private User ParseUser(int offset)
    {
        var user = new User
        {
            Name = GetUtf16String(offset + 0x14, 10),
            FriendCode = FriendCodeGenerator.GetFriendCode(_saveData, offset + 0x5C),
            MiiData = ParseMiiData(offset + 0x6C),
            Vr = BufferToUint16(_saveData, offset + 0xB0),
            Br = BufferToUint16(_saveData, offset + 0xB2),
            TotalRaceCount = BitConverter.ToInt32(_saveData, offset + 0xB4)
        };

        ParseFriends(user, offset);
        return user;
    }
    
    private MiiData ParseMiiData(int offset)
    {
        var miiData = new MiiData
        {
            AvatarId = BitConverter.ToUInt32(_saveData, offset),
            ClientId = BitConverter.ToUInt32(_saveData, offset + 4),
        };
        byte[] miiBytes = new byte[MiiSize];
        Buffer.BlockCopy(_saveData, offset + 8, miiBytes, 0, MiiSize);
        miiData.Base64MiiData = Convert.ToBase64String(miiBytes);
        return miiData;
    }
        
    public class MiiData
    {
        public uint AvatarId { get; set; }
        public uint ClientId { get; set; }
        
        public string Base64MiiData { get; set; }

        public Byte[] GetMiiBytes()
        {
            return Convert.FromBase64String(Base64MiiData);
        }
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
                Name = GetUtf16String(currentOffset + 0x1C, 10),
                FriendCode = FriendCodeGenerator.GetFriendCode(_saveData, currentOffset + 4),
                Wins = BitConverter.ToUInt16(_saveData, currentOffset + 0x14),
                Losses = BitConverter.ToUInt16(_saveData, currentOffset + 0x12),
                MiiData = Convert.ToBase64String(_saveData.AsSpan(currentOffset + 0x1A, MiiSize))
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

    private string GetUtf16String(int offset, int maxLength)
    {
        var bytes = new List<byte>();
        for (var i = 0; i < maxLength * 2; i += 2)
        {
            var b1 = _saveData[offset + i];
            var b2 = _saveData[offset + i + 1];
            if (b1 == 0 && b2 == 0) break;
            bytes.Add(b1);
            bytes.Add(b2);
        }

        return Encoding.BigEndianUnicode.GetString(bytes.ToArray());
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
