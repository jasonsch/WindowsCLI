using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace attr
{
    class Program
    {
        static void PrintUsage(string ErrorMessage = null)
        {
            Console.WriteLine("attr: ");
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                Console.WriteLine("Invalid parameters!");
            }
            else
            {
                Console.WriteLine(ErrorMessage);
            }

            Console.WriteLine("Usage: attr <[+attr]+|[-attr]+> <files>");
            Environment.Exit(0);
        }

        class AttributeOperation
        {
            // True if we're adding the attribute, false if we're removing it.
            public bool addAttribute;

            public string attribute;
        }


        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
            }

            var (fileNames, attributes) = ParseCommandLine(args);
            if (attributes.Count == 0)
            {
                PrintUsage("No attributes specified!");
            }
            else if (fileNames.Count == 0)
            {
                PrintUsage("No files specified!");
            }

            ApplyAttributes(fileNames, attributes);
        }

        private static void ApplyAttributes(List<string> fileNames, List<AttributeOperation> attributes)
        {
            // TODO
        }

        static (List<string>, List<AttributeOperation>) ParseCommandLine(string[] args)
        {
            List<AttributeOperation> attributes = new List<AttributeOperation>();
            List<string> fileNames = new List<string>();

            foreach (string arg in args)
            {
                if (arg.StartsWith("+"))
                {
                    attributes.Add(new AttributeOperation() { addAttribute = true, attribute = arg.Substring(1) });
                }
                else if (arg.StartsWith("-"))
                {
                    attributes.Add(new AttributeOperation() { addAttribute = false, attribute = arg.Substring(1) });
                }
                else
                {
                    fileNames.Add(arg);
                }
            }

            return (fileNames, attributes);
        }
    }
}
