using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Z3.Utils.ExtensionMethods;

namespace Z3.Utils
{
    public static class StringUtils
    {
        private static Dictionary<Type, string> cachedNiceString = new();

        public static int GetNumberPart(string text)
        {
            string fullNumber = text
                .Where(character => int.TryParse(character.ToString(), out int _))
                .Aggregate(string.Empty, (current, character) => current + character);

            return int.Parse(fullNumber);
        }

        public static string CombinePaths(params string[] paths)
        {
            char separator = Path.DirectorySeparatorChar;
            string finalPath = string.Empty;

            for (int index = 0; index < paths.Length; index++)
            {
                string path = paths[index];
                finalPath += path;

                if (index != paths.Length - 1)
                    finalPath += separator;
            }

            return finalPath;
        }

        public static string GetTypeNiceString(object obj) => GetTypeNiceString(obj.GetType());

        public static string GetTypeNiceString<T>() => GetTypeNiceString(typeof(T));

        public static string GetTypeNiceString(Type type)
        {
            if (cachedNiceString.TryGetValue(type, out string result))
                return result;

            result = type.Name.GetNiceString();
            cachedNiceString.Add(type, result);

            return result;
        }
    }
}