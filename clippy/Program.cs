using System;
using System.IO;
using Mono.Options;
using YellowLab.Windows.GUI;

namespace clippy
{
    class Program
    {
        static void Main(string[] args)
        {
            bool Paste = false;
            OptionSet Options = new OptionSet();

            Options.Add("p|paste", value => Paste = true);

            Options.Parse(args);

            if (Paste)
            {
                Console.WriteLine(Clipboard.Text);
            }
            else
            {
                Clipboard.Text = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
        }
    }
}

