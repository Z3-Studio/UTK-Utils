using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Z3.Utils.ExtensionMethods
{
    public static class CollectionExtensions
    {
        public static T GetRandom<T>(this IList<T> list)
        {
            int randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }

        public static void SetSize<T>(this List<T> list, int size, T emptyElement = default)
        {
            int count = list.Count - size;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                    list[i] = emptyElement;
            }

            if (count > 0)
            {
                list.RemoveRange(size, count);
            }
            else if (count < 0)
            {
                for (int i = 0; i > count; i--)
                    list.Add(emptyElement);
            }
        }

        public static T[,] Resize<T>(this T[,] original, int newX, int newY)
        {
            T[,] resized = new T[newX, newY];
            int originalX = original.GetLength(0);
            int originalY = original.GetLength(1);

            int maxX = originalX > newX ? newX : originalX;
            int maxY = originalY > newY ? newY : originalY;

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    resized[x, y] = original[x, y];
                }
            }

            return resized;
        }

        public static T[] Resize<T>(this T[] original, int newLength)
        {
            T[] resized = new T[newLength];
            int iterations = Mathf.Min(original.Length, newLength);

            for (int i = 0; i < resized.Length; i++)
            {
                if (i < iterations)
                    resized[i] = original[i];
                else
                    resized[i] = default;
            }

            return resized;
        }

        public static T[] Resize2D<T>(this T[] original, Vector2Int originalSize, Vector2Int newSize)
        {
            T[,] array2D = original.ConvertTo2D(originalSize.x, originalSize.y);
            T[,] resized2D = array2D.Resize(newSize.x, newSize.y);
            return resized2D.Flatten();
        }

        public static T[,] ConvertTo2D<T>(this T[] original, int xSize, int ySize)
        {
            T[,] converted = new T[xSize, ySize];
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                    converted[x, y] = original[ySize * x + y];
            }

            return converted;
        }

        public static T[] Flatten<T>(this T[,] original)
        {
            return original.Cast<T>().ToArray();
        }

        public static List<T> ShuffleToList<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Shuffle().ToList();
        }

        public static T[] ShuffleToArray<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Shuffle().ToArray();
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.OrderBy(e => Guid.NewGuid());
        }

        public static Dictionary<T, U> GenerateEnumDictionary<T, U>(this Dictionary<T, U> dictionary, U inicialValue = default) where T : Enum
        {
            dictionary = new Dictionary<T, U>();
            foreach (T enumType in Enum.GetValues(typeof(T)))
            {
                dictionary.Add(enumType, inicialValue);
            }
            return dictionary;
        }
    }
}