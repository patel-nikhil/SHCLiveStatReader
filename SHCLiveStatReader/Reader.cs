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

        static IntPtr GetProcessHandle()
        {
            Process process = Process.GetProcessesByName("Stronghold_Crusader_Extreme")[0];
            return OpenProcess(PROCESS_WM_READ, false, process.Id);
        }

        public static bool TestZero(Int32 addr, Int32 size) => Reader.ReadInt(addr, size) == 0;
        public static bool IsStatic(Int32 addr, Int32 size) {
            Int32 val = Reader.ReadInt(addr, size);
            System.Threading.Thread.Sleep(20);
            return Reader.ReadInt(addr, size) == val;
        }

        public static Int32 ReadInt(Int32 addr, Int32 size)
        {
            IntPtr processHandle = GetProcessHandle();
            int bytesRead = 0;
            byte[] buffer = new byte[size];
            ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static Int32 ReadByte(Int32 addr)
        {
            IntPtr processHandle = GetProcessHandle();
            int bytesRead = 0;
            byte[] buffer = new byte[1];
            ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);
            return buffer[0];
        }

        public static Boolean ReadBool(Int32 addr, Int32 size)
        {
            IntPtr processHandle = GetProcessHandle();
            int bytesRead = 0;
            byte[] buffer = new byte[size];
            ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToBoolean(buffer, 0);
        }

        public static String ReadString(Int32 addr)
        {
            IntPtr processHandle = GetProcessHandle();
            int bytesRead = 0;
            byte[] buffer = new byte[90];
            ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);
            return System.Text.Encoding.UTF8.GetString(buffer).Split('\0')[0];
        }

        public static Object ReadType(Int32 addr, String type)
        {
            if (type == "integer")
            {
                return Reader.ReadInt(addr, 8);
            }
            else if (type == "word")
            {
                return Reader.ReadInt(addr, 4);
            }
            else if (type == "byte")
            {
                return Reader.ReadByte(addr);
            }
            else if (type == "boolean")
            {
                return Reader.ReadBool(addr, 1);
            }
            else if (type == "string")
            {
                return Reader.ReadString(addr);
            }
            return 0;
        }
    }
}