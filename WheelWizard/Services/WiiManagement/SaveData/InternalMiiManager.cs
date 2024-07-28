using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CT_MKWII_WPF.Services.WiiManagement.GameData;

public static class InternalMiiManager
{
    private const string MiiDbFileName= "RFL_DB.dat";
    private static readonly string WiiDbFile = Path.Combine(PathManager.WiiFolderPath, "shared2", "menu", "FaceLib", "RFL_DB.dat");
    private const int MiiLength = 74;
    private const int MiiNameLength = 10;
    private static readonly byte[] emptyMii = new byte[MiiLength]; // empty mii template
    
    public static List<byte[]> GetAllMiiData()
    {
        var miis = new List<byte[]>();
        var allMiiData = GetMiiDb();
        if (allMiiData.Length == 0) return miis;
        using (var memoryStream = new MemoryStream(allMiiData))
        {
            memoryStream.Seek(0x4, SeekOrigin.Begin); //plaza offset
            for (int i = 0; i < 100; i++) //100 mii limit
            {
                byte[] miiData = new byte[MiiLength];
                int bytesRead = memoryStream.Read(miiData, 0, MiiLength);
                if ((bytesRead == 0) || miiData.SequenceEqual(emptyMii))
                {
                    break;
                }
                miis.Add(miiData);
            }
        }
        return miis;
    }
    

    public static byte[] GetMiiDb()
    {
        if (!File.Exists(WiiDbFile))
        {
            return Array.Empty<byte>();
        }

        return File.ReadAllBytes(WiiDbFile);
    }
    
}
