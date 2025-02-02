// Copyright (c) 2025 Quetzal Rivera.
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
/// Represents a command that shows the languages used by the users.
/// </summary>
[TelegramBotCommand(
    "langs",
    "Shows the bot usage statistics.",
    ["languages", "lenguajes", "idiomas"]
)]
[BotCommandVisibility(BotCommandVisibility.Hidden)]
class LanguagesCommand(
    ITelegramBotClient client,
    IDistributedCache cache,
    IUserRepository userRepository
) : BotCommandBase
{
    /// <inheritdoc />
    protected override async Task InvokeAsync(Message message, CancellationToken cancellationToken)
    {
        var statsMessage =
            await cache.GetOrCreateAsync(
                "snao:languages-msg",
                async entry =>
                {
                    // Send a message indicating that the bot is processing the command.
                    await client.SendChatActionAsync(
                        message.Chat.Id,
                        ChatActions.Typing,
                        cancellationToken: cancellationToken
                    );
                    entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                    return this.GetLanguageCodesMsg();
                },
                cancellationToken
            ) ?? this.GetLanguageCodesMsg();

        await client.SendMessageAsync(
            message.Chat.Id,
            statsMessage,
            parseMode: FormatStyles.HTML,
            cancellationToken: cancellationToken
        );
    }

    // Generates the message with the statistics.
    private string GetLanguageCodesMsg()
    {
        var languageCodesText = userRepository
            .GetLanguageCodes()
            .OrderByDescending(lc => lc.Value)
            .Select(lc => $"<b>{lc.Key}:</b> [{lc.Value}]");

        return this.Context.Localizer["Languages", string.Join("\n", languageCodesText)];
    }
}
