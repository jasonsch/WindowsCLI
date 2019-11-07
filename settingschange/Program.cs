using System;
using YellowLab.Windows.Win32;

namespace settingschange
{
    class Program
    {
        static void Main(string[] args)
        {
            IntPtr result;

            Win32Interop.SendMessageTimeout(Win32Interop.HWND_BROADCAST, 0x1A, IntPtr.Zero, "Environment", 2, 2000, out result);
        }
    }
}
