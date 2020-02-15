using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SHC
{

    public class Reader
    {
        const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public static void Main()
        {

            //Process process = Process.GetProcessesByName("Stronghold_Crusader_Extreme")[0];
            //IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);

            while (true)
            {
                try
                {
                    Process process = Process.GetProcessesByName("Stronghold_Crusader_Extreme")[0];
                    IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);

                    UpdateGameData(processHandle, MemoryMap.playerData);
                    UpdateGameData(processHandle, MemoryMap.leaderBoard);


                    //foreach (KeyValuePair<String, Data> data in MemoryMap.dataset){
                    //    Console.WriteLine(data.Value.ToString());
                    //}
                    //Console.WriteLine(MemoryMap.PlayerDataString());
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter("SHCPlayerData.txt"))
                    {
                        file.WriteLine(MemoryMap.PlayerDataString());
                    }

                    String leaderBoard = MemoryMap.LeaderboardString();
                    if (leaderBoard == "{}") continue;

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter("SHCLeaderBoard.txt"))
                    {
                        file.WriteLine(leaderBoard);
                    }

                    System.Threading.Thread.Sleep(1000);
                }
                catch (IndexOutOfRangeException)
                {
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }

        static Int32 readInt(IntPtr processHandle, Int32 addr, Int32 size)
        {
            int bytesRead = 0;
            String num = "";
            byte[] buffer = new byte[size];

            ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);
            if (size == 1)
            {
                return int.Parse(buffer[0].ToString("x"), System.Globalization.NumberStyles.HexNumber);
            }

            int start = buffer.Length - 1;
            while (start > 0 && buffer[start] == 0) start--;
            for (var i = start; i >= 0; i--)
            {
                byte current = buffer[i];
                if (i == start)
                {
                    num = num + current.ToString("x");
                }
                else if (current.ToString("x").Length == 1 && !(current > 0 && current < 9))
                {
                    num = num + current.ToString("x") + "0";
                }
                else
                {
                    num = num + current.ToString("x");
                }
            }
            return int.Parse(num, System.Globalization.NumberStyles.HexNumber);
        }


        static String readString(IntPtr processHandle, Int32 addr, Int32 size)
        {
            int bytesRead = 0;
            String str = "";
            byte[] buffer = new byte[size];

            ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);
            Encoding ascii = Encoding.ASCII;
            foreach (byte b in buffer)
            {
                if (b == '\0') break;
                str += Convert.ToChar(b);
            }
            //str = String.Join("",ascii.GetChars(buffer));
            return str;
        }

        static void UpdateGameData(IntPtr processHandle, Dictionary<string, Data> gameData)
        {
            foreach (KeyValuePair<String, Data> data in gameData)
            {
                //Console.WriteLine(data.Key);
                foreach (KeyValuePair<string, Dictionary<String, Object>> dataItem in data.Value.GameData())
                {
                    Int32 addr = Convert.ToInt32(dataItem.Value["Address"]);
                    Int32 size = Convert.ToInt32(dataItem.Value["Size"]);
                    Object value = "";
                    if (dataItem.Key != "Name")
                    {
                        value = readInt(processHandle, addr, size);
                    }
                    else
                    {
                        value = readString(processHandle, addr, size);
                    }
                    dataItem.Value["Value"] = value;
                }
            }
        }
    }
}