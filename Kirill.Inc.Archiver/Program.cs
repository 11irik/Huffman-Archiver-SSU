using System.IO;
using Encoding = Kirill.Inc.Archiver.Encoding;

namespace Kirill.Inc.Archiver
{
    internal static class Program
    {
        private const string TableAndTextFileName = "Archive.k-zip";
        private const string DecodedTextFileName = "DecodedText.txt";
        
        public static void Main()
        {
            string line;
            
            using (StreamReader fileIn = new StreamReader("./input.txt"))
            {
                line = fileIn.ReadToEnd();
            }
            
            Encoding.EncodeAll(line, TableAndTextFileName);
            
            Decoding.DecodeAll(TableAndTextFileName, DecodedTextFileName);
        }
      
    }
    
}