using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace regprint
{
    class Program
    {
        static void Main(string[] args)
        {
            RegistryKey key = ResolveRegistryPath(args[0]);

            PrintRegistryKey(key);
        }

        private static void PrintRegistryKey(RegistryKey key, uint tabLevel = 0)
        {
            PrintTabs(tabLevel);
            Console.WriteLine($"{key}");

            foreach (var valueName in key.GetValueNames())
            {
                PrintTabs(tabLevel + 1);
                Console.WriteLine($"{(valueName.Length == 0 ? "(Default)" : valueName)} -> {key.GetValue(valueName)} ({key.GetValueKind(valueName)})");
            }

            foreach (var subkeyName in key.GetSubKeyNames())
            {
                PrintRegistryKey(OpenSubKeySafe(key, subkeyName), tabLevel + 1);
            }
        }

        private static void PrintTabs(uint tabLevel)
        {
            for (uint i = 0; i < tabLevel; ++i)
            {
                Console.Write("\t");
            }
        }

        private static RegistryKey ResolveRegistryPath(string path)
        {
            string[] pathComponents = path.Split('\\');

            RegistryKey key = ResolveRegistryHive(pathComponents[0]);
            if (key == null)
            {
                return null;
            }

            for (int i = 1; i < pathComponents.Length; ++i)
            {
                key = OpenSubKeySafe(key, pathComponents[i]);
                if (key == null)
                {
                    return null;
                }
            }

            return key;
        }

        private static RegistryKey OpenSubKeySafe(RegistryKey key, string subKeyName)
        {
            RegistryKey subKey = null;

            try
            {
                subKey = key.OpenSubKey(subKeyName);
            }
            catch (System.Security.SecurityException)
            {
            }

            return subKey;
        }

        private static RegistryKey ResolveRegistryHive(string path)
        {
            path = path.ToUpper();

            if (path == "HKCU" || path == "HKEY_CURRENT_USER")
            {
                return Registry.CurrentUser;
            }
            else if (path == "HKLM" || path == "HKEY_LOCAL_MACHINE")
            {
                return Registry.LocalMachine;
            }
            else if (path == "HKCR" || path == "HKEY_CLASSES_ROOT")
            {
                return Registry.ClassesRoot;
            }
            else if (path == "HKU" || path == "HKEY_USERS")
            {
                return Registry.Users;
            }
            else if (path == "HKCC" || path == "HKEY_CURRENT_CONFIG")
            {
                return Registry.CurrentConfig;
            }

            return null;
        }
    }
}
