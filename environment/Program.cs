using System;
using System.Collections.Generic;
using Microsoft.Win32;
using Mono.Options;
using YellowLab.Windows.Win32;

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
            using (RegistryKey UserEnvironment = Registry.CurrentUser.OpenSubKey(UserEnvironmentKey, true))
            {
                if (Append)
                {
                    Value = UserEnvironment.GetValue(VariableName, "") as string + Value;
                }

                UserEnvironment.SetValue(VariableName, Value);
            }

            BroadcastSettingChangeMessage();
        }

        private static void DeleteEnvironmentVariable(string name)
        {
            using (RegistryKey UserEnvironment = Registry.CurrentUser.OpenSubKey(UserEnvironmentKey, true))
            {
                UserEnvironment.DeleteValue(name);
            }

            BroadcastSettingChangeMessage();
        }

        //
        // This cases explorer.exe to pickup the new registry settings so processes that it creates going forward reflect
        // the new settings.
        //
        private static void BroadcastSettingChangeMessage()
        {
            IntPtr result;

            Win32Interop.SendMessageTimeout(Win32Interop.HWND_BROADCAST, 0x1A, IntPtr.Zero, "Environment", 2, 2000, out result);
        }


        static void Main(string[] args)
        {
            bool appendValue = false;
            bool deleteValue = false;

            OptionSet options = new OptionSet();

            options.Add("?|h|help", value => PrintUsage());
            options.Add("a|append", value => appendValue = true);
            options.Add("d|delete", value => deleteValue = true);

            List<string> parameters = options.Parse(args);
            if (parameters.Count == 1)
            {
                if (deleteValue)
                {
                    DeleteEnvironmentVariable(parameters[0]);
                }
                else
                {
                    ShowEnvironmentVariable(parameters[0]);
                }
            }
            else if (parameters.Count == 2)
            {
                UpdateEnvironmentVariable(parameters[0], parameters[1], appendValue);
            }
            else
            {
                PrintUsage();
            }

        }
    }
}
