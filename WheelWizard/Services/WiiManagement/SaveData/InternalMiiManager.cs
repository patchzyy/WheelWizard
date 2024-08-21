using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CT_MKWII_WPF.Services.WiiManagement.SaveData;

public static class InternalMiiManager
{
    private const string MiiDbFileName= "RFL_DB.dat";
    private static readonly string WiiDbFile = Path.Combine(PathManager.WiiFolderPath, "shared2", "menu", "FaceLib", "RFL_DB.dat");
    private const int MiiLength = 74;
    private const int MiiNameLength = 10;
    private static readonly byte[] emptyMii = new byte[MiiLength];
    
    public static List<byte[]> GetAllMiiData()
    {
        var miis = new List<byte[]>();
        var allMiiData = GetMiiDb();
        if (allMiiData.Length == 0) return miis;
        using (var memoryStream = new MemoryStream(allMiiData))
        {
            memoryStream.Seek(0x4, SeekOrigin.Begin); //plaza offset
            for (var i = 0; i < 100; i++) //100 mii limit
            {
                var miiData = new byte[MiiLength];
                var bytesRead = memoryStream.Read(miiData, 0, MiiLength);
                if ((bytesRead == 0) || miiData.SequenceEqual(emptyMii))
                    break;
                miis.Add(miiData);
            }
        }
        return miis;
    }

    private static byte[] GetMiiDb()
    {
        return !File.Exists(WiiDbFile) ? Array.Empty<byte>() : File.ReadAllBytes(WiiDbFile);
    }
}
