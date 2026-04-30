using System;
using System.Collections.Generic;
using System.Linq;

namespace Z3.Utils.ExtensionMethods
{
    public static class SearchString
    {
        public static List<T> SequentialSearch<T>(
            this IEnumerable<T> source,
            string query,
            Func<T, string> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                return source.ToList();
            }

            string normalizedQuery = Normalize(query);

            List<Result<T>> results = new List<Result<T>>();

            foreach (T item in source)
            {
                string candidate = selector(item);

                if (string.IsNullOrEmpty(candidate))
                {
                    continue;
                }

                int score = ScoreSequential(candidate, normalizedQuery);

                if (score >= 0)
                {
                    results.Add(new Result<T>(item, score));
                }
            }

            results.Sort((a, b) => b.Score.CompareTo(a.Score));

            List<T> finalList = new List<T>(results.Count);

            for (int i = 0; i < results.Count; i++)
            {
                finalList.Add(results[i].Item);
            }

            return finalList;
        }

        private static int ScoreSequential(string candidate, string query)
        {
            string text = Normalize(candidate);

            int queryIndex = 0;
            int lastMatchIndex = -1;
            int totalGap = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (queryIndex >= query.Length)
                {
                    break;
                }

                char textChar = text[i];
                char queryChar = query[queryIndex];

                if (textChar == queryChar)
                {
                    if (lastMatchIndex >= 0)
                    {
                        int gap = i - lastMatchIndex - 1;
                        totalGap += gap;
                    }

                    lastMatchIndex = i;
                    queryIndex++;
                }
            }

            if (queryIndex != query.Length)
            {
                return -1;
            }

            int score = 1000 - totalGap;
            return score;
        }

        private static string Normalize(string value)
        {
            char[] buffer = new char[value.Length];
            int count = 0;

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                if (char.IsLetterOrDigit(c))
                {
                    buffer[count] = char.ToLowerInvariant(c);
                    count++;
                }
            }

            return new string(buffer, 0, count);
        }

        private struct Result<T>
        {
            public T Item;
            public int Score;

            public Result(T item, int score)
            {
                Item = item;
                Score = score;
            }
        }
    }
}
