using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowLab.Win32;
using Mono.Options;

namespace injectkey
{
    class Program
    {
        static void Main(string[] args)
        {

            OptionSet options = new OptionSet();
            char? character = null;
            bool keyUp = false;
            bool isVK = false;

            options.Add("?|h|help", value => PrintUsage());
            options.Add("u|key-up", value => keyUp = true);
            options.Add("l|literal=", value => { character = ParseLiteral(value); isVK = true; });

            var parameters = options.Parse(args);
            if (!character.HasValue)
            {
                if (parameters.Count != 1)
                {
                    PrintUsage();
                }

                character = parameters[0][0];
            }

            InjectKeyboardInput(character.Value, keyUp, isVK);
        }

        private static void InjectKeyboardInput(char character, bool keyUp, bool isVK)
        {
            INPUT i;

            if (isVK)
            {
                i = INPUT.CreateKeyboardInput((short)character, keyUp);
            }
            else
            {
                i = INPUT.CreateKeyboardInput2((short)character, keyUp);
            }

            Win32Interop.InjectKeyStrokes(new INPUT[] { i });
        }

        private static char ParseLiteral(string value)
        {
            value = value.ToUpper();
            if (value == "ALT" || value == "VK_ALT")
            {
                return (char)0x12;
            }
            else if (value == "CTRL" || value == "VK_CONTROL")
            {
                return (char)0x11;
            }
            else if (value == "SHIFT" || value == "VK_SHIFT")
            {
                return (char)0x10;
            }
            else if (value == "WIN" || value == "VK_WIN")
            {
                return (char)0x5B;
            }

            throw new ArgumentException($"Invalid character literal '{value}'");
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Invalid usage!");
            Environment.Exit(0);
        }
    }
}
