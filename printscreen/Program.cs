using System;
using System.IO;
using System.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Mono.Options;
using YellowLab.GUI;

namespace printscreen
{
    class Program
    {
        const string FileNameFormatter = "Screen Shot {0} at {1}.png";
        const string DateFormatter = "yyyy-MM-dd";
        const string TimeFormatter = "h.mm.ss tt";

        static void InjectPrintscreenKeySequence()
        {
            InputInjector.InjectKey(Key.PrintScreen);
        }

        static string GetOutputFilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\";
        }

        static string GenerateFileName()
        {
            DateTime Time = DateTime.Now;

            return GetOutputFilePath() + String.Format(FileNameFormatter, Time.ToString(DateFormatter), Time.ToString(TimeFormatter));
        }

        static void PrintUsage()
        {
            System.Console.WriteLine("PrintScreen.exe [-f] [-filename=<file>] [-t=<time>]");
            System.Console.WriteLine("-f ==> Save image to desktop.");
            System.Console.WriteLine("-filename=<file> ==> Save image to the specified file.");
            System.Console.WriteLine("-timeout=<time> ==> Wait 'time' seconds before taking the screenshot.");

            Environment.Exit(0);
        }

        //
        // The image saving code requires that our thread be annotated this way.
        //
        [STAThread]
        static void Main(string[] args)
        {
            OptionSet Options = new OptionSet();
            bool SaveToFile = false;
            string FileName = null;
            int SleepDuration = 0;

            Options.Add("?|h|help", value => PrintUsage());
            Options.Add("f", value => SaveToFile = (value != null));
            Options.Add("filename=", value => FileName = value);
            Options.Add("timeout=", value => SleepDuration = Convert.ToInt32(value));

            Options.Parse(args);

            if (SleepDuration != 0)
            {
                System.Threading.Thread.Sleep(SleepDuration * 1000);
            }


            InjectPrintscreenKeySequence();

            using (Stream stream = printscreen.Properties.Resources.CameraSnap)
            {
                new SoundPlayer(stream).PlaySync();
            }

            if (SaveToFile || FileName != null)
            {
                SaveImageToFile(FileName);
            }
        }

        private static void SaveImageToFile(string FileName)
        {
            if (FileName == null)
            {
                FileName = GenerateFileName();
            }

            BitmapSource source = System.Windows.Clipboard.GetImage();

            using (var fileStream = new FileStream(FileName, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(fileStream);
            }
        }
    }
}
