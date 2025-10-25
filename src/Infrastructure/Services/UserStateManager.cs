// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SauceNAO.Core.Extensions;
using SauceNAO.Core.Models;
using SauceNAO.Core.Services;

namespace SauceNAO.Infrastructure.Services;

/// <summary>
/// Defines methods to manage user states.
/// </summary>
class UserStateManager(IDistributedCache cache) : IUserStateManager
{
    public Task CreateOrUpdateAsync(UserState state, CancellationToken cancellationToken)
    {
        var key = $"snao:user-state:{state.ChatId}:{state.UserId}";
        var options = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(30),
        };
        var serializedState = JsonSerializer.SerializeToUtf8Bytes(state);
        return cache.SetAsync(key, serializedState, options, cancellationToken);
    }

    public Task<UserState?> GetAsync(long chatId, long? userId, CancellationToken cancellationToken)
    {
        var key = $"snao:user-state:{chatId}:{userId}";
        return cache.GetAsync<UserState>(key, cancellationToken);
    }

    public Task RemoveAsync(UserState state, CancellationToken cancellationToken)
    {
        var key = $"snao:user-state:{state.ChatId}:{state.UserId}";
        return cache.RemoveAsync(key, cancellationToken);
    }
}
