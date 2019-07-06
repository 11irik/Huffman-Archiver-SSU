using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kirill.Inc.Archiver
{
    public static class Decoding
    {
        private static Dictionary<string, char> DecodeTable(string fileName)
        {
            Dictionary<string, char> decodeTable = new Dictionary<string, char>();
            
            using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                byte count = reader.ReadByte();

                for (byte i = 0; i < count; ++i)
                {
                    char symbol = reader.ReadChar();
                    byte symbolLength = reader.ReadByte();

                    int k = symbolLength / 8;
                    if (symbolLength % 8 != 0) k++;
                    var bytes = reader.ReadBytes(k);

                    BitArray bits = new BitArray(bytes) {Length = symbolLength};
                    decodeTable[BitsToBitString(bits)] = symbol;
                }
            }

            return decodeTable;
        }

        private static string DecodeText(string fileName, Dictionary<string, char> table)
        {
            StringBuilder s = new StringBuilder("");
            var symbol = "";

            byte[] fileBytes = File.ReadAllBytes(fileName);
            BitArray b = new BitArray(fileBytes);
            
            for (var i = 0; i < b.Length; ++i)
            {
                symbol += b[i] ? 1 : 0;
                if (table.ContainsKey(symbol))
                {
                    s.Append(table[symbol]);
                    symbol = "";
                }
            }

            return s.ToString();
        }
        
        public static void DecodeAll(string encodedFileName, string decodedFileName)
        {
            string _textFileName = "BinaryTextTemp2.dat";
            string _tableFileName = "BinaryTableTemp2.dat";

            byte[] allBytes = File.ReadAllBytes(encodedFileName);
            
            using (BinaryWriter writer = new BinaryWriter(File.Open(_tableFileName, FileMode.Create)))
            {
                int count = BitConverter.ToInt32(allBytes, 0);
                writer.Write(allBytes, 4, count + 7);
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(_textFileName, FileMode.Create)))
            {
                int count = allBytes.Length - BitConverter.ToInt32(allBytes, 0) - 4;
                writer.Write(allBytes, BitConverter.ToInt32(allBytes, 0) + 11, count - 7);
            }
            
            Dictionary<string, char> decodeTable = DecodeTable(_tableFileName);

            string s = DecodeText(_textFileName, decodeTable);

            using (StreamWriter write = new StreamWriter(File.Open(decodedFileName, FileMode.Create)))
            {
                write.Write(s);
            }
            
            File.Delete(_tableFileName);
            File.Delete(_textFileName);
        }

        private static string BitsToBitString(BitArray bits)
        {
            var s = "";
            foreach (bool b in bits)
            {
                if (b) s += 1;
                else s += 0;
            }

            return s;
        }
    }
}