using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Mono.Options;

namespace regfind
{
    class Program
    {
        static void Main(string[] args)
        {
            OptionSet options = new OptionSet();
            string startPath = null;
            bool searchKeys = false;
            bool searchValues = false;

            options.Add("s|start=", arg => startPath = arg);
            options.Add("k|search-keys", arg => searchKeys = true);
            options.Add("v|search-values", arg => searchValues = true);

            List<string> searchTerms = options.Parse(args);

            if (searchTerms.Count != 1)
            {
                PrintUsage();
            }

            RegistryKey[] keys = KeysFromPath(startPath);
            if (keys == null)
            {
                PrintUsage($"Invalid path: {startPath}");
            }

            SearchRegistry(keys, searchKeys, searchValues, searchTerms[0]);
        }

        private static RegistryKey[] KeysFromPath(string startPath)
        {
            if (startPath == null)
            {
                return new RegistryKey[] { Registry.CurrentUser, Registry.LocalMachine, Registry.ClassesRoot, Registry.Users };
            }

            return new RegistryKey[] { ResolveRegistryPath(startPath) };
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

        private static void SearchRegistry(RegistryKey[] keys, bool searchKeys, bool searchValues, string term)
        {
            foreach (RegistryKey key in keys)
            {
                SearchRegistryKey(key, searchKeys, searchValues, term);
            }
        }

        private static void SearchRegistryKey(RegistryKey key, bool searchKeys, bool searchValues, string term)
        {
            foreach (var keyName in key.GetValueNames())
            {
                if (searchKeys)
                {
                    if (Contains(keyName, term))
                    {
                        PrintSearchHit(key, keyName, key.GetValueKind(keyName));
                        continue;
                    }
                }

                if (searchValues)
                {
                    var valueKind = key.GetValueKind(keyName);
                    if (valueKind == RegistryValueKind.ExpandString || valueKind == RegistryValueKind.MultiString || valueKind == RegistryValueKind.String)
                    {
                        object value = key.GetValue(keyName);

                        if (Contains(value as string, term))
                        {
                            PrintSearchHit(key, keyName, valueKind);
                        }
                    }
                }
            }

            foreach (var childKey in key.GetSubKeyNames())
            {
                RegistryKey subKey;

                if (Contains(childKey, term))
                {
                    PrintSearchHit(key, childKey, RegistryValueKind.None);
                }

                try
                {
                    subKey = key.OpenSubKey(childKey);
                }
                catch (System.Security.SecurityException)
                {
                    continue;
                }

                SearchRegistryKey(subKey, searchKeys, searchValues, term);
            }
        }

        private static void PrintSearchHit(RegistryKey key, string keyName, RegistryValueKind valueKind)
        {
            Console.WriteLine($"{key.ToString()}\\{keyName} ({valueKind})");
        }

        private static bool Contains(string keyName, string term)
        {
            return Regex.IsMatch(keyName, term);
        }

        private static void PrintUsage(string errorMessage = null)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                Console.WriteLine("Invalid arguments!");
            }
            else
            {
                Console.WriteLine(errorMessage);
            }

            Console.WriteLine("regfind.exe [-s] [-k] [-v] <search term>");
            Console.WriteLine("-s / -start => Path of reg key to start search from");
            Console.WriteLine("-k / -search-keys => Search for the term in the name of the keys");
            Console.WriteLine("-v / -search-values => Search for the term in the keys' values");
            Environment.Exit(0);
        }
    }
}
