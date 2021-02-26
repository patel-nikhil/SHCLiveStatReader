using System;
using System.IO;
using System.Text;
using static SHC.Constants;

namespace SHC
{
    public class Util
    {

        static readonly Random gen = new Random();

        public static int GetNextRandom()
        {
            return gen.Next();
        }

        public static string GetFilename(string prefix)
        {
            string newFileName = prefix + gen.Next().ToString() + DATA_FILE_SUFFIX;
            while (File.Exists(newFileName))
            {
                newFileName = prefix + gen.Next().ToString() + DATA_FILE_SUFFIX;
            }
            return newFileName;
        }
        
        public static void WriteData(string filename, object data)
        {
            File.WriteAllText(filename, Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented), Encoding.BigEndianUnicode);
        }
    }
}
