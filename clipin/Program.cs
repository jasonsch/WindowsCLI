using System;
using System.Drawing;
using System.IO;
using YellowLab.Windows.GUI;

namespace clipin
{
    class Program
    {
        static void PrintUsage()
        {
            Console.WriteLine("Invalid usage!");
            Console.WriteLine("Usage: clipin [filename]");
            Console.WriteLine("If no filename is specified, read from stdin");
            Environment.Exit(-1);
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Clipboard.text = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                if (args.Length != 1)
                {
                    PrintUsage();
                }

                if (IsImageFile(args[0]))
                {
                    Clipboard.bitmap = (Bitmap)Image.FromStream(File.OpenRead(args[0]));
                }
                else
                {
                    Clipboard.text = File.ReadAllText(args[0]);
                }
            }
        }

        private static bool IsImageFile(string file)
        {
            return file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                   file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                   file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase);
        }
    }
}

