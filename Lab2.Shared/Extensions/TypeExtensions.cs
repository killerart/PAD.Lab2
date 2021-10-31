using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab2.Shared.Extensions {
    public static class StringExtensions {
        // ReSharper disable once InconsistentNaming
        public static string MD5(this string str) {
            using var md5 = System.Security.Cryptography.MD5.Create();

            var inputBytes = Encoding.ASCII.GetBytes(str);
            var hashBytes  = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var t in hashBytes)
                sb.Append(t.ToString("x2"));

            return sb.ToString();
        }

        public static string Reversed(this string str) {
            return string.Create(str.Length,
                                 str,
                                 static (reversed, initial) => {
                                     for (int i = 0; i < reversed.Length; i++) {
                                         reversed[i] = initial[initial.Length - i - 1];
                                     }
                                 });
        }

        public static byte[] ToPlainBytes(this string str) {
            return Encoding.Default.GetBytes(str);
        }
    }

    public static class ByteArrayExtensions {
        public static string ConvertToString(this byte[] array) {
            return Encoding.Default.GetString(array);
        }
    }

    public static class DateTimeExtensions {
        public static long ToUnixTimestamp(this DateTime dateTime) {
            return (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).Ticks / TimeSpan.TicksPerSecond;
        }

        public static DateTime ToUtcFromUnixTimestamp(this long seconds) {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
        }

        public static int Age(this DateTime birthDate) {
            var now = DateTime.Now;

            var age = now.Year - birthDate.Year;

            if (birthDate > now.AddYears(-age))
                age--;

            return age;
        }
    }

    public static class RandomExtensions {
        public static long NextLong(this Random random, long min = 0, long max = long.MaxValue) {
            var buf = new byte[8];
            random.NextBytes(buf);
            var longRand = BitConverter.ToInt64(buf, 0);
            return Math.Abs(longRand % (max - min)) + min;
        }

        public static double NextDouble(this Random randGenerator, double minValue, double maxValue) {
            return randGenerator.NextDouble() * (maxValue - minValue) + minValue;
        }
    }

    public static class ListExtensions {
        public static T? ExtractRandom<T>(this List<T> list, Random? random = null) {
            if (list.Count == 0)
                return default;
            random ??= new Random();
            var randomIndex   = random.Next(list.Count);
            var randomElement = list[randomIndex];
            list.RemoveAt(randomIndex);
            return randomElement;
        }
    }

    public static class DoubleExtensions {
        public static double ToRadians(this double val) {
            return Math.PI / 180.0 * val;
        }
    }

    public static class EnumerableExtensions {
        public static SortedDictionary<TKey, T> ToSortedDictionary<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector)
            where TKey : notnull {
            return new SortedDictionary<TKey, T>(enumerable.ToDictionary(keySelector));
        }

        public static SortedDictionary<TKey, T> ToSortedDictionary<T, TKey>(this IEnumerable<T>     enumerable,
                                                                            Func<T, TKey>           keySelector,
                                                                            IEqualityComparer<TKey> equalityComparer) where TKey : notnull {
            return new SortedDictionary<TKey, T>(enumerable.ToDictionary(keySelector, equalityComparer));
        }

        public static SortedDictionary<TKey, TElement> ToSortedDictionary<T, TKey, TElement>(this IEnumerable<T> enumerable,
                                                                                             Func<T, TKey>       keySelector,
                                                                                             Func<T, TElement>   elementSelector) where TKey : notnull {
            return new SortedDictionary<TKey, TElement>(enumerable.ToDictionary(keySelector, elementSelector));
        }

        public static SortedDictionary<TKey, TElement> ToSortedDictionary<T, TKey, TElement>(this IEnumerable<T>     enumerable,
                                                                                             Func<T, TKey>           keySelector,
                                                                                             Func<T, TElement>       elementSelector,
                                                                                             IEqualityComparer<TKey> equalityComparer)
            where TKey : notnull {
            return new SortedDictionary<TKey, TElement>(enumerable.ToDictionary(keySelector, elementSelector, equalityComparer));
        }

        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> enumerable) {
            return new SortedSet<T>(enumerable.ToHashSet());
        }

        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> equalityComparer) {
            return new SortedSet<T>(enumerable.ToHashSet(equalityComparer));
        }
    }
}
