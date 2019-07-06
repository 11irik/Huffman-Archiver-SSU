using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Example;

namespace Kirill.Inc.Archiver
{
    public static class Encoding
    {
        private static Dictionary<char, BitArray> BuildDictionary(string line)
        {
            Dictionary<char, int> dictionary = new Dictionary<char, int>();
            List<Node> nodes = new List<Node>();
            
            foreach (char c in line)
            {
                if (dictionary.ContainsKey(c))
                {
                    dictionary[c]++;
                }
                else
                {
                    dictionary[c] = 1;
                }
            }

            foreach (var Pair in dictionary)
            {
                nodes.Add(new Node(Pair.Key, Pair.Value));
            }

            while (nodes.Count > 1)
            {
                nodes.Sort();
                nodes.Add(new Node(nodes[0], nodes[1]));
                nodes.Remove(nodes[0]);
                nodes.Remove(nodes[0]);
            }

            Dictionary<char, BitArray> bitDictionary = new Dictionary<char, BitArray>();

            foreach (var Pair in dictionary)
            {
                bool[] array = nodes[0].InOrder(Pair.Key, new List<bool>()).ToArray();
                bitDictionary.Add(Pair.Key, new BitArray(array));
            }
            
            return bitDictionary;
        }

        private static void EncodeDictionary(Dictionary<char, BitArray> dictionary, string fileName)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                byte numberOfChars = (byte)dictionary.Count;
                writer.Write(numberOfChars);

                foreach (var symbol in dictionary)
                {
                    writer.Write(symbol.Key);
                    
                    byte bitCodeLength = (byte)symbol.Value.Count;
                    writer.Write(bitCodeLength);
                    
                    byte [] bytes = new byte[bitCodeLength / 8 + (bitCodeLength % 8 == 0 ? 0 : 1 )];
                    symbol.Value.CopyTo(bytes, 0);
                    writer.Write(bytes);
                }
            }
        }
        
        private static byte[] EncodeDictionary(Dictionary<char, BitArray> dictionary)
        {
            List<byte> bytes = new List<byte>();
            
            byte numberOfChars = (byte)dictionary.Count;
            bytes.Add(numberOfChars);

            foreach (var symbol in dictionary)
            {
                bytes.Add((byte)symbol.Key);

                byte bitCodeLength = (byte)symbol.Value.Count;
                bytes.Add(bitCodeLength);

                byte[] byteCode = new byte[bitCodeLength / 8 + (bitCodeLength % 8 == 0 ? 0 : 1)];
                symbol.Value.CopyTo(byteCode, 0);
                bytes.AddRange(byteCode);
            }

            return bytes.ToArray();
        }

        private static void EncodeText(string line, Dictionary<char, BitArray> dictionary, string fileName)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                List<bool> encodedString = new List<bool>();
                foreach (char c in line)
                {
                    bool[] tmp = new bool[dictionary[c].Count];
                    dictionary[c].CopyTo(tmp, 0);
                    encodedString.AddRange(tmp);
                }
                
                BitArray encodedBits = new BitArray(encodedString.ToArray());
                byte [] encodedBytes= new byte[encodedBits.Length / 8 + (encodedBits.Length % 8 == 0 ? 0 : 1 )];
                encodedBits.CopyTo(encodedBytes, 0);
                
                writer.Write(encodedBytes);
            }
        }
        
        private static byte[] EncodeText(string line, Dictionary<char, BitArray> dictionary)
        {
            List<bool> encodedString = new List<bool>();
            foreach (char c in line)
            {
                bool[] tmp = new bool[dictionary[c].Count];
                dictionary[c].CopyTo(tmp, 0);
                encodedString.AddRange(tmp);
            }

            BitArray encodedBits = new BitArray(encodedString.ToArray());
            byte[] encodedBytes = new byte[encodedBits.Length / 8 + (encodedBits.Length % 8 == 0 ? 0 : 1)];
            encodedBits.CopyTo(encodedBytes, 0);

            return encodedBytes;



        }

        public static void EncodeAll(string line, string fileName)
        {
            //const string textFileName = "BinaryTextTemp.dat";
            //const string dictionaryFileName = "BinaryTableTemp.dat";
            
            Dictionary<char, BitArray> dictionary = BuildDictionary(line);
            //EncodeDictionary(dictionary, dictionaryFileName);
            //EncodeText(line, dictionary, textFileName);
            
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                int dictionaryLength = GetDictionaryLength(dictionary);
                writer.Write(dictionaryLength);
                
                writer.Write(EncodeDictionary(dictionary));

                writer.Write(EncodeText(line, dictionary));
            }
            
            
            
        }

        private static int GetDictionaryLength(Dictionary<char, BitArray> dictionary)
        {
            int dictionaryLengthInBytes = 0;

            foreach (var symbol in dictionary)
            {
                byte charSize = 1;
                byte SizeOfNumberOfCharCodeBytes = 1;
                byte[] bytes = new byte[symbol.Value.Length / 8 + (symbol.Value.Length % 8 == 0 ? 0 : 1)];;

                dictionaryLengthInBytes += (byte) bytes.Length + charSize + SizeOfNumberOfCharCodeBytes;
            }

            return dictionaryLengthInBytes;
        }

        public static String BitsToString(BitArray a)
        {
            string s = "";
            foreach (bool b in a)
            {
                if (b)
                    s += ("1");
                else
                {
                    s += ("0");
                }
            }

            return s;
        }
        
        //Debug part
        public static void ConsoleShowTable(Dictionary<char, BitArray> dictionary)
        {
            foreach (var Pair in dictionary)
            {
                Console.WriteLine("{0} {1}", Pair.Key.ToString(), BitsToString(Pair.Value));
            }
        }
        
        public static void TextfileWriteTable(Dictionary<char, BitArray> dictionary)
        {
            using (StreamWriter write = new StreamWriter(File.Open("tableone.txt", FileMode.Create)))
            {
                string s = "";
                foreach (var Pair in dictionary)
                {
                    s += Pair.Key.ToString() + " " + BitsToString(Pair.Value);
                    s += '\n';
                }
                write.Write(s);
            }
            
        }
        
    }
}