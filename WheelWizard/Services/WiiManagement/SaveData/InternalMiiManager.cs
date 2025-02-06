using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WheelWizard.Services.WiiManagement.SaveData
{
    public static class InternalMiiManager
    {
        private static readonly string WiiDbFile = Path.Combine(
            PathManager.WiiFolderPath,
            "shared2",
            "menu",
            "FaceLib",
            "RFL_DB.dat");

        private const int MiiLength = 74;
        private const int MiiNameLength = 10;
        private static readonly byte[] emptyMii = new byte[MiiLength];

        /// <summary>
        /// Reads the entire RFL_DB.dat and returns up to 100 Mii blocks (74 bytes each).
        /// </summary>
        public static List<byte[]> GetAllMiiData()
        {
            var miis = new List<byte[]>();
            var allMiiData = GetMiiDb();
            if (allMiiData.Length == 0)
                return miis;

            // According to the official structure, the first 4 bytes are a "Plaza Offset".
            // We skip those 4 and read up to 100 Mii entries (74 bytes each).
            using var memoryStream = new MemoryStream(allMiiData);
            memoryStream.Seek(0x4, SeekOrigin.Begin);

            for (var i = 0; i < 100; i++)
            {
                var miiData = new byte[MiiLength];
                int bytesRead = memoryStream.Read(miiData, 0, MiiLength);

                // If we can’t read more or we reach an 'empty' Mii, stop early.
                if (bytesRead == 0 || miiData.SequenceEqual(emptyMii))
                    break;

                miis.Add(miiData);
            }

            return miis;
        }

        /// <summary>
        /// Overwrites the local RFL_DB.dat with the given list of Mii blocks.
        /// Keeps the initial 4 bytes (Plaza Offset) intact; writes or clears up to 100 Mii slots.
        /// Also recalculates and writes the CRC16 checksum over the first 0x1F1DE bytes.
        /// </summary>
        public static void SaveMiiDb(List<byte[]> allMiis)
        {
            if (!File.Exists(WiiDbFile))
                return; // You may wish to handle differently if the file truly doesn't exist

            // Read the original DB to preserve the first 4 bytes (and any trailing data).
            byte[] dbFile = File.ReadAllBytes(WiiDbFile);
            using var ms = new MemoryStream(dbFile);

            // Seek past the 4-byte header
            ms.Seek(0x4, SeekOrigin.Begin);

            // Write each updated Mii (up to 100). If fewer than 100, write empty blocks for the rest.
            for (int i = 0; i < 100; i++)
            {
                if (i < allMiis.Count)
                {
                    ms.Write(allMiis[i], 0, MiiLength);
                }
                else
                {
                    ms.Write(emptyMii, 0, MiiLength);
                }
            }

            // Recalculate and update the CRC16 checksum if the file is large enough.
            // The checksum covers the first 0x1F1DE bytes.
            const int crcOffset = 0x1F1DE;
            if (dbFile.Length >= crcOffset + 2)
            {
                ushort crc = CalculateCRC16(dbFile, 0, crcOffset);
                dbFile[crcOffset] = (byte)(crc >> 8);       // High byte
                dbFile[crcOffset + 1] = (byte)(crc & 0xFF);   // Low byte
            }

            // Finally save back to the same file
            File.WriteAllBytes(WiiDbFile, dbFile);
        }

        /// <summary>
        /// Updates a Mii’s name (in RFL_DB.dat) given its 4-byte “ClientId” and the new name.
        /// Returns true if updated successfully, false if that ClientId was not found.
        /// </summary>
        public static bool UpdateMiiName(uint clientId, string newName)
        {
            // If there's no ID or no name, skip
            if (clientId == 0 || string.IsNullOrWhiteSpace(newName))
                return false;

            // Restrict Mii name to a maximum of 10 characters
            if (newName.Length > MiiNameLength)
                newName = newName.Substring(0, MiiNameLength);

            var allMiis = GetAllMiiData();
            bool updated = false;

            for (int i = 0; i < allMiis.Count; i++)
            {
                // The 4-byte "Mii ID"/"Avatar ID" is at offset 0x18 in a Wii Mii block.
                uint thisMiiId = 0;
                for (int b = 0; b < 4; b++)
                {
                    thisMiiId |= (uint)(allMiis[i][0x18 + b] << (8 * b));
                }

                if (thisMiiId == clientId)
                {
                    WriteMiiName(allMiis[i], newName);
                    updated = true;
                    break;
                }
            }

            if (updated)
                SaveMiiDb(allMiis);

            return updated;
        }

        /// <summary>
        /// Helper to read the entire RFL_DB.dat as a byte array (or empty if it doesn’t exist).
        /// </summary>
        private static byte[] GetMiiDb()
        {
            try
            {
                return (!File.Exists(WiiDbFile))
                    ? Array.Empty<byte>()
                    : File.ReadAllBytes(WiiDbFile);
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Overwrites the 10-character name inside a 74-byte Wii Mii block.
        /// Officially, the name is stored at offset 2, in (likely) 16-bit little-endian encoding.
        /// </summary>
        private static void WriteMiiName(byte[] mii, string newName)
        {
            // The name field is offset 2..(2+20) in the 74 bytes (10 UTF-16 chars).
            // We’ll write it in *little-endian* UTF-16 (common for Wii).
            var nameBytes = Encoding.BigEndianUnicode.GetBytes(newName);

            // Zero out the old name area
            for (int i = 0; i < 20; i++)
                mii[2 + i] = 0;

            // Copy in the new name bytes (up to 20 bytes)
            Array.Copy(nameBytes, 0, mii, 2, Math.Min(nameBytes.Length, 20));
        }

        /// <summary>
        /// Calculates a CRC16 (CCITT-FALSE) checksum for a portion of the given data.
        /// </summary>
        /// <param name="data">The byte array containing the data.</param>
        /// <param name="offset">The starting offset to begin calculation.</param>
        /// <param name="length">The number of bytes over which to compute the checksum.</param>
        /// <returns>A 16-bit unsigned integer representing the CRC16 checksum.</returns>
        private static ushort CalculateCRC16(byte[] data, int offset, int length)
        {
            const ushort polynomial = 0x1021;
            ushort crc = 0x0000;

            for (int i = offset; i < offset + length; i++)
            {
                crc ^= (ushort)(data[i] << 8);
                for (int bit = 0; bit < 8; bit++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ polynomial);
                    else
                        crc <<= 1;
                }
            }
            return crc;
        }
    }
}
