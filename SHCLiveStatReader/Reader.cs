using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using static SHC.Constants;

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
            try
            {
                Process process = Process.GetProcessesByName(SHC_PROCESS_NAME)[0];
                return OpenProcess(PROCESS_WM_READ, false, process.Id);
            }
            catch (Exception)
            {
                throw new SHCNotFoundException();
            }
        }

        public static bool TestZero(int addr, int size) => Reader.ReadInt(addr, size) == 0;
        public static bool IsStatic(int addr, int size) {
            int val = ReadInt(addr, size);
            System.Threading.Thread.Sleep(20);
            return ReadInt(addr, size) == val;
        }

        public static int ReadInt(int addr, int size)
        {
            IntPtr processHandle = GetProcessHandle();
            int bytesRead = 0;
            byte[] buffer = new byte[size];
            ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static int ReadWord(int addr, int size)
        {
            IntPtr processHandle = GetProcessHandle();
            int bytesRead = 0;
            byte[] buffer = new byte[size];
            ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToInt16(buffer, 0);
        }

        public static byte ReadByte(int addr)
        {
            IntPtr processHandle = GetProcessHandle();
            int bytesRead = 0;
            byte[] buffer = new byte[1];
            ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);
            return buffer[0];
        }

        public static bool ReadBool(int addr, int size)
        {
            IntPtr processHandle = GetProcessHandle();
            int bytesRead = 0;
            byte[] buffer = new byte[size];
            ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToBoolean(buffer, 0);
        }

        public static string ReadString(int addr)
        {
            IntPtr processHandle = GetProcessHandle();
            int bytesRead = 0;
            byte[] buffer = new byte[90];
            ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);
            return Encoding.GetEncoding(1252).GetString(buffer).Split('\0')[0];
        }

        public static byte[] ReadBytes(int addr, int size)
        {
            IntPtr processHandle = GetProcessHandle();
            int bytesRead = 0;
            byte[] buffer = new byte[size];
            ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);
            return buffer;
        }

        public static object ReadType(int addr, string type)
        {
            if (type == "integer")
            {
                return Reader.ReadInt(addr, 4);
            }
            else if (type == "word")
            {
                return Reader.ReadWord(addr, 2);
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