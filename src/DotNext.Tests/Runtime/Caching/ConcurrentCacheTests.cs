using System.Diagnostics.CodeAnalysis;

namespace DotNext.Runtime.Caching
{
    [ExcludeFromCodeCoverage]
    public sealed class ConcurrentCacheTests : Test
    {
        [Fact]
        public static void DefaultKeyComparer()
        {
            var cache = new ConcurrentCache<int, string>(10);
            True(cache.TryAdd(0, "0"));
            True(cache.TryAdd(1, "1"));

            Equal("0", cache[0]);
            Equal("1", cache[1]);

            False(cache.TryGetValue(2, out _));
            Equal(2, cache.Count);

            True(cache.TryRemove(0, out var actual));
            Equal("0", actual);
            Equal(1, cache.Count);

            False(cache.TryGetValue(0, out _));

            cache[1] = "42";
            Equal("42", cache[1]);
        }

        [Fact]
        public static void CustomKeyComparer()
        {
            var cache = new ConcurrentCache<string, Guid>(10, CacheEvictionPolicy.LFU, StringComparer.OrdinalIgnoreCase);
            var value1 = Guid.NewGuid();
            var value2 = Guid.NewGuid();

            True(cache.TryAdd(value1.ToString(), value1));
            True(cache.TryAdd(value2.ToString(), value2));

            Equal(value1, cache[value1.ToString()]);
            Equal(value2, cache[value2.ToString()]);

            False(cache.TryGetValue(string.Empty, out _));
            Equal(2, cache.Count);

            True(cache.TryRemove(value1.ToString(), out var actual));
            Equal(value1, actual);
            Equal(1, cache.Count);

            False(cache.TryGetValue(value1.ToString(), out _));

            cache[value2.ToString()] = Guid.Empty;
            Equal(Guid.Empty, cache[value2.ToString()]);
        }

        [Fact]
        public static void Overflow()
        {
            var cache = new ConcurrentCache<int, string>(3, 3);
            var evictionList = new List<KeyValuePair<int, string>>();
            cache.OnEviction += (key, value) => evictionList.Add(new(key, value));

            Equal("0", cache.AddOrUpdate(0, "0", out var added));
            True(added);

            Equal("1", cache.GetOrAdd(1, "1", out added));
            True(added);

            Equal("1", cache.GetOrAdd(1, "42", out added));
            False(added);

            Equal("2", cache.AddOrUpdate(2, "2", out added));
            True(added);

            Equal("3", cache.AddOrUpdate(3, "3", out added));
            True(added);

            NotEmpty(evictionList);
            Equal(0, evictionList[0].Key);
            Equal("0", evictionList[0].Value);
        }

        [Fact]
        public static void Enumerators()
        {
            var cache = new ConcurrentCache<int, string>(3, 3);
            cache[0] = "0";
            cache[1] = "1";
            cache[2] = "2";

            IReadOnlyDictionary<int, string> dictionary = cache;
            True(dictionary.ContainsKey(0));
            False(dictionary.ContainsKey(3));

            Equal(new int[] { 0, 1, 2 }, dictionary.Keys.ToArray());
            Equal(new string[] { "0", "1", "2" }, dictionary.Values.ToArray());
            Equal(new KeyValuePair<int, string>[] { new(0, "0"), new(1, "1"), new(2, "2") }, dictionary.ToArray());

            cache.Clear();
            Equal(0, cache.Count);
            False(dictionary.ContainsKey(0));
        }
    }
}