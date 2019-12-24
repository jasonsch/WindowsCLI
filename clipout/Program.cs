using System;
using System.Drawing;
using System.IO;
using YellowLab.Windows.GUI;

namespace clipout
{
    class Program
    {
        static void PrintUsage()
        {
            Console.WriteLine("Invalid usage!");
            Console.WriteLine("Usage: clipout [filename]");
            Console.WriteLine("If no filename is specified, write to stdout (only text)");
            Environment.Exit(-1);
        }

        //
        // TODO -- Also put the file on the clipboard with CF_HDROP so it can be pasted via explorer
        // https://docs.microsoft.com/en-us/windows/win32/dataxchg/standard-clipboard-formats
        //
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Write(Clipboard.text);
            }
            else
            {
                if (args.Length != 1)
                {
                    PrintUsage();
                }

                if (IsImageFile(args[0]))
                {
                    File.WriteAllBytes(args[0], ImageToByte(Clipboard.bitmap));
                }
                else
                {
                    File.WriteAllText(args[0], Clipboard.text);
                }
            }
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
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
