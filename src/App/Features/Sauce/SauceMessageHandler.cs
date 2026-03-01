// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Exceptions.Media;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.App.Features.Sauce;

[Priority(2)]
sealed class SauceMessageHandler(
    ITelegramBotClient client,
    IMediaExtractor mediaExtractor,
    ISauceHandler sauceHandler
) : IMessageHandler
{
    private static string? BotUsername;
    private static readonly string[] Keywords = ["sauce", "source", "salsa", "snao", "saucenao"];

    public async Task<bool> TryHandleAsync(Message message, CancellationToken cancellationToken)
    {
        // Get the bot username if not already set
        if (string.IsNullOrEmpty(BotUsername))
        {
            var me = await client.GetMeAsync(cancellationToken);
            BotUsername = me.Username;
        }

        if (message.Chat.Type == ChatTypes.Private || message.ForwardOrigin is null)
        {
            if (
                message
                    .Text?.Split(' ')
                    .Any(w => w == $"/{BotUsername}" || Keywords.Contains(w.ToLowerInvariant()))
                is true
            )
            {
                var target =
                    await mediaExtractor.DeepExtractAsync(message, cancellationToken)
                    ?? throw new InvalidPhotoException(message);
                await sauceHandler.HandleAsync(target, cancellationToken);
                return true;
            }
        }
        if (message.Chat.Type == ChatTypes.Private)
        {
            // Ignore messages sent by the bot
            if (message.ViaBot?.Username == BotUsername)
            {
                return false;
            }

            // In private chats, always handle Sauce requests
            var target =
                await mediaExtractor.ExtractAsync(message, cancellationToken)
                ?? throw new InvalidPhotoException(message);
            await sauceHandler.HandleAsync(target, cancellationToken);
            return true;
        }

        return false;
    }
}
