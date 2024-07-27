using System;
using System.Security.Cryptography;
using System.Text;

namespace CT_MKWII_WPF.Utilities.Generators;

public class FriendCodeGenerator
{
    
    public static uint BufferToUint32(byte[] data, int offset)
    {
        return (uint)((data[offset] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3]);
    }
    public static string GetFriendCode(byte[] data, int offset)
    {
        var pid = BufferToUint32(data, offset);
        if (pid == 0) return string.Empty;
        
        var srcBuf = new byte[]
        {
            data[offset + 3],
            data[offset + 2],
            data[offset + 1],
            data[offset + 0], 
            0x4A, 0x43, 0x4D, 0x52
        };
        var md5Hash = GetMd5Hash(srcBuf);
        var fcDec = ((Convert.ToUInt32(md5Hash.Substring(0, 2), 16) >> 1) * (1UL << 32)) + pid;

        return FormatFriendCode(fcDec);
    }

    private static string GetMd5Hash(byte[] input)
    {
        using (var md5 = MD5.Create())
        {
            var hashBytes = md5.ComputeHash(input);
            var sb = new StringBuilder();
            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }

    private static string FormatFriendCode(ulong fcDec)
    {
        var fc = "";
        for (var i = 0; i < 3; i++)
        {
            fc += FcPartParse((int)(fcDec / Math.Pow(10, 4 * (2 - i)) % 10000));
            if (i < 2) fc += "-";
        }

        return fc;
    }

    private static string FcPartParse(int part) => part.ToString("D4");
}
