using System;
using System.IO;
using System.Collections.Generic;
using System.Speech.Synthesis;
using Mono.Options;

namespace say
{
    class Program
    {
        static void PrintUsage()
        {
            System.Console.WriteLine("usage: say [-f <file>] [-r <rate>] 'text to say'");
            System.Console.WriteLine("\t[-f <filename>] ==> File whose contents are to be read.");
            System.Console.WriteLine("\t[-r <rate>] ==> An integer from [-10, 10] that controls the speaking rate.");

            Environment.Exit(0);
        }

        static void Main(string[] args)
        {
            OptionSet options = new OptionSet();
            string FileName = null;
            List<string> Words = null;
            int SpeakingRate = 0;

            options.Add("?|h|help", value => PrintUsage());
            options.Add("f=", value => FileName = value);
            options.Add("r=", value => SpeakingRate = Convert.ToInt32(value));
            Words = options.Parse(args);

            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                synth.SetOutputToDefaultAudioDevice();
                synth.Rate = SpeakingRate;

                if (string.IsNullOrEmpty(FileName))
                {
                    foreach (string Word in Words)
                    {
                        synth.Speak(Word);
                    }
                }
                else
                {
                    SpeakFile(synth, FileName);
                }
            }
        }

        static void SpeakFile(SpeechSynthesizer Synth, string FileName)
        {
            if (File.Exists(FileName))
            {
                Synth.Speak(System.IO.File.ReadAllText(FileName));
            }
            else
            {
                Console.WriteLine("ERROR: File {0} doesn't exist!", FileName);
            }
        }
    }
}
