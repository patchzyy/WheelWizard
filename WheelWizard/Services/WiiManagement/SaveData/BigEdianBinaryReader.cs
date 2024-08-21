using System.Collections.Generic;
using System.Text;

namespace CT_MKWII_WPF.Services.WiiManagement.SaveData;

public static class BigEdianBinaryReader
{
    //Helper functions to convert a buffer to an uint using big endian
    public static uint BufferToUint32(byte[] data, int offset)
    {
        return (uint)((data[offset] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3]);
    }
    
    public static uint BufferToUint16(byte[] data, int offset)
    {
        return (uint)((data[offset] << 8) | data[offset + 1]);
    }
    
    //big endian get the string
    public static string GetUtf16String(byte[] data, int offset, int maxLength)
    {
        var bytes = new List<byte>();
        for (var i = 0; i < maxLength * 2; i += 2)
        {
            var b1 = data[offset + i];
            var b2 = data[offset + i + 1];
            if (b1 == 0 && b2 == 0) break;
            bytes.Add(b1);
            bytes.Add(b2);
        }
        return Encoding.BigEndianUnicode.GetString(bytes.ToArray());
    }
}
