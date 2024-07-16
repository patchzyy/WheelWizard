namespace CT_MKWII_WPF.Utils;
using System;
using System.Security.Cryptography;
using System.Text;

public class FriendCodeGenerator
{
    public static string GetFriendCode(byte[] data, int offset)
    {
        uint pid = BitConverter.ToUInt32(data, offset);
        if (pid == 0) return string.Empty;

        byte[] srcBuf = new byte[8];
        srcBuf[0] = data[offset + 3];
        srcBuf[1] = data[offset + 2];
        srcBuf[2] = data[offset + 1];
        srcBuf[3] = data[offset + 0];
        srcBuf[4] = 0x4A;
        srcBuf[5] = 0x43;
        srcBuf[6] = 0x4D;
        srcBuf[7] = 0x52;

        string md5Hash = GetMd5Hash(srcBuf);
        ulong fcDec = ((Convert.ToUInt64(md5Hash.Substring(0, 2), 16) >> 1) * (1UL << 32)) + pid;

        return FormatFriendCode(fcDec);
    }

    private static string GetMd5Hash(byte[] input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(input);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }

    private static string FormatFriendCode(ulong fcDec)
    {
        string fc = "";
        for (int i = 0; i < 3; i++)
        {
            fc += FcPartParse((int)(fcDec / Math.Pow(10, 4 * (2 - i)) % 10000));
            if (i < 2) fc += "-";
        }
        return fc;
    }

    private static string FcPartParse(int part)
    {
        return part.ToString("D4");
    }
}