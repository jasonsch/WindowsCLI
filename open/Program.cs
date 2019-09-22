using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using Mono.Options;

namespace open
{
    class Program
    {
        static bool Wait = false;
        static bool BatchArguments = false;
        static string CommandToRun = null;

        /*
         * Command-line option: -b ==> Batch (pass all files to the appropriate app at one time)
         * Command-line option: -c ==> Path of command to run for the file.
         * Command-line option: -w ==> Wait for the command to end before returning.
         */
        static void Main(string[] args)
        {
            OptionSet options = new OptionSet();

            options.Add("b", foo => BatchArguments = true);
            options.Add("c=", command => CommandToRun = command);
            options.Add("w", foo => Wait = true);
            args = options.Parse(args).ToArray();

            if (BatchArguments)
            {
                args = new string[] { String.Join(" ", args) };
            }

            foreach (string arg in args)
            {
                HandleArgument(arg);
            }
        }

        static bool IsRegularExpression(string Parameter)
        {
            return Parameter.Contains('*');
        }

        static string GetBasePath(string FilePath)
        {
            string BasePath = Path.GetDirectoryName(FilePath);
            if (String.IsNullOrEmpty(BasePath))
            {
                BasePath = ".";
            }

            return BasePath;
        }

        /// <summary>
        /// Takes a regular expression that one would supply in a shell for an app's command line
        /// and massages it so it's acceptable to the .NET RegEx code.
        /// </summary>
        /// <param name="FileSpec">This will be something like "*.jpg".</param>
        /// <returns>A valid string that can be passed to (e.g.) RegEx.IsMatch()</returns>
        static string GetFileSystemRegularExpression(string FileSpec)
        {
            return Regex.Replace(Path.GetFileName(FileSpec), "\\*\\.", "[\\w]+\\.");
        }

        static string[] ApplyFileSystemRegularExpression(string FileSpec)
        {
            string[] Files = Directory.GetFiles(GetBasePath(FileSpec)).Where(File => Regex.IsMatch(File, GetFileSystemRegularExpression(FileSpec))).ToArray();
            if (BatchArguments)
            {
                return new string[] { String.Join(" ", Files) };
            }

            return Files;
        }

        static void HandleArgument(string arg)
        {
            if (IsRegularExpression(arg))
            {
                foreach (string File in ApplyFileSystemRegularExpression(arg))
                {
                    HandleArgument(File);
                }
            }
            else
            {
                try
                {
                    System.Diagnostics.Process p;

                    if (CommandToRun != null)
                    {
                        p = System.Diagnostics.Process.Start(CommandToRun, arg);
                    }
                    else
                    {
                        p = System.Diagnostics.Process.Start(arg);
                    }

                    if (p != null && Wait)
                    {
                        p.WaitForExit();
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Couldn't execute command: " + arg);
                    System.Console.WriteLine("Exception: " + e);
                }
            }
        }
    }
}
