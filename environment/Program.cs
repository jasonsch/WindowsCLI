using System;
using System.Collections.Generic;
using Microsoft.Win32;
using Mono.Options;

namespace environment
{
    class Program
    {
        const string UserEnvironmentKey = @"Environment";
        const string MachineEnvironmentKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";

        static void PrintUsage()
        {
            System.Console.WriteLine("Usage: environment [-a] <variable name> [new value]");
            System.Console.WriteLine("\t[-a] ==> Append [new value] to the existing value.");

            Environment.Exit(0);
        }

        static void ShowEnvironmentVariable(string VariableName)
        {
            object RegValue = null;

            using (RegistryKey UserEnvironment = Registry.CurrentUser.OpenSubKey(UserEnvironmentKey))
            {
                RegValue = UserEnvironment.GetValue(VariableName);
                if (RegValue == null)
                {
                    using (RegistryKey MachineEnvironment = Registry.LocalMachine.OpenSubKey(MachineEnvironmentKey))
                    {
                        RegValue = MachineEnvironment.GetValue(VariableName);
                    }
                }
            }

            if (RegValue == null)
            {
                System.Console.WriteLine("Couldn't find environment variable {0}", VariableName);
            }
            else
            {
                System.Console.WriteLine("{0}", RegValue.ToString());
            }
        }

        static void UpdateEnvironmentVariable(string VariableName, string Value, bool Append)
        {
            using (RegistryKey UserEnvironment = Registry.CurrentUser.OpenSubKey(UserEnvironmentKey))
            {

            }
        }

        static void Main(string[] args)
        {
            bool AppendValue = false;

            OptionSet options = new OptionSet();

            options.Add("?|h|help", value => PrintUsage());
            options.Add("a", value => AppendValue = (value != null));
            List<string> parameters = options.Parse(args);
            if (parameters.Count == 1)
            {
                ShowEnvironmentVariable(parameters[0]);
            }
            else if (parameters.Count == 2)
            {
                UpdateEnvironmentVariable(parameters[0], parameters[1], AppendValue);
            }
            else
            {
                PrintUsage();
            }

        }
    }
}

