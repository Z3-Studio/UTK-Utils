using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Z3.Utils
{
    public static class CollectionUtils
    {
        public static Vector2Int[] IntSides = { Vector2Int.right, Vector2Int.down, Vector2Int.left, Vector2Int.up };
        public static Vector2Int[] IntCorners = { new Vector2Int(-1, 1), Vector2Int.one, new Vector2Int(1, -1), -Vector2Int.one };

        public static T[] CreateRepeatedArray<T>(int count, T element)
        {
            T[] repeated = new T[count];

            for (int i = 0; i < count; i++)
                repeated[i] = element;

            return repeated;
        }

        public static T GetElementAndRemove<T>(this List<T> list, int index)
        {
            T element = list[index];
            list.RemoveAt(index);
            return element;
        }

        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            bool isNull = list == null;
            if (isNull) return true;

            bool isEmpty = list.Count == 0;
            return isEmpty;
        }

        public static T[] MergeArrays<T>(T[] first, T[] second)
        {
            T[] mergedArray = new T[first.Length + second.Length];
            first.CopyTo(mergedArray, 0);
            second.CopyTo(mergedArray, first.Length);
            return mergedArray;
        }


        public static Type GetElementType(IEnumerable iList)
        {
            Type listType = iList.GetType();
            if (listType.IsArray)
            {
                return listType.GetElementType();
            }
            else if (listType.IsGenericType)
            {
                return listType.GetGenericArguments()[0];
            }
            else
            {
                throw new InvalidOperationException("iList is not an array or a generic list.");
            }
        }

        public static int ClearNullItems<T>(IList<T> source)
        {
            return ClearNullItems(source, (item) => item == null);
        }

        public static int ClearNullObject<T>(IList<T> source) where T : UnityEngine.Object
        {
            return ClearNullItems(source, (item) => item == null);
        }

        public static int ClearNullItems<T>(IList<T> source, Func<T, bool> checkMethod)
        {
            int nullCount = 0;
            int i = 0;
            foreach (T item in source.ToList())
            {
                if (checkMethod(item))
                {
                    nullCount++;
                    source.RemoveAt(i);
                    continue;
                }

                i++;
            }

            return nullCount;
        }

        public static void ClearNull<T>(IList<T> source)
        {
            int i = 0;
            foreach (T item in source.ToList())
            {
                if (item == null)
                {
                    source.RemoveAt(i);
                    continue;
                }

                i++;
            }
        }

        public static string GetCollectionTypeName(IEnumerable enumerable) // Review it
        {
            Type listType = enumerable.GetType();
            if (listType.IsArray)
            {
                return "Array of " + listType.GetElementType().Name;
            }
            else if (listType.IsGenericType)
            {
                Type[] interfaces = listType.GetInterfaces();
                if (interfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    return "List of " + listType.GetGenericArguments()[0].Name;
                }
                else if (interfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    return "IEnumerable of " + listType.GetGenericArguments()[0].Name;
                }
                else if (interfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                {
                    return "Dictionary of " + listType.GetGenericArguments()[0].Name + ", " + listType.GetGenericArguments()[1].Name;
                }
                else if (interfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(HashSet<>)))
                {
                    return "HashSet of " + listType.GetGenericArguments()[0].Name;
                }
            }

            if (listType == typeof(Queue))
            {
                return "Queue";

            }
            else if (listType == typeof(Stack))
            {
                return "Stack";

            }

            throw new InvalidOperationException("The input enumerable is not a recognized collection type.");
        }
    }
}