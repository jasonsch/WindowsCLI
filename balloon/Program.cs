using System;
using YellowLab.Windows.GUI;

namespace balloon
{
    class Program
    {
        static void PrintUsage()
        {
            Console.WriteLine("Invalid number of arguments!");
            Console.WriteLine("Usage: balloon <title> <body>");
        }

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                PrintUsage();
                return;
            }

            ShellBalloon.Show(args[0], args[1]);
        }
    }
}
