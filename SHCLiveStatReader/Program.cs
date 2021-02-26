using System;
using System.IO;
using static SHC.Constants;
using static SHC.Util;

namespace SHC
{
    class Program
    {
        public static void Main()
        {
            BackupExistingFile(PLAYERDATA_FILENAME, GREATEST_LORD_FILENAME);

            while(true){
                try
                {
                    StateMachine.Update();
                }
                catch (SHCNotFoundException) { }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "\n" + e.StackTrace);
                    File.WriteAllText(PLAYERDATA_FILENAME, string.Empty);
                }
            }
        }

        private static void BackupExistingFile(string playerDataFilename, string greatestLordDataFilename)
        {
            if (File.Exists(playerDataFilename) || File.Exists(greatestLordDataFilename))
            {
                string suffix = GetNextRandom().ToString();
                while (File.Exists(PLAYERDATA_FILE_PREFIX + suffix + DATA_FILE_SUFFIX) || File.Exists(GREATEST_LORD_FILE_PREFIX + suffix + DATA_FILE_SUFFIX))
                {
                    suffix = GetNextRandom().ToString();
                }
                if (File.Exists(playerDataFilename))
                {
                    File.Move(playerDataFilename, PLAYERDATA_FILE_PREFIX + suffix + DATA_FILE_SUFFIX);
                }
                if (File.Exists(greatestLordDataFilename))
                {
                    File.Move(greatestLordDataFilename, GREATEST_LORD_FILE_PREFIX + suffix + DATA_FILE_SUFFIX);
                }
            }
        }
    }
}
