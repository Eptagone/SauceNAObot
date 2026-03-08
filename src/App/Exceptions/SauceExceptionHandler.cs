// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Options;
using SauceNAO.App.Resources;
using SauceNAO.Core;
using SauceNAO.Core.Configuration;
using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Exceptions.Sauce;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.UpdatingMessages;

namespace SauceNAO.App.Exceptions;

class SauceExceptionHandler(
    IContextProvider contextProvider,
    ITelegramBotClient client,
    IOptions<AppConfiguration> options,
    IBetterStringLocalizer<SauceExceptionHandler> localizer
) : IMessageExceptionHandler
{
    public async Task<bool> TryHandleAsync(
        MessageException exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is SauceException se)
        {
            await this.HandleAsync(se, cancellationToken);
            return true;
        }

        return false;
    }

    private async Task HandleAsync(SauceException exception, CancellationToken cancellationToken)
    {
        var message = exception.ReceivedMessage;
        await contextProvider.LoadAsync(message, cancellationToken);

        var errorKey = exception.GetType().Name switch
        {
            nameof(NoApiKeysException) => "NoApiKeys",
            nameof(SauceNotFoundException) => "NotFound",
            _ => exception.InnerException?.GetType().Name switch
            {
                nameof(SearchLimitReachedException) => "Busy",
                nameof(InvalidApiKeyException) => "Busy",
                _ => null,
            },
        };

        InlineKeyboardMarkup? replyMarkup = null;
        if (
            exception is NoApiKeysException
            && !string.IsNullOrEmpty(options.Value.SupportChatInvitationLink)
        )
        {
            var builder = new InlineKeyboardBuilder().AppendUrl(
                localizer["SupportChat"],
                options.Value.SupportChatInvitationLink
            );
            replyMarkup = new InlineKeyboardMarkup(builder);
        }
        errorKey ??= "UnknownError";
        string text =
            errorKey == "Busy"
                ? localizer[errorKey, options.Value.SupportChatInvitationLink ?? string.Empty]
                : localizer[errorKey];
        if (exception is SauceNotFoundException snf)
        {
            text = string.Format(
                text,
                string.Format(ImageSearchLinks.GoogleImageSearch, snf.MediaUrl),
                string.Format(ImageSearchLinks.YandexImageSearch, snf.MediaUrl),
                string.Format(ImageSearchLinks.SauceNAOsearch, snf.MediaUrl)
            );
        }

        if (exception.SentMessage is null)
        {
            await client.SendMessageAsync(
                message.Chat.Id,
                text,
                parseMode: FormatStyles.HTML,
                linkPreviewOptions: new() { IsDisabled = true },
                replyParameters: new()
                {
                    ChatId = message.Chat.Id,
                    MessageId = message.MessageId,
                    AllowSendingWithoutReply = true,
                },
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken
            );
        }
        else
        {
            await client.EditMessageTextAsync(
                exception.SentMessage.Chat.Id,
                exception.SentMessage.MessageId,
                text,
                parseMode: FormatStyles.HTML,
                linkPreviewOptions: new() { IsDisabled = true },
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken
            );
        }
    }
}
