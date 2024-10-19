// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Caching.Distributed;
using SauceNAO.Domain.Extensions;
using SauceNAO.Domain.Repositories;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Commands;

/// <summary>
/// Represents a command that shows the bot usage statistics.
/// </summary>
[TelegramBotCommand("stats", "Shows the bot usage statistics.", ["statistics", "estadisticas"])]
[BotCommandVisibility(BotCommandVisibility.Default)]
class StatsCommand(
    ITelegramBotClient client,
    IDistributedCache cache,
    ISauceMediaRepository mediaRepository,
    IUserRepository userRepository,
    IChatRepository chatRepository
) : BotCommandBase
{
    /// <inheritdoc />
    protected override async Task InvokeAsync(Message message, CancellationToken cancellationToken)
    {
        var statsMessage =
            await cache.GetOrCreateAsync(
                "snao:stats-msg",
                async entry =>
                {
                    // Send a message indicating that the bot is processing the command.
                    await client.SendChatActionAsync(
                        message.Chat.Id,
                        ChatActions.Typing,
                        cancellationToken: cancellationToken
                    );
                    entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                    return this.GetStatsMessage();
                },
                cancellationToken
            ) ?? this.GetStatsMessage();

        await client.SendMessageAsync(
            message.Chat.Id,
            statsMessage,
            parseMode: FormatStyles.HTML,
            cancellationToken: cancellationToken
        );
    }

    // Generates the message with the statistics.
    private string GetStatsMessage()
    {
        var searchedMediasCount = mediaRepository.Count();
        var activeUserCount = userRepository.CountActiveUsers();
        var chatCount = chatRepository.Count();

        return this.Context.Localizer["Stats", searchedMediasCount, activeUserCount, chatCount];
    }
}
