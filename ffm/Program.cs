using System;
using YellowLab.Windows.Win32;
using Mono.Options;

namespace ffm
{
    class Program
    {
        static void PrintUsage()
        {
            System.Console.WriteLine("usage: ffm [-t] [-timeout=<value>] [-z] [-p]");
            System.Console.WriteLine("\t[-t(+/-)] ==> Enable/disable active window tracking");
            System.Console.WriteLine("\t[-timeout=<value>] ==> Set tracking timeout to \"value\" (in milliseconds)");
            System.Console.WriteLine("\t[-z(+/-)] ==> Bring active window to foreground or not");
            System.Console.WriteLine("\t[-p(+/-)] ==> Persist the system changes");

            Environment.Exit(0);
        }

        static void Main(string[] args)
        {
            int Timeout = 300; // In milliseconds
            bool EnableActiveWindowTracking = true;
            bool EnableActiveWindowZordering = true;
            bool PersistSystemSettings = false;
            bool ArgumentsPassed = false;

            OptionSet options = new OptionSet();

            options.Add("?|h|help", value => PrintUsage());
            options.Add("t", value => EnableActiveWindowTracking = (value != null));
            options.Add("timeout=", value => Timeout = Int32.Parse(value));
            options.Add("z", value => EnableActiveWindowZordering = (value != null));
            options.Add("p", value => PersistSystemSettings = (value != null));
            options.Parse(args);

            if (ArgumentsPassed)
            {
                Win32Interop.SetSystemParameter(YellowLab.Windows.Win32.Win32Interop.SPI_SETACTIVEWINDOWTRACKING, EnableActiveWindowTracking, PersistSystemSettings);
                Win32Interop.SetSystemParameter(YellowLab.Windows.Win32.Win32Interop.SPI_SETACTIVEWNDTRKZORDER, EnableActiveWindowZordering, PersistSystemSettings);
                Win32Interop.SetSystemParameter(YellowLab.Windows.Win32.Win32Interop.SPI_SETACTIVEWNDTRKTIMEOUT, Timeout, PersistSystemSettings);
            }
            else
            {
                PrintFocusFollowsMouseSettings();
            }
        }

        private static void PrintFocusFollowsMouseSettings()
        {
            bool bEnabled;
            bool bZorderEnabled;
            int uTimeout;

            Win32Interop.GetSystemParameter(YellowLab.Windows.Win32.Win32Interop.SPI_GETACTIVEWINDOWTRACKING, out bEnabled);
            Console.WriteLine("Enabled ==> " + bEnabled);

            Win32Interop.GetSystemParameter(YellowLab.Windows.Win32.Win32Interop.SPI_GETACTIVEWNDTRKZORDER, out bZorderEnabled);
            Console.WriteLine("Z-order enabled ==> " + bZorderEnabled);

            Win32Interop.GetSystemParameter(YellowLab.Windows.Win32.Win32Interop.SPI_GETACTIVEWNDTRKTIMEOUT, out uTimeout);
            Console.WriteLine("Timeout ==> " + uTimeout);
        }
    }
}
