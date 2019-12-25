using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowLab.Windows.GUI;
using System.IO;

namespace clipfile
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                // No parameter => copy current directory to clipboard
                Clipboard.files = new string[] { Path.GetFullPath(".") };
            }
            else
            {
                List<string> files = new List<string>();

                // TODO -- Accept wildcards.
                for (int i = 0; i < args.Length; ++i)
                {
                    files.Add(Path.GetFullPath(args[i]));
                }

                Clipboard.files = files;
            }
        }
    }
}
