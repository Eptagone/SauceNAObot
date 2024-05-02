// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SauceNAO.Application.Models;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Services;

/// <summary>
/// Defines methods to manage user states.
/// </summary>
/// <param name="logger">The logger.</param>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="cache">The memory cache instance.</param>
class UserStateManager(
    ILogger<UserStateManager> logger,
    IServiceProvider serviceProvider,
    ITelegramBotClient client,
    IMemoryCache cache
) : IUserStateManager
{
    private readonly ILogger<UserStateManager> logger = logger;
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly IMemoryCache cache = cache;

    /// <inheritdoc />
    public Task? ContinueStateAsync(
        Message message,
        ISauceNaoContext context,
        CancellationToken cancellationToken
    )
    {
        var key = $"UserState:{message.Chat.Id}:{message.From?.Id}";
        // Try to get the state. If it doesn't exist, return null.
        if (!this.cache.TryGetValue(key, out UserState? state) || state is null)
        {
            return null;
        }

        // If the user wants to cancel the current state, return null.
        if (message.Text == "/cancel")
        {
            this.cache.Remove(key);
            var cancelMessage = context.Localizer["CancelMessage"];

            // Send the message.
            client.SendMessageAsync(
                message.Chat.Id,
                cancelMessage,
                replyParameters: new ReplyParameters
                {
                    MessageId = message.MessageId,
                    AllowSendingWithoutReply = true,
                },
                cancellationToken: cancellationToken
            );
            return null;
        }

        var service = this.serviceProvider.GetKeyedService<IUserStateHandler>(state.Scope);
        if (service is null)
        {
            this.logger.LogFailedToHandleUserState(state.Scope);
            return null;
        }

        service.Context = context;
        return service.ResolveStateAsync(state, message, cancellationToken);
    }

    /// <inheritdoc />
    public void CreateOrUpdateState(UserState state)
    {
        var key = $"UserState:{state.ChatId}:{state.UserId}";
        this.cache.Set(key, state, TimeSpan.FromMinutes(30));
    }

    /// <inheritdoc />
    public void RemoveState(UserState state)
    {
        var key = $"UserState:{state.ChatId}:{state.UserId}";
        this.cache.Remove(key);
    }
}
