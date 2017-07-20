using System.Collections.Generic;
using System.Linq;

namespace AndyTools.Utilities
{
    using System;

    public static class Extensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            if (val.CompareTo(max) > 0) return max;
            return val;
        }

        public static bool IsEven(this int value)
        {
            return value % 2 == 0;
        }

        public static bool In2DArrayBounds<T>(this T[,] array, int x, int y)
        {
            var result = true;

            if (x < array.GetLowerBound(0) ||
                x > array.GetUpperBound(0) ||
                y < array.GetLowerBound(1) ||
                y > array.GetUpperBound(1))
            {
                result = false;
            }

            return result;
        }

        public static List<T> ShiftAndWrap<T>(this List<T> list, int shiftCount)
        {
            if (shiftCount == 0 || shiftCount == list.Count)
            {
                return list.ToList();
            }

            if (shiftCount < 0 || shiftCount > list.Count)
            {
                throw new ArgumentOutOfRangeException("shiftCount", "The shift amount must be non-negative and not exceed the size of the list.");
            }

            var newList = list.ToList();
            var tempList = newList.GetRange(newList.Count - shiftCount, shiftCount);
            newList.RemoveRange(newList.Count - shiftCount, shiftCount);
            newList.InsertRange(0, tempList);

            return newList;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            var buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
    }
}
