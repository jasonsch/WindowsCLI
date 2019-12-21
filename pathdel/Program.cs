using System;
using Microsoft.Win32;
using YellowLab.Windows.Win32;

namespace pathdel
{
    class Program
    {
        static void BroadcastWininiChangeMessage()
        {
            IntPtr result;

            Win32Interop.SendMessageTimeout(Win32Interop.HWND_BROADCAST, 0x1A, IntPtr.Zero, "Environment", 2, 2000, out result);
        }

        static void Main(string[] args)
        {
            RegistryKey Key = Registry.CurrentUser.OpenSubKey("Environment", true);

            if (args.Length < 1)
            {
                Console.WriteLine("Error: Invalid # of arguments");
                Console.WriteLine("Usage: pathadd <path>");
                Environment.Exit(0);
            }

            string Path = Key.GetValue("Path") as string;
            string[] PathValues = Path.Split(';');
            string NewPath = string.Empty;

            foreach (string PathValue in PathValues)
            {
                if (PathValue.ToLower() != args[0].ToLower())
                {
                    NewPath += $"{PathValue};";
                }
            }

            Key.SetValue("Path", NewPath.TrimEnd(';'));
            BroadcastWininiChangeMessage();
        }
    }
}
