using System;
using System.IO.Compression;
using Microsoft.VisualBasic.FileIO;

namespace unzip
{
    class Program
    {
        static void Main(string[] args)
        {
            string InputFile = null;
            string OutputDirectory = null;

            if (args.Length == 1)
            {
                InputFile = args[0];
                // TODO -- Assumes the file name ends with ".zip"
                OutputDirectory = InputFile.Substring(0, InputFile.Length - 4);
            }
            else if (args.Length == 2)
            {
                InputFile = args[0];
                OutputDirectory = args[1];
            }
            else
            {
                System.Console.WriteLine("Wrong number of args!");
                Environment.Exit(0);
            }

            // Extract the files from the .zip and then move the .zip to the recycle bin.
            ZipFile.ExtractToDirectory(InputFile, OutputDirectory);
            FileSystem.DeleteFile(InputFile, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }
    }
}
