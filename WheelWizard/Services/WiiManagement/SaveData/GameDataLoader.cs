using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WheelWizard.Models.Enums;
using WheelWizard.Models.GameData;
using WheelWizard.Models.MiiImages;
using WheelWizard.Services.LiveData;
using WheelWizard.Services.Other;
using WheelWizard.Services.Settings;
using WheelWizard.Utilities.Generators;
using WheelWizard.Utilities.RepeatedTasks;
using WheelWizard.Views.Popups.Generic;

// big big thanks to https://kazuki-4ys.github.io/web_apps/FaceThief/ for the JS implementation

namespace WheelWizard.Services.WiiManagement.SaveData;

public class GameDataLoader : RepeatedTaskManager
{
    public static GameDataLoader Instance { get; } = new();

    /// <summary>
    /// The path to where the “rksys.dat” folder structure is expected to live, e.g.
    ///   ..\path\to\Riivolution\riivolution\save\RetroWFC\RMCP\rksys.dat
    /// </summary>
    private static string SaveFilePath
    {
        get
        {
            var path = Path.Combine(PathManager.RiivolutionWhWzFolderPath, "riivolution", "save", "RetroWFC");
            if (Directory.Exists(path)) 
                return path;

            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                // Display an error if we cannot create the directory.
                new MessageBoxWindow().SetMainText($"Error creating save directory: {ex.Message}").Show();
            }
            return path;
        }
    }

    private byte[]? _saveData;

    private GameData GameData { get; }

    private const int RksysSize   = 0x2BC000;
    private const string RksysMagic = "RKSD0006";
    private const int MaxPlayerNum = 4;
    private const int RkpdSize = 0x8CC0;
    private const string RkpdMagic = "RKPD";
    private const int MaxFriendNum = 30;
    private const int FriendDataOffset = 0x56D0;
    private const int FriendDataSize = 0x1C0;
    private const int MiiSize = 0x4A;

    /// <summary>
    /// Returns the "focused" or currently active license/user as determined by the Settings.
    /// </summary>
    public GameDataUser GetCurrentUser 
        => Instance.GameData.Users[(int)SettingsManager.FOCUSSED_USER.Get()];

    public List<GameDataFriend> GetCurrentFriends 
        => Instance.GameData.Users[(int)SettingsManager.FOCUSSED_USER.Get()].Friends;

    public Models.GameData.GameData GetGameData 
        => Instance.GameData;

    public GameDataUser GetUserData(int index) 
        => GameData.Users[index];

    public bool HasAnyValidUsers 
        => GameData.Users.Any(user => user.FriendCode != "0000-0000-0000");

    private GameDataLoader() : base(40)
    {
        GameData = new Models.GameData.GameData();
        LoadGameData();
    }

    /// <summary>
    /// Refresh the "IsOnline" status of our local users based on the list of currently online players.
    /// </summary>
    public void RefreshOnlineStatus()
    {
        var currentRooms = RRLiveRooms.Instance.CurrentRooms;
        var onlinePlayers = currentRooms.SelectMany(room => room.Players.Values).ToList();
        foreach (var user in GameData.Users)
        {
            user.IsOnline = onlinePlayers.Any(player => player.Fc == user.FriendCode);
        }
    }

    /// <summary>
    /// Loads the entire rksys.dat file from disk into memory and parses the 4 possible licenses.
    /// If the file is invalid or not found, we create dummy users.
    /// </summary>
    public void LoadGameData()
    {
        try
        {
            _saveData = LoadSaveDataFile();
            if (_saveData != null && ValidateMagicNumber())
            {
                ParseUsers();
                Console.WriteLine("[LoadGameData] rksys.dat loaded and parsed successfully.");
                return;
            }

            // If the file was invalid or not found, create 4 dummy licenses
            GameData.Users.Clear();
            for (var i = 0; i < MaxPlayerNum; i++)
            {
                CreateDummyUser();
            }
            Console.WriteLine("[LoadGameData] No valid rksys.dat found. Created dummy users.");
        }
        catch (Exception e)
        {
            new MessageBoxWindow()
                .SetMainText($"An error occurred while loading the game data: {e.Message}")
                .Show();
        }
    }

    private void CreateDummyUser()
    {
        var dummyUser = new GameDataUser
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
            Friends = new List<GameDataFriend>(),
            RegionId = 10, // 10 => “unknown”
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

    private GameDataUser ParseUser(int offset)
    {
        if (_saveData == null) throw new ArgumentNullException(nameof(_saveData));

        var user = new GameDataUser
        {
            MiiData     = ParseMiiData(offset + 0x14),
            FriendCode  = FriendCodeGenerator.GetFriendCode(_saveData, offset + 0x5C),
            Vr          = BigEndianBinaryReader.BufferToUint16(_saveData, offset + 0xB0),
            Br          = BigEndianBinaryReader.BufferToUint16(_saveData, offset + 0xB2),
            TotalRaceCount = BigEndianBinaryReader.BufferToUint32(_saveData, offset + 0xB4),
            TotalWinCount   = BigEndianBinaryReader.BufferToUint32(_saveData, offset + 0xDC),

            // Region is often found near offset 0x23308 + 0x3802 in RKGD. This code is a partial guess.
            // In practice, region might be read differently depending on your rksys layout.
            RegionId = (BigEndianBinaryReader.BufferToUint16(_saveData, 0x23308 + 0x3802) / 4096),
        };

        ParseFriends(user, offset);
        return user;
    }

    private MiiData ParseMiiData(int offset)
    {
        if (_saveData == null) throw new ArgumentNullException(nameof(_saveData));

        // In Mario Kart Wii's rksys, offset +0x10 => AvatarId, offset +0x14 => ClientId
        // The name is big-endian UTF-16 at offset itself (length 10 chars => 20 bytes).
        var name = BigEndianBinaryReader.GetUtf16String(_saveData, offset, 10);
        var avatarId = BitConverter.ToUInt32(_saveData, offset + 0x10);
        var clientId = BitConverter.ToUInt32(_saveData, offset + 0x14);

        // Convert the Mii block from RFL_DB if it’s actually valid
        byte[] rawMii = InternalMiiManager.GetMiiDataByClientId(clientId);

        var miiData = new MiiData
        {
            Mii = new Mii
            {
                Name = name,
                Data = Convert.ToBase64String(rawMii)
            },
            AvatarId = avatarId,
            ClientId = clientId
        };
        return miiData;
    }

    private void ParseFriends(GameDataUser gameDataUser, int userOffset)
    {
        if (_saveData == null) return;

        var friendOffset = userOffset + FriendDataOffset;
        for (var i = 0; i < MaxFriendNum; i++)
        {
            var currentOffset = friendOffset + i * FriendDataSize;
            if (!CheckMiiData(currentOffset + 0x1A)) continue;

            var friend = new GameDataFriend
            {
                Vr          = BigEndianBinaryReader.BufferToUint16(_saveData, currentOffset + 0x16),
                Br          = BigEndianBinaryReader.BufferToUint16(_saveData, currentOffset + 0x18),
                FriendCode  = FriendCodeGenerator.GetFriendCode(_saveData, currentOffset + 4),
                Wins        = BigEndianBinaryReader.BufferToUint16(_saveData, currentOffset + 0x14),
                Losses      = BigEndianBinaryReader.BufferToUint16(_saveData, currentOffset + 0x12),
                CountryCode = _saveData[currentOffset + 0x68],
                RegionId    = _saveData[currentOffset + 0x69],

                MiiData = new MiiData
                {
                    Mii = new Mii
                    {
                        Name = BigEndianBinaryReader.GetUtf16String(_saveData, currentOffset + 0x1C, 10),
                        Data = Convert.ToBase64String(_saveData.AsSpan(currentOffset + 0x1A, MiiSize))
                    },
                    AvatarId = 0,
                    ClientId = 0
                },
            };
            gameDataUser.Friends.Add(friend);
        }
    }

    private bool CheckMiiData(int offset)
    {
        // If the entire 0x4A bytes are zero, we treat it as empty / no Mii data
        for (var i = 0; i < MiiSize; i++)
        {
            if (_saveData != null && _saveData[offset + i] != 0)
                return true;
        }
        return false;
    }

    private bool ValidateMagicNumber()
    {
        if (_saveData == null) return false;
        return Encoding.ASCII.GetString(_saveData, 0, RksysMagic.Length) == RksysMagic;
    }

    private static byte[]? LoadSaveDataFile()
    {
        try
        {
            if (!Directory.Exists(SaveFilePath))
                return null;

            var currentRegion = (MarioKartWiiEnums.Regions)SettingsManager.RR_REGION.Get();
            if (currentRegion == MarioKartWiiEnums.Regions.None)
            {
                // Double check if there's at least one valid region
                var validRegions = RRRegionManager.GetValidRegions();
                if (validRegions.First() != MarioKartWiiEnums.Regions.None)
                {
                    currentRegion = validRegions.First();
                    SettingsManager.RR_REGION.Set(currentRegion);
                }
                else
                {
                    return null;
                }
            }

            var saveFileFolder = Path.Combine(SaveFilePath, RRRegionManager.ConvertRegionToGameID(currentRegion));
            var saveFile = Directory.GetFiles(saveFileFolder, "rksys.dat", SearchOption.TopDirectoryOnly);
            return (saveFile.Length == 0) ? null : File.ReadAllBytes(saveFile[0]);
        }
        catch
        {
            // If anything fails, return null
            return null;
        }
    }

    /// <summary>
    /// Calculates the CRC32 of the specified slice of bytes using the
    /// standard polynomial (0xEDB88320) in the same way MKWii does.
    /// </summary>
    public static uint ComputeCrc32(byte[] data, int offset, int length)
    {
        const uint POLY = 0xEDB88320;
        uint crc = 0xFFFFFFFF;

        for (int i = offset; i < offset + length; i++)
        {
            byte b = data[i];
            crc ^= b;
            for (int j = 0; j < 8; j++)
            {
                if ((crc & 1) != 0)
                    crc = (crc >> 1) ^ POLY;
                else
                    crc >>= 1;
            }
        }

        return ~crc;
    }
    

    /// <summary>
    /// Fixes the MKWii save file by recalculating and inserting the CRC32 at 0x27FFC.
    /// </summary>
    public static void FixRksysCrc(byte[] rksysData)
    {
        if (rksysData == null || rksysData.Length < RksysSize)
            throw new ArgumentException("Invalid rksys.dat data");

        // 1) Compute CRC over [0 .. 0x27FFB].
        int lengthToCrc = 0x27FFC; // 0x27FFC is exclusive (the CRC bytes themselves)
        uint newCrc = ComputeCrc32(rksysData, 0, lengthToCrc);

        // 2) Write CRC at offset 0x27FFC in big-endian.
        BigEndianBinaryReader.WriteUInt32BigEndian(rksysData, 0x27FFC, newCrc);
    }

    // ------------------------------------------------------------------
    //  NEW / MODIFIED CODE FOR “NO NAME” → GENERATE A REAL MII
    // ------------------------------------------------------------------

    /// <summary>
    /// Prompts the user to change the name of a selected license. If the license
    /// is “no name” or has no real Mii data, we first generate a Mii in RFL_DB.dat,
    /// link it, and then allow naming.
    /// </summary>
    /// <param name="userIndex">Which license slot (0–3) to rename.</param>
    public async void PromptLicenseNameChange(int userIndex)
    {
        // Validate user index
        if (userIndex < 0 || userIndex >= MaxPlayerNum)
        {
            new MessageBoxWindow()
                .SetMainText("Invalid license index. Please select a valid license.")
                .Show();
            return;
        }

        // Grab the license data from memory
        var user = GameData.Users[userIndex];
        Console.WriteLine($"[PromptLicenseNameChange] Selected license slot: {userIndex}, current name: '{user.MiiData?.Mii?.Name}'");

        // If it’s a dummy license with friend code "0000-0000-0000", we also check if it’s truly incomplete.
        // The user specifically said: "Don't rely on friend code for that check." So we skip that.
        // Instead, check if the Mii is "no name" or if the raw data is empty.
        bool miiIsEmptyOrNoName = IsNoNameOrEmptyMii(user);

        if (miiIsEmptyOrNoName)
        {
            Console.WriteLine("[PromptLicenseNameChange] Detected an empty/no-name Mii. Generating new Mii in the RFL_DB...");

            // 1) Generate a brand new Mii block in RFL_DB with some placeholder name (we’ll rename next).
            //    This also gives us a brand-new ClientId.
            uint newClientId = InternalMiiManager.CreateNewMiiInDb("TempMii");

            // 2) Update the user’s license structure to reference this new Mii
            user.MiiData.Mii.Data = Convert.ToBase64String(InternalMiiManager.GetMiiDataByClientId(newClientId));
            user.MiiData.Mii.Name = "TempMii";   // Next step is renaming anyway.
            user.MiiData.ClientId = newClientId;
            user.MiiData.AvatarId = newClientId; // Typically the same in MKWii saves.

            // Write these new IDs into the rksys.dat local buffer
            WriteLicenseMiiIdsToSaveData(userIndex, newClientId, newClientId);
            
            // Save to disk so our new Mii reference is recognized
            SaveRksysToFile();

            Console.WriteLine($"[PromptLicenseNameChange] Created new Mii with clientId=0x{newClientId:X8} and updated license slot {userIndex}.");
        }
        else
        {
            Console.WriteLine("[PromptLicenseNameChange] License already has a valid Mii; proceeding to rename.");
        }

        // Now do the normal rename flow:

        // Show a popup to get the new name
        var currentName = user.MiiData.Mii.Name ?? "";
        var renamePopup = new TextInputWindow()
            .setLabelText($"Enter new name for {currentName}:");

        renamePopup.PopulateText(currentName);

        var newName = await renamePopup.ShowDialog();
        if (string.IsNullOrWhiteSpace(newName)) return;

        // Basic checks
        if (newName.Length < 3)
        {
            new MessageBoxWindow()
                .SetMainText("Names must be at least 3 characters long.")
                .Show();
            return;
        }
        if (!newName.All(char.IsLetterOrDigit))
        {
            new MessageBoxWindow()
                .SetMainText("Names can only contain letters and numbers.")
                .Show();
            return;
        }

        // Trim to max 10 chars for MKWii license
        if (newName.Length > 10)
            newName = newName.Substring(0, 10);

        // Update in-memory license data
        user.MiiData.Mii.Name = newName;

        // Safely update the big-endian UTF-16 name in the local _saveData buffer
        WriteLicenseNameToSaveData(userIndex, newName);

        // Also update the Mii name in the RFL_DB (using the saved ClientId)
        bool updated = InternalMiiManager.UpdateMiiName(user.MiiData.ClientId, newName);
        Console.WriteLine(updated
            ? $"[PromptLicenseNameChange] Mii name updated in RFL_DB for clientId=0x{user.MiiData.ClientId:X8}."
            : "[PromptLicenseNameChange] Could not update Mii name in RFL_DB (clientId not found?).");

        // Finally, write the updated _saveData back to rksys.dat
        SaveRksysToFile();

        Console.WriteLine($"[PromptLicenseNameChange] License rename finished. New name: {newName}");
    }

    /// <summary>
    /// Checks if the given GameDataUser has a “no name” or effectively empty Mii data.
    /// </summary>
    private bool IsNoNameOrEmptyMii(GameDataUser user)
    {
        if (user?.MiiData?.Mii == null)
            return true;

        string name = user.MiiData.Mii.Name?.Trim() ?? "";
        if (name.Equals("no name", StringComparison.OrdinalIgnoreCase))
            return true;

        // Also consider the base64 Mii data (74 bytes). If empty or all zeros, treat as no Mii.
        var raw = Convert.FromBase64String(user.MiiData.Mii.Data ?? "");
        if (raw.Length != 74) return true; // Not valid
        if (raw.All(b => b == 0)) return true;

        // Otherwise, it’s presumably valid
        return false;
    }

    /// <summary>
    /// Writes a new UTF-16 big-endian name for a license into our <c>_saveData</c> buffer
    /// at offset:  0x8 + userIndex*0x8CC0 + 0x14
    /// (within the "RKPD" block).
    /// </summary>
    private void WriteLicenseNameToSaveData(int userIndex, string newName)
    {
        if (_saveData == null || _saveData.Length < RksysSize) return;

        var rkpdOffset = 0x8 + userIndex * RkpdSize; 
        var nameOffset = rkpdOffset + 0x14;

        // Convert the new name to big-endian UTF-16 (2 bytes per char)
        var nameBytes = Encoding.BigEndianUnicode.GetBytes(newName);

        // Clear out existing 20 bytes for the name area
        for (int i = 0; i < 20; i++)
            _saveData[nameOffset + i] = 0;

        // Copy new name bytes (up to 20)
        Array.Copy(nameBytes, 0, _saveData, nameOffset, Math.Min(nameBytes.Length, 20));
    }

    /// <summary>
    /// Writes the specified AvatarId and ClientId into the license’s Mii data area 
    /// </summary>
    private void WriteLicenseMiiIdsToSaveData(int userIndex, uint avatarId, uint clientId)
    {
        if (_saveData == null || _saveData.Length < RksysSize) return;
        var rkpdOffset = 0x8 + userIndex * RkpdSize;
        int avatarIdOffset = rkpdOffset + 0x28;
        int clientIdOffset = rkpdOffset + 0x2C;
        BigEndianBinaryReader.WriteUInt32BigEndian(_saveData, avatarIdOffset, avatarId);
        BigEndianBinaryReader.WriteUInt32BigEndian(_saveData, clientIdOffset, clientId);
    }

    /// <summary>
    /// Saves the current <c>_saveData</c> buffer back to disk as “rksys.dat”.
    /// This ensures any modifications become persistent.
    /// </summary>
    private void SaveRksysToFile()
    {
        if (_saveData == null) return;

        // Recalculate CRC
        FixRksysCrc(_saveData);

        // Determine folder for the region
        var currentRegion = (MarioKartWiiEnums.Regions)SettingsManager.RR_REGION.Get();
        var saveFolder = Path.Combine(SaveFilePath, RRRegionManager.ConvertRegionToGameID(currentRegion));

        try
        {
            Directory.CreateDirectory(saveFolder);
            var path = Path.Combine(saveFolder, "rksys.dat");
            File.WriteAllBytes(path, _saveData);

            Console.WriteLine("[SaveRksysToFile] Successfully wrote updated rksys.dat.");
        }
        catch (Exception ex)
        {
            new MessageBoxWindow()
                .SetMainText($"Failed to save rksys.dat.\n{ex.Message}")
                .Show();
        }
    }

    protected override Task ExecuteTaskAsync()
    {
        // This gets periodically called by RepeatedTaskManager base class
        LoadGameData();
        return Task.CompletedTask;
    }
}
