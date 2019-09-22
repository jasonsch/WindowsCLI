using System;
using Microsoft.Win32;

namespace startupadd
{
    class Program
    {
        static void Main(string[] args)
        {
            RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

            if (args.Length < 2)
            {
                Console.WriteLine("Error: Invalid # of arguments");
                Console.WriteLine("Usage: setupadd <name> <command-to-run>");
                Environment.Exit(0);
            }

            Key.SetValue(args[0], args[1], RegistryValueKind.String);
        }
    }
}
