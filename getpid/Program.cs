using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace getpid
{
    class Program
    {
        static bool printVerbose = false;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
            }

            foreach (Process p in Process.GetProcesses())
            {
                if (Regex.IsMatch(p.ProcessName, args[0], RegexOptions.IgnoreCase) || p.ProcessName.ToLower() == args[0].ToLower())
                {
                    if (printVerbose)
                    {
                        Console.WriteLine($"Process match ==> {p.ProcessName} / {p.Id} / {p.SessionId}");
                    }
                    else
                    {
                        Console.WriteLine(p.Id);
                    }
                }
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Invalid Usage!");
            Console.WriteLine("Usage: getpid <process name>");
            Environment.Exit(0);
        }
    }
}
