using System;
using System.Collections.Generic;
using UnityEngine;

namespace Z3.Utils
{
    public static class RandomUtils
    {
        public static T PickWeighted<T>(IReadOnlyList<T> items, float random01, Func<T, float> weightSelector)
        {
            float totalWeight = 0f;

            foreach (T item in items)
            {
                float weight = weightSelector(item);

                if (weight > 0f)
                    totalWeight += weight;
            }

            float pick = Mathf.Clamp(random01, 0f, 1f) * totalWeight;
            float cumulative = 0f;

            foreach (T item in items)
            {
                float weight = weightSelector(item);

                if (weight <= 0f)
                    continue;

                cumulative += weight;

                if (pick <= cumulative)
                    return item;
            }


            return items[items.Count - 1];
        }

        public static T Pick<T>(IReadOnlyList<T> items, float random01)
        {
            int index = (int)(Mathf.Clamp(random01, 0f, 0.999999f) * items.Count);
            return items[index];
        }

        public static float RandomSeeded(int seed)
        {
            if (seed == 0)
                return UnityEngine.Random.value;

            return SimpleHashTo01(seed);
        }

        public static float SimpleHashTo01(int seed)
        {
            const float Normalize24Bits = 1 << 24;

            unchecked
            {
                uint value = (uint)seed;

                value ^= value >> 16;
                value *= 0x85EBCA6B;
                value ^= value >> 13;
                return (value & 0x00FFFFFF) / Normalize24Bits;
            }
        }
    }
}
