using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CT_MKWII_WPF.Classes;

namespace CT_MKWII_WPF.Utils;

public class GameDataLoader
{
    private GameData _gameData;
    private byte[] _saveData;
    
    public GameData GameData => _gameData;
    
    private const int RKSYS_SIZE = 0x2BC000;
    private const string RKSYS_MAGIC = "RKSD0006";
    private const int MAX_PLAYER_NUM = 4;
    private const int RKPD_SIZE = 0x8CC0;
    private const string RKPD_MAGIC = "RKPD";
    private const int MAX_FRIEND_NUM = 30;
    private const int FRIEND_DATA_OFFSET = 0x56D0;
    private const int FRIEND_DATA_SIZE = 0x1C0;
    private const int MII_SIZE = 0x4A;
    
    
    public GameDataLoader()
    {
        _gameData = new GameData();
    }
    
    
    public void LoadGameData()
    {
        _saveData = LoadSaveDataFile();
        if (_saveData == null || _saveData.Length != RKSYS_SIZE || !ValidateMagicNumber())
        {
            throw new InvalidDataException("Invalid save file data");
        }
        ParseUsers();
    }
    
    private void ParseUsers()
    {
        for (int i = 0; i < MAX_PLAYER_NUM; i++)
        {
            int rkpdOffset = RKSYS_MAGIC.Length + i * RKPD_SIZE;
            if (Encoding.ASCII.GetString(_saveData, rkpdOffset, RKPD_MAGIC.Length) == RKPD_MAGIC)
            {
                User user = ParseUser(rkpdOffset);
                _gameData.Users.Add(user);
            }
        }
    }
    
    private User ParseUser(int offset)
    {
        User user = new User
        {
            Name = GetUTF16String(offset + 0x14, 10),
            FriendCode = FriendCodeGenerator.GetFriendCode(_saveData,offset + 0x5C),
            MiiData = Convert.ToBase64String(_saveData.AsSpan(offset + 0x4C, MII_SIZE)),
            VR = BitConverter.ToUInt16(_saveData, offset + 0xB0),
            BR = BitConverter.ToUInt16(_saveData, offset + 0xB2),
            TotalRaceCount = BitConverter.ToInt32(_saveData, offset + 0xB4)
        };

        ParseFriends(user, offset);
        return user;
    }
    
    private void ParseFriends(User user, int userOffset)
    {
        int friendOffset = userOffset + FRIEND_DATA_OFFSET;
        for (int i = 0; i < MAX_FRIEND_NUM; i++)
        {
            int currentOffset = friendOffset + i * FRIEND_DATA_SIZE;
            if (CheckMiiData(currentOffset + 0x1A))
            {
                Friend friend = new Friend
                {
                    Name = GetUTF16String(currentOffset + 0x1C, 10),
                    FriendCode = FriendCodeGenerator.GetFriendCode(_saveData, currentOffset + 4),
                    Wins = BitConverter.ToUInt16(_saveData, currentOffset + 0x14),
                    Losses = BitConverter.ToUInt16(_saveData, currentOffset + 0x12),
                    MiiData = Convert.ToBase64String(_saveData.AsSpan(currentOffset + 0x1A, MII_SIZE))
                };
                user.Friends.Add(friend);
            }
        }
    }
    
    private bool CheckMiiData(int offset)
    {
        for (int i = 0; i < MII_SIZE; i++)
        {
            if (_saveData[offset + i] != 0) return true;
        }
        return false;
    }
    
    private string GetUTF16String(int offset, int maxLength)
    {
        List<byte> bytes = new List<byte>();
        for (int i = 0; i < maxLength * 2; i += 2)
        {
            byte b1 = _saveData[offset + i];
            byte b2 = _saveData[offset + i + 1];
            if (b1 == 0 && b2 == 0) break;
            bytes.Add(b1);
            bytes.Add(b2);
        }
        return Encoding.BigEndianUnicode.GetString(bytes.ToArray());
    }
    
    private string GetFriendCode(int offset)
    {
        // This is a simplified version. The actual friend code generation is more complex.
        uint pid = BitConverter.ToUInt32(_saveData, offset);
        return pid.ToString("D12").Insert(4, "-").Insert(9, "-");
    }

    
    private bool ValidateMagicNumber()
    {
        return Encoding.ASCII.GetString(_saveData, 0, RKSYS_MAGIC.Length) == RKSYS_MAGIC;
    }

    public static byte[] LoadSaveDataFile()
    {
        var saveFileLocation = Path.Combine(SettingsUtils.GetLoadPathLocation(), "Riivolution", "RetroRewind6", "save");
        var SaveFile = Directory.GetFiles(saveFileLocation, "rksys.dat", SearchOption.AllDirectories);
        if (SaveFile.Length == 0)
        {
            return null;
        }
        return File.ReadAllBytes(SaveFile[0]);
    }
}