using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Lab2.Shared.JsonConverters;
using Microsoft.Extensions.Caching.Distributed;

namespace Lab2.Shared.Extensions {
    public static class DistributedCacheExtensions {
        public static readonly JsonSerializerOptions JsonSerializerOptions = new() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = {
                new VersionConverter(),
                new TimeSpanConverter()
            }
        };

        public static T? GetJson<T>(this IDistributedCache distributedCache, string key) {
            var jsonString = distributedCache.GetString(key);
            return string.IsNullOrWhiteSpace(jsonString) ? default : JsonSerializer.Deserialize<T>(jsonString, JsonSerializerOptions);
        }

        public static void SetJson<T>(this IDistributedCache distributedCache, string key, T value, DistributedCacheEntryOptions? options = null) {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            var jsonString = JsonSerializer.Serialize(value, JsonSerializerOptions);
            distributedCache.SetString(key, jsonString, options ?? new DistributedCacheEntryOptions());
        }

        public static async Task<T?> GetJsonAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken ct = default) {
            var jsonString = await distributedCache.GetStringAsync(key, ct);
            return string.IsNullOrWhiteSpace(jsonString) ? default : JsonSerializer.Deserialize<T>(jsonString, JsonSerializerOptions);
        }

        public static async Task SetJsonAsync<T>(this IDistributedCache        distributedCache,
                                                 string                        key,
                                                 T                             value,
                                                 DistributedCacheEntryOptions? options = null,
                                                 CancellationToken             ct      = default) {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            var jsonString = JsonSerializer.Serialize(value, JsonSerializerOptions);
            await distributedCache.SetStringAsync(key, jsonString, options ?? new DistributedCacheEntryOptions(), ct);
        }
    }
}
