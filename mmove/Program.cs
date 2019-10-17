using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using YellowLab.Windows.Win32;
using Mono.Options;


namespace mmove
{
    /*
     * Moves music files to the appropriate path under the user's "my music" folder.
     */
    public partial class Program
    {
        // TODO -- Command line flag to control this.
        static bool VerbosePrintingEnabled = true;
        static bool DeleteFileAfter = false;
        static string DestinationPath = Win32Interop.GetMusicPath();

        static void PrintUsage()
        {
            Console.WriteLine("Wrong number of arguments!");
            Console.WriteLine("mmove [-d | --destination] [-r | --remove]");
            Console.WriteLine("-d <directory> \t Use <directory> as the destination rather than the system-wide music path");
            Console.WriteLine("-r\tMove the source files to the recycle bin after they're copied");
            Environment.Exit(1); // TODO -- Error value and print out usage
        }

        static void Main(string[] args)
        {
            OptionSet Options = new OptionSet();
            List<string> FileNames;

            Options.Add("?|h|help", value => PrintUsage());
            Options.Add("d|destination", value => DestinationPath = value);
            Options.Add("r|remove", value => DeleteFileAfter = true);

            FileNames = Options.Parse(args);
            if (FileNames.Count == 0)
            {
                PrintUsage();
            }

            foreach (string File in FileNames)
            {
                MoveMusic(File, DestinationPath);
            }
        }

        static void LogVerboseMessage(string Message)
        {
            if (VerbosePrintingEnabled)
            {
                Console.WriteLine(Message);
            }

            System.Diagnostics.Debug.WriteLine(Message);
        }

        //
        // File can be an actual file or a directory containing some number of files. Note that
        // the file itself could be a .zip file, too.
        //
        static void MoveMusic(string File, string MusicPath)
        {
            string OriginalFileName = null;

            LogVerboseMessage($"MoveMusic('{File}', '{MusicPath}')");

            if (IsZipFile(File))
            {
                OriginalFileName = File;
                File = UncompressZipFile(File);
            }

            if (System.IO.File.Exists(File))
            {
                System.Diagnostics.Debug.Assert(OriginalFileName == null);
                CopyMusicFile(File, MusicPath);
            }
            else // It's a directory
            {
                string SubDirectory;

                System.Diagnostics.Debug.Assert(Directory.Exists(File));

                SubDirectory = DetermineDestinationDirectory(File);
                foreach (string ChildFile in Directory.EnumerateFiles(File))
                {
                    CopyMusicFile(ChildFile, MusicPath, SubDirectory);
                }
            }

            if (OriginalFileName != null)
            {
                LogVerboseMessage($"Deleting temporary directory {File}");
                Directory.Delete(File, true);
                File = OriginalFileName;
            }

            DeleteFile(File);
        }

        public static void DeleteFile(string FileName)
        {
            if (DeleteFileAfter)
            {
                FileSystem.DeleteFile(FileName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
        }

        //
        // TODO -- Option to force the song's title to match the file name (or at least warn if they're different?)
        // TODO -- Can we get metadata from aac files?
        //
        /// <summary>
        /// Reads the ID3 info from the specified file and moves the file to the right directory (based
        /// on the song's artist and album name) under DestinationPath.
        /// </summary>
        /// <param name="FileName">TODO</param>
        /// <param name="DestinationPath">TODO</param>
        static void CopyMusicFile(string FileName, string DestinationPath)
        {
            string Directory;

            if (GetMetadata(FileName, out Directory))
            {
                CopyMusicFile(FileName, DestinationPath, Directory);
            }
            else
            {
                Console.WriteLine($"Couldn't figure out what to do with {FileName}");
            }
        }

        static void CopyMusicFile(string FileName, string DestinationPath, string SubPath)
        {
            string FullPath = DestinationPath + Path.DirectorySeparatorChar + SubPath;

            LogVerboseMessage($"CopyMusicFile('{FileName}', '{DestinationPath}', '{SubPath}')");

            if (!Directory.Exists(FullPath))
            {
                Directory.CreateDirectory(FullPath);
            }

            FullPath += Path.DirectorySeparatorChar + Path.GetFileName(FileName);
            LogVerboseMessage($"Moving file {FileName} to {FullPath}");
            CopyFile(FileName, FullPath);
        }
    }
}
