using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WheelWizard.Services.WiiManagement.SaveData
{
    public static class InternalMiiManager
    {
        // Example path to the Wii Mii DB
        private static readonly string WiiDbFile = Path.Combine(
            PathManager.WiiFolderPath,
            "shared2",
            "menu",
            "FaceLib",
            "RFL_DB.dat");

        private const int MiiLength = 74; // Each Mii block is 74 bytes
        private static readonly byte[] emptyMii = new byte[MiiLength];

        /// <summary>
        /// Reads the entire RFL_DB.dat and returns up to 100 Mii blocks (74 bytes each).
        /// </summary>
        public static List<byte[]> GetAllMiiData()
        {
            var miis = new List<byte[]>();
            var allMiiData = GetMiiDb();
            if (allMiiData.Length < 4)
                return miis;

            using var memoryStream = new MemoryStream(allMiiData);
            // According to some docs, the first 4 bytes is a "Plaza Offset" or "RNCD" magic, skip them
            memoryStream.Seek(0x4, SeekOrigin.Begin);

            for (int i = 0; i < 100; i++)
            {
                var miiData = new byte[MiiLength];
                int bytesRead = memoryStream.Read(miiData, 0, MiiLength);
                if (bytesRead < MiiLength)
                    break;

                // If we read an all-zero block, it typically means "empty".
                if (miiData.SequenceEqual(emptyMii))
                {
                    miis.Add(new byte[MiiLength]); 
                    // We keep the slot, but it's recognized as empty
                }
                else
                {
                    miis.Add(miiData);
                }
            }
            return miis;
        }

        /// <summary>
        /// Overwrites the local RFL_DB.dat with the given list of Mii blocks.
        /// Keeps the initial 4 bytes intact, writes or clears up to 100 Mii slots,
        /// then recalculates the CRC16 at 0x1F1DE (if large enough).
        /// </summary>
        public static void SaveMiiDb(List<byte[]> allMiis)
        {
            if (!File.Exists(WiiDbFile))
            {
                Console.WriteLine("[SaveMiiDb] RFL_DB.dat not found. Cannot save Mii DB!");
                return;
            }

            byte[] dbFile = File.ReadAllBytes(WiiDbFile);

            // Keep the first 4 bytes (header)
            using var ms = new MemoryStream(dbFile);
            ms.Seek(0x4, SeekOrigin.Begin);

            // Write 100 Mii blocks max
            for (int i = 0; i < 100; i++)
            {
                var block = (i < allMiis.Count) ? allMiis[i] : emptyMii;
                ms.Write(block, 0, MiiLength);
            }

            // Recompute checksum if file is large enough
            const int crcOffset = 0x1F1DE;
            if (dbFile.Length >= crcOffset + 2)
            {
                ushort crc = CalculateCRC16(dbFile, 0, crcOffset);
                dbFile[crcOffset]     = (byte)(crc >> 8);
                dbFile[crcOffset + 1] = (byte)(crc & 0xFF);
            }

            // Write updated DB
            File.WriteAllBytes(WiiDbFile, dbFile);
            Console.WriteLine("[SaveMiiDb] Successfully updated RFL_DB.dat with new Mii data.");
        }

        /// <summary>
        /// Retrieves the *raw* 74-byte Mii block that has the specified ClientId (found at offset 0x18..0x1B).
        /// Returns an empty array if none found or if RFL_DB.dat is missing.
        /// </summary>
        public static byte[] GetMiiDataByClientId(uint clientId)
        {
            if (clientId == 0) return Array.Empty<byte>();

            var allMiis = GetAllMiiData();
            foreach (var block in allMiis)
            {
                if (block.Length != MiiLength) 
                    continue;

                uint thisMiiId = ReadLittleEndianUInt32(block, 0x18);
                if (thisMiiId == clientId)
                    return block;
            }
            return Array.Empty<byte>();
        }

         /// <summary>
        /// Creates a brand new Mii entry in RFL_DB with a given name.
        /// We pick the first empty slot, generate a random 4-byte clientId,
        /// fill the Mii block per the wiki’s detailed structure and save it.
        /// Returns the newly assigned clientId or 0 on failure.
        /// </summary>
        /// <param name="initialName">The name to store in the Mii block (truncated to 10 characters)</param>
        public static uint CreateNewMiiInDb(string initialName)
        {
            if (!File.Exists(WiiDbFile))
            {
                Console.WriteLine("[CreateNewMiiInDb] RFL_DB.dat not found. Cannot create new Mii!");
                return 0;
            }

            // 1) Read the entire DB
            var allMiis = GetAllMiiData();

            // 2) Find first empty slot.
            int slotIndex = -1;
            for (int i = 0; i < allMiis.Count; i++)
            {
                if (allMiis[i].SequenceEqual(emptyMii))
                {
                    slotIndex = i;
                    break;
                }
            }
            // If no empty slot is found but there are fewer than 100, append a new slot.
            if (slotIndex < 0 && allMiis.Count < 100)
            {
                slotIndex = allMiis.Count;
                allMiis.Add(emptyMii.ToArray());
            }
            if (slotIndex < 0)
            {
                Console.WriteLine("[CreateNewMiiInDb] No empty Mii slot found (all 100 used?).");
                return 0;
            }

            // 3) Construct a new 74-byte Mii block following the wiki structure.
            var newMiiBlock = new byte[MiiLength];

            // -- Offset 0x00-0x01: Header (flags & basic info)
            // Bits (from MSB to LSB):
            //   bit15: invalid (0 = valid)
            //   bit14: isGirl (0 = male)
            //   bits13-10: month (we set to 1)
            //   bits9-5: day (we set to 1)
            //   bits4-1: favColor (0)
            //   bit0: isFavorite (0)
            
            ushort header = (ushort)((0 << 15) | (0 << 14) | (1 << 10) | (1 << 5) | (0 << 1) | 0);

            WriteUInt16BigEndian(newMiiBlock, 0x00, header);

            // -- Offset 0x02-0x15: Mii name (10 UTF-16 code units, big-endian)
            string miiName = (initialName.Length > 10 ? initialName.Substring(0, 10) : initialName).PadRight(10, '\0');
            byte[] nameBytes = Encoding.BigEndianUnicode.GetBytes(miiName);
            Array.Copy(nameBytes, 0, newMiiBlock, 0x02, Math.Min(nameBytes.Length, 20));

            // -- Offset 0x16: Height (0–127). We choose a default of 80 (0x50).
            newMiiBlock[0x16] = 0x50;

            // -- Offset 0x17: Weight (0–127). We choose a default of 80 (0x50).
            newMiiBlock[0x17] = 0x50;

            // -- Offset 0x18-0x1B: Mii ID (clientId) – 4 bytes.
            uint newClientId = GenerateRandomUInt32();
            // Write in little-endian (to be consistent with other routines)
            newMiiBlock[0x18] = (byte)(newClientId & 0xFF);
            newMiiBlock[0x19] = (byte)((newClientId >> 8) & 0xFF);
            newMiiBlock[0x1A] = (byte)((newClientId >> 16) & 0xFF);
            newMiiBlock[0x1B] = (byte)((newClientId >> 24) & 0xFF);

            // -- Offset 0x1C-0x1F: System ID (set to zero)
            byte[] validSystemId = new byte[] { 0x42, 0xBC, 0xE1, 0xC2 };
            Array.Copy(validSystemId, 0, newMiiBlock, 0x1C, 4);

            // -- Offset 0x20-0x21: Face features
            // Fields: faceShape (3), skinColor (3), facialFeature (4),
            //         unknown (3), mingleOff (1), unknown (1), downloaded (1).
            // We set all these to 0.
            WriteUInt16BigEndian(newMiiBlock, 0x20, 0);

            // -- Offset 0x22-0x23: Hair info
            // Fields: hairType (7), hairColor (3), hairPart (1), unknown (5)
            WriteUInt16BigEndian(newMiiBlock, 0x22, 0);

            // -- Offset 0x24-0x27: Eyebrow settings
            // Fields (32 bits): 
            //   eyebrowType (5), unknown (1), eyebrowRotation (4),
            //   unknown (6), eyebrowColor (3), eyebrowSize (4, default 4),
            //   eyebrowVertPos (5, default 10), eyebrowHorizSpacing (4, default 2)
            uint eyebrows = ((uint)(0 & 0x1F) << 27) | // eyebrowType = 0
                            ((uint)(0 & 0x1) << 26)  | // unknown
                            ((uint)(0 & 0xF) << 22)  | // eyebrowRotation = 0
                            ((uint)(0 & 0x3F) << 16) | // unknown
                            ((uint)(0 & 0x7) << 13)  | // eyebrowColor = 0
                            ((uint)(4 & 0xF) << 9)   | // eyebrowSize = 4
                            ((uint)(10 & 0x1F) << 4) | // eyebrowVertPos = 10
                            ((uint)(2 & 0xF));         // eyebrowHorizSpacing = 2
            WriteUInt32BigEndian(newMiiBlock, 0x24, eyebrows);

            // -- Offset 0x28-0x2B: Eye settings
            // Fields (32 bits):
            //   eyeType (6), unknown (2), eyeRotation (3),
            //   eyeVertPos (5, default 12), eyeColor (3),
            //   unknown (1), eyeSize (3, default 4), eyeHorizSpacing (4, default 2),
            //   unknown (5)
            uint eyes = ((uint)(0 & 0x3F) << 26) |    // eyeType = 0
                        ((uint)(0 & 0x3) << 24)  |    // unknown
                        ((uint)(0 & 0x7) << 21)  |    // eyeRotation = 0
                        ((uint)(12 & 0x1F) << 16)|    // eyeVertPos = 12
                        ((uint)(0 & 0x7) << 13)  |    // eyeColor = 0
                        ((uint)(0 & 0x1) << 12)  |    // unknown
                        ((uint)(4 & 0x7) << 9)   |    // eyeSize = 4
                        ((uint)(2 & 0xF) << 5)   |    // eyeHorizSpacing = 2
                        ((uint)(0 & 0x1F));          // unknown
            WriteUInt32BigEndian(newMiiBlock, 0x28, eyes);

            // -- Offset 0x2C-0x2D: Nose settings
            // Fields (16 bits): noseType (4), noseSize (4, default 4),
            //                   noseVertPos (5, default 9), unknown (3)
            ushort nose = (ushort)(((0 & 0xF) << 12) |  // noseType = 0
                                   ((4 & 0xF) << 8)  |  // noseSize = 4
                                   ((9 & 0x1F) << 3) |  // noseVertPos = 9
                                   (0));               // unknown = 0
            WriteUInt16BigEndian(newMiiBlock, 0x2C, nose);

            // -- Offset 0x2E-0x2F: Lip settings
            // Fields (16 bits): lipType (5), lipColor (2), lipSize (4, default 4),
            //                   lipVertPos (5, default 13)
            ushort lip = (ushort)(((0 & 0x1F) << 11) |  // lipType = 0
                                  ((0 & 0x3) << 9)   |  // lipColor = 0
                                  ((4 & 0xF) << 5)   |  // lipSize = 4
                                  (13 & 0x1F));         // lipVertPos = 13
            WriteUInt16BigEndian(newMiiBlock, 0x2E, lip);

            // -- Offset 0x30-0x31: Glasses settings
            // Fields (16 bits): glassesType (4), glassesColor (3), unknown (1),
            //                   glassesSize (3, default 4), glassesVertPos (5, default 10)
            ushort glasses = (ushort)(((0 & 0xF) << 12) | // glassesType = 0
                                      ((0 & 0x7) << 9)  | // glassesColor = 0
                                      ((0 & 0x1) << 8)  | // unknown = 0
                                      ((4 & 0x7) << 5)  | // glassesSize = 4
                                      (10 & 0x1F));       // glassesVertPos = 10
            WriteUInt16BigEndian(newMiiBlock, 0x30, glasses);

            // -- Offset 0x32-0x33: Mustache/Beard settings
            // Fields (16 bits): mustacheType (2), beardType (2), facialHairColor (3),
            //                   mustacheSize (4, default 4), mustacheVertPos (5, default 10)
            ushort facialHair = (ushort)(((0 & 0x3) << 14) | // mustacheType = 0
                                         ((0 & 0x3) << 12) | // beardType = 0
                                         ((0 & 0x7) << 9)  | // facialHairColor = 0
                                         ((4 & 0xF) << 5)  | // mustacheSize = 4
                                         (10 & 0x1F));      // mustacheVertPos = 10
            WriteUInt16BigEndian(newMiiBlock, 0x32, facialHair);

            // -- Offset 0x34-0x35: Mole settings
            // Fields (16 bits): moleOn (1), moleSize (4, default 4),
            //                   moleVertPos (5, default 20), moleHorizPos (5, default 2),
            //                   unknown (1)
            ushort mole = (ushort)(((0 & 0x1) << 15) |      // moleOn = 0 (no mole)
                                   ((4 & 0xF) << 11) |      // moleSize = 4
                                   ((20 & 0x1F) << 6) |     // moleVertPos = 20
                                   ((2 & 0x1F) << 1) |      // moleHorizPos = 2
                                   (0));                   // unknown = 0
            WriteUInt16BigEndian(newMiiBlock, 0x34, mole);

            // -- Offset 0x36-0x49: Creator name (10 UTF-16 code units).
            // For a new Mii we leave this blank (all zeros).
            // (If desired, you could write a default creator name using Encoding.BigEndianUnicode.)
            
            string creatorName = "Creator".PadRight(10, '\0');
            byte[] creatorBytes = Encoding.BigEndianUnicode.GetBytes(creatorName);
            Array.Copy(creatorBytes, 0, newMiiBlock, 0x36, Math.Min(creatorBytes.Length, 20));

            // 4) Put the new block into that slot
            allMiis[slotIndex] = newMiiBlock;

            // 5) Save it back to disk
            SaveMiiDb(allMiis);

            Console.WriteLine($"[CreateNewMiiInDb] Created new Mii in slot {slotIndex} with clientId=0x{newClientId:X8}, name='{initialName}'");
            return newClientId;
        }
         
        private static readonly Random random = new Random();
        private static uint GenerateRandomUInt32()
        {
            
            // Combine two 16-bit calls (Random.Next returns a nonnegative Int32)
            unchecked
            {
                return ((uint)random.Next(0, 1 << 16) << 16) | (uint)random.Next(0, 1 << 16);
            }
        }

        /// <summary>
        /// Writes a 16-bit unsigned integer in big-endian order into the byte array.
        /// </summary>
        private static void WriteUInt16BigEndian(byte[] data, int offset, ushort value)
        {
            data[offset] = (byte)(value >> 8);
            data[offset + 1] = (byte)(value & 0xFF);
        }
        
        /// <summary>
        /// Writes a 32-bit unsigned integer in big-endian order into the byte array.
        /// </summary>
        private static void WriteUInt32BigEndian(byte[] data, int offset, uint value)
        {
            data[offset] = (byte)(value >> 24);
            data[offset + 1] = (byte)((value >> 16) & 0xFF);
            data[offset + 2] = (byte)((value >> 8) & 0xFF);
            data[offset + 3] = (byte)(value & 0xFF);
        }
        

        /// <summary>
        /// Updates the name of an existing Mii in RFL_DB given its 4-byte “ClientId”.
        /// Returns true if updated successfully, false if that ClientId was not found.
        /// </summary>
        public static bool UpdateMiiName(uint clientId, string newName)
        {
            if (clientId == 0)
                return false;

            if (!File.Exists(WiiDbFile))
                return false;

            var allMiis = GetAllMiiData();
            bool updated = false;

            for (int i = 0; i < allMiis.Count; i++)
            {
                var block = allMiis[i];
                if (block.Length != MiiLength)
                    continue;

                uint thisMiiId = ReadLittleEndianUInt32(block, 0x18);
                if (thisMiiId == clientId)
                {
                    // Found the Mii
                    WriteMiiName(block, newName);
                    allMiis[i] = block;
                    updated = true;
                    break;
                }
            }

            if (updated)
                SaveMiiDb(allMiis);

            return updated;
        }

        /// <summary>
        /// Writes a 10-character Mii name into the 74-byte Mii block at offset 0x02,
        /// in *little-endian* UTF-16 (which is standard for the Wii).
        /// Max length is 10 chars => 20 bytes.
        /// </summary>
        private static void WriteMiiName(byte[] miiBlock, string name)
        {
            if (name.Length > 10)
                name = name.Substring(0, 10);

            // Clear out the old name area
            for (int i = 0; i < 20; i++)
                miiBlock[2 + i] = 0;

            // Convert to big-endian UTF-16 (this is what Wii expects)
            var nameBytes = Encoding.BigEndianUnicode.GetBytes(name);
            Array.Copy(nameBytes, 0, miiBlock, 2, Math.Min(nameBytes.Length, 20));
        }

        /// <summary>
        /// Reads a 32-bit little-endian uint from the given byte array at the specified offset.
        /// </summary>
        private static uint ReadLittleEndianUInt32(byte[] data, int offset)
        {
            // Wii often uses little-endian for Mii blocks internally
            return (uint)(
                  (data[offset + 0] << 0)
                | (data[offset + 1] << 8)
                | (data[offset + 2] << 16)
                | (data[offset + 3] << 24)
            );
        }

        /// <summary>
        /// Helper to load the entire RFL_DB.dat from disk or return an empty array if not found.
        /// </summary>
        private static byte[] GetMiiDb()
        {
            try
            {
                if (!File.Exists(WiiDbFile))
                    return Array.Empty<byte>();

                return File.ReadAllBytes(WiiDbFile);
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Calculates a CRC16 (CCITT-FALSE) checksum for a portion of the given data.
        /// This is used for RFL_DB.dat validation at offset 0x1F1DE (covers the preceding bytes).
        /// </summary>
        public static ushort CalculateCRC16(byte[] data, int offset, int length)
        {
            const ushort polynomial = 0x1021;
            ushort crc = 0x0000;

            for (int i = offset; i < offset + length; i++)
            {
                crc ^= (ushort)(data[i] << 8);
                for (int j = 0; j < 8; j++)
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
