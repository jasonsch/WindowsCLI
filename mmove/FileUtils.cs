using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using YellowLab.Windows.Win32;

namespace mmove
{
    public partial class Program
    {
        //
        // Naive test to see if the specified file is a .zip file.
        //
        static bool IsZipFile(string FileName)
        {
            return FileName.EndsWith(".zip");
        }

        static string GetArtist(TagLib.Tag tag)
        {
            if (!string.IsNullOrEmpty(tag.FirstAlbumArtist))
            {
                return tag.FirstAlbumArtist;
            }
            else if (tag.AlbumArtists.Length != 0)
            {
                // TODO -- Concat all the artists' names if there's more than one?
                return tag.AlbumArtists[0];
            }
            else if (tag.Artists.Length != 0)
            {
                return tag.Artists[0];
            }
            else
            {
                return null;
            }
        }

        static bool GetMetadata(string FileName, out string Directory)
        {
            if (FileName.EndsWith(".mp3"))
            {
                TagLib.File file = TagLib.File.Create(FileName);

                Directory = FixupPathComponent(GetArtist(file.Tag)) + Path.DirectorySeparatorChar + FixupPathComponent(file.Tag.Album);

                return true;
            }
            else
            {
                LogVerboseMessage($"Couldn't get metadata from {FileName} as it's not an mp3 file");
                Directory = null;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Component"></param>
        /// <returns></returns>
        static string FixupPathComponent(string Component)
        {
            foreach (char c in Path.GetInvalidPathChars())
            {
                Component = Component.Replace(c, '-');
            }

            Component = Component.Replace(Path.AltDirectorySeparatorChar, '-');
            Component = Component.Replace(Path.DirectorySeparatorChar, '-');
            Component = Component.Replace(Path.PathSeparator, '-');
            Component = Component.Replace(Path.VolumeSeparatorChar, '-');

            return Component;
        }

        static void CopyFile(string SourcePath, string DestinationPath)
        {
            System.Diagnostics.Debug.Assert(Directory.Exists(Path.GetDirectoryName(DestinationPath)));
            File.Copy(SourcePath, DestinationPath);
        }

        //
        // Decompresses the specified File into a directory under %TMP% and returns the path to the
        // resulting directory.
        //
        static string UncompressZipFile(string FileName)
        {
            string OutputDirectory = Path.GetTempPath() + Guid.NewGuid().ToString();

            LogVerboseMessage($"Extracting {FileName} to directory {OutputDirectory}");

            Directory.CreateDirectory(OutputDirectory);
            ZipFile.ExtractToDirectory(FileName, OutputDirectory);

            return OutputDirectory;
        }

        /// <summary>
        /// Scans all the files in a directory looking for one that is an mp3 so it can read its tags to
        /// get the artist name (which we use as the sub-directory name).
        /// </summary>
        /// <param name="Directory">The directory to scan.</param>
        /// <returns>The name of the artist that this directory contains music from.</returns>
        static string DetermineDestinationDirectory(string Directory)
        {
            string SubDir = null;

            foreach (string FileName in System.IO.Directory.EnumerateFiles(Directory))
            {
                if (GetMetadata(FileName, out SubDir))
                {
                    break;
                }
            }

            return SubDir;
        }

    }
}