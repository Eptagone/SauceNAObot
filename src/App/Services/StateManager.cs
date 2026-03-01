using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SauceNAO.Core.Services;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.App.Services;

sealed class StateManager(IDistributedCache cache, IServiceProvider serviceProvider) : IStateManager
{
    public async Task DispatchAsync<TState>(
        Message message,
        TState state,
        CancellationToken cancellationToken
    )
        where TState : class
    {
        await this.UpdateAsync(message, state, cancellationToken);
        var cleanMessage = new Message
        {
            MessageId = message.MessageId,
            Chat = message.Chat,
            Date = message.Date,
            From = message.From,
        };
        var handlers = serviceProvider.GetServices<IUserStateHandler>();
        foreach (var handler in handlers)
        {
            if (await handler.TryContinueAsync(cleanMessage, cancellationToken))
            {
                return;
            }
        }

        throw new NotImplementedException();
    }

    public async Task<TState?> GetAsync<TState>(
        Message message,
        CancellationToken cancellationToken
    )
        where TState : class
    {
        var cacheKey = CreateStateKey<TState>(message);
        var stateBytes = await cache.GetAsync(cacheKey, cancellationToken);
        if (stateBytes is null)
        {
            return default;
        }

        using var stream = new MemoryStream(stateBytes);
        var state = await JsonSerializer.DeserializeAsync<TState>(
            stream,
            cancellationToken: cancellationToken
        );
        return state;
    }

    public Task RemoveAsync<TState>(Message message, CancellationToken cancellationToken)
        where TState : class
    {
        var cacheKey = CreateStateKey<TState>(message);
        return cache.RemoveAsync(cacheKey, cancellationToken);
    }

    public async Task UpdateAsync<TState>(
        Message message,
        TState state,
        CancellationToken cancellationToken
    )
        where TState : class
    {
        var cacheKey = CreateStateKey<TState>(message);
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, state, cancellationToken: cancellationToken);
        await cache.SetAsync(
            cacheKey,
            stream.ToArray(),
            new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) },
            cancellationToken
        );
    }

    private static string CreateStateKey<TState>(Message message)
        where TState : class
    {
        var name = typeof(TState).Name;
        return $"state/{name}/{message.Chat.Id}";
    }
}
