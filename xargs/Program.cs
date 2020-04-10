using System;
using YellowLab.Windows.GUI;

namespace xargs
{
    class Program
    {
        private static void PrintUsage()
        {
            Console.WriteLine("Invalid usage!");
            // TODO
            Environment.Exit(0);
        }

        static void Main(string[] args)
        {
            string program = "";
            string arguments = "";
            string line, lastLine = null; // in case we need to ignore the last read line because it'll make our command line too long
            // const int maxArgumentLength = 0x1000;
            const int maxArgumentLength = 100;

            if (args.Length == 0)
            {
                PrintUsage();
            }

            for (int i = 0; i < args.Length; ++i)
            {
                program += $"{args[i]} ";
            }
            program.Trim();

            var streamReader = new System.IO.StreamReader(System.Console.OpenStandardInput());

            while (true)
            {
                if (lastLine == null)
                {
                    line = streamReader.ReadLine();
                    if (line == null)
                    {
                        // TODO -- This code shouldn't be in two different places.
                        Console.Write($"program ===> {program} / arguments ==> {arguments}");
                        Process.CreateProcess(program, arguments);
                        break;
                    }
                }
                else
                {
                    line = lastLine;
                    lastLine = null;
                }

                if (line.Length > maxArgumentLength)
                {
                    Console.WriteLine("ERROR: Ignoring argument that's too long: ", line);
                }
                else if ((line.Length + arguments.Length + 1) <= maxArgumentLength)
                {
                    arguments += $"{line} ";
                }
                else
                {
                    //
                    // Adding this line would put us over the limit so let's execute our command.
                    //
                    lastLine = line;
                    Console.Write($"program ===> {program} / arguments ==> {arguments}");
                    Process.CreateProcess(program, arguments);
                }
            }
        }
    }
}
