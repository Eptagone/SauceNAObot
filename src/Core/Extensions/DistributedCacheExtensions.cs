// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace SauceNAO.Core.Extensions;

public static class DistributedCacheExtensions
{
    /// <summary>
    /// Asynchronously gets the value associated with this key if it exists, or generates a new entry using the provided key and a value from the given factory if the key is not found.
    /// </summary>
    /// <param name="cache">The <see cref="IDistributedCache"/> instance this method extends.</param>
    /// <param name="key">The key of the entry to look for or create.</param>
    /// <param name="factory">The factory task that creates the value associated with this key if the key does not exist in the cache.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public static async Task<TItem?> GetOrCreateAsync<TItem>(
        this IDistributedCache cache,
        string key,
        Func<DistributedCacheEntryOptions, Task<TItem>> factory,
        CancellationToken cancellationToken = default
    )
    {
        if (typeof(TItem) == typeof(string))
        {
            var item = await cache.GetStringAsync(key, cancellationToken);
            if (item is null)
            {
                var options = new DistributedCacheEntryOptions();
                item = (await factory(options))?.ToString() ?? string.Empty;
                await cache.SetStringAsync(key, item, options, cancellationToken);
            }

            return string.IsNullOrEmpty(item) ? default : (TItem)(object)item;
        }

        var itemBytes = await cache.GetAsync(key, cancellationToken);
        if (typeof(TItem) == typeof(bool))
        {
            if (itemBytes is null)
            {
                var options = new DistributedCacheEntryOptions();
                var value = await factory(options);
                itemBytes = value is null ? [] : (value is true ? [1] : [0]);
                await cache.SetAsync(key, itemBytes, options, cancellationToken);
                return value;
            }

            return itemBytes.Length == 0 ? default : (TItem)(object)(itemBytes[0] == 1);
        }

        if (itemBytes is null)
        {
            var options = new DistributedCacheEntryOptions();
            var value = await factory(options);
            itemBytes = JsonSerializer.SerializeToUtf8Bytes(value);
            await cache.SetAsync(key, itemBytes, options, cancellationToken);
            return value;
        }

        return JsonSerializer.Deserialize<TItem>(itemBytes)!;
    }

    /// <summary>
    /// Sets a value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="options">The cache options for the value.</param>
    public static TItem? Set<TItem>(
        this IDistributedCache cache,
        string key,
        TItem value,
        DistributedCacheEntryOptions? options = null
    )
        where TItem : class
    {
        var itemBytes = JsonSerializer.SerializeToUtf8Bytes(value);
        cache.Set(key, itemBytes, options ?? new DistributedCacheEntryOptions());
        return value;
    }

    /// <summary>
    /// Try to get the value associated with the given key.
    /// </summary>
    /// <typeparam name="TItem">The type of the object to get.</typeparam>
    /// <param name="cache">The <see cref="IDistributedCache"/> instance this method extends.</param>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">The value associated with the given key.</param>
    /// <returns><c>true</c> if the key was found. <c>false</c> otherwise.</returns>
    [Obsolete("Use GetAsync instead")]
    public static bool TryGetValue<TItem>(
        this IDistributedCache cache,
        string key,
        out TItem? value
    )
        where TItem : class
    {
        var itemBytes = cache.Get(key);
        if (itemBytes is null)
        {
            value = default;
            return false;
        }

        value = JsonSerializer.Deserialize<TItem>(itemBytes);
        return true;
    }

    /// <summary>
    /// Asynchronously gets the value associated with this key if it exists.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="cache">The <see cref="IDistributedCache"/> instance this method extends.</param>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The value associated with this key if it exists.</returns>
    public static async Task<TValue?> GetAsync<TValue>(
        this IDistributedCache cache,
        string key,
        CancellationToken cancellationToken
    )
        where TValue : class
    {
        var itemBytes = await cache.GetAsync(key, cancellationToken);
        if (itemBytes is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<TValue>(itemBytes);
    }
}
