using System;
using Microsoft.Win32;

namespace pathdel
{
    class Program
    {
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
                if (PathValue != args[0])
                {
                    NewPath += $"{PathValue};";
                }
            }

            Key.SetValue("Path", NewPath.TrimEnd(';'));
        }
    }
}
