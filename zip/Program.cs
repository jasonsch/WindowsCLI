using System;
using System.IO.Compression;
using Mono.Options;

namespace zip
{
    class Program
    {
        static void PrintUsage()
        {
            Console.WriteLine("zip <directory>");
            Environment.Exit(0);
        }

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                PrintUsage();
            }

            string directory = args[0];
            Console.WriteLine("Zip: {0} => {1}", directory, directory + ".zip");
            ZipFile.CreateFromDirectory(directory, directory + ".zip");
        }
    }
}
