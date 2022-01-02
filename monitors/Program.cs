using System;
using System.Windows.Forms;

namespace monitors
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                Console.WriteLine($"screen ==> {screen.WorkingArea} / {screen.Bounds} / {screen.DeviceName} / {screen.Primary}");
            }
        }
    }
}
