using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SHC
{
    class Program
    {
        public static void Main()
        {
            string playerDataFileName = "SHCPlayerData.txt";
            string greatestLordDataFileName = "GreatestLord.txt";
            if (File.Exists(playerDataFileName) || File.Exists(greatestLordDataFileName))
            {
                int count = 0;
                while (File.Exists(Path.GetFileNameWithoutExtension(playerDataFileName) + count.ToString() + ".txt") || File.Exists(Path.GetFileNameWithoutExtension(greatestLordDataFileName) + count.ToString() + ".txt")){
                    count++;
                }
                if (File.Exists(playerDataFileName))
                {
                    File.Move(playerDataFileName, Path.GetFileNameWithoutExtension(playerDataFileName) + count.ToString() + ".txt");
                }
                if (File.Exists(greatestLordDataFileName))
                {
                    File.Move(greatestLordDataFileName, Path.GetFileNameWithoutExtension(greatestLordDataFileName) + count.ToString() + ".txt");
                }
            }

            while(true){
                try
                {
                    StateMachine.Update();
                }
                catch (SHCNotFoundException) { }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "\n" + e.StackTrace);
                    File.WriteAllText(playerDataFileName, String.Empty);
                }
            }
        }
    }
}
