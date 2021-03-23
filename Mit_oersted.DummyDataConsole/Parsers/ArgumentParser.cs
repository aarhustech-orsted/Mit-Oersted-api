using System;
using System.Collections.Generic;
using System.Linq;

namespace Mit_Oersted.DummyDataConsole.Parsers
{
    internal static class ArgumentParser
    {
        public static List<string> GetArguments(string[] args, string name)
        {
            return GetArgumentValues(args, "/" + name + ":");
        }

        public static string GetArgument(string[] args, string name, string defaultValue)
        {
            return GetArgumentValue(args, "/" + name + ":") ?? defaultValue;
        }

        public static bool GetArgument(string[] args, string name, bool defaultValue)
        {
            string argumentValue = GetArgumentValue(args, "/" + name + ":");

            if (argumentValue != null)
            {
                if (bool.TryParse(argumentValue, out bool result)) return result;
            }

            return defaultValue;
        }

        private static string GetArgumentValue(string[] args, string pattern)
        {
            string argument = args.FirstOrDefault(x => x.StartsWith(pattern, StringComparison.OrdinalIgnoreCase));
            if (argument != null) return argument.Substring(pattern.Length);

            return null;
        }

        private static List<string> GetArgumentValues(string[] args, string pattern)
        {
            IEnumerable<string> argumentValues = args.Where(x => x.StartsWith(pattern, StringComparison.OrdinalIgnoreCase));

            List<string> result = new List<string>();
            if (argumentValues != null) result.AddRange(from string argument in argumentValues select argument.Substring(pattern.Length));

            return result;
        }
    }
}
