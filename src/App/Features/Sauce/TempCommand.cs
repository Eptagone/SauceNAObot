using SauceNAO.App.Resources;
using SauceNAO.Core;
using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Exceptions.Media;
using SauceNAO.Core.Models;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.Extensions.Commands;
using Telegram.BotAPI.UpdatingMessages;

namespace SauceNAO.App.Features.Sauce;

[
    BotCommand("temp", "Create a temporary link to an image"),
    LocalizedBotCommand("es", "Crear un enlace temporal a una imagen")
]
sealed class TempCommand(
    IContextProvider contextProvider,
    ITelegramBotClient client,
    IMediaExtractor mediaExtractor,
    IMediaUrlGenerator mediaUrlGenerator,
    IBetterStringLocalizer<TempCommand> localizer
) : ICommandHandler
{
    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        await client.SendChatActionAsync(
            message.Chat.Id,
            ChatActions.Typing,
            cancellationToken: cancellationToken
        );

        await contextProvider.LoadAsync(message, cancellationToken);
        var target =
            await mediaExtractor.DeepExtractAsync(
                message.ReplyToMessage ?? message,
                cancellationToken
            )
            ?? throw new InvalidPhotoException(message)
            {
                MediaMessage = message.ReplyToMessage ?? message,
            };

        var sentMessage = await client.SendMessageAsync(
            message.Chat.Id,
            localizer["GeneratingTmpUrl"],
            parseMode: FormatStyles.HTML,
            replyParameters: new()
            {
                AllowSendingWithoutReply = true,
                MessageId = message.MessageId,
            },
            cancellationToken: cancellationToken
        );

        try
        {
            var imageUrl = await mediaUrlGenerator.GenerateSafeAsync(target, cancellationToken);
            var googleUrl = string.Format(ImageSearchLinks.GoogleImageSearch, imageUrl);
            var yandexUrl = string.Format(ImageSearchLinks.YandexImageSearch, imageUrl);
            var sauceNaoUrl = string.Format(ImageSearchLinks.SauceNAOsearch, imageUrl);
            var keyboard = new InlineKeyboardBuilder()
                .AppendUrl("Google", googleUrl)
                .AppendUrl("Yandex", yandexUrl)
                .AppendRow()
                .AppendUrl("SauceNAO", sauceNaoUrl);
            await client.EditMessageTextAsync(
                sentMessage.Chat.Id,
                sentMessage.MessageId,
                localizer["TemporalUrlDone", imageUrl],
                parseMode: FormatStyles.HTML,
                linkPreviewOptions: new() { IsDisabled = true },
                replyMarkup: new InlineKeyboardMarkup(keyboard),
                cancellationToken: cancellationToken
            );
        }
        catch (MessageException e)
        {
            e.SentMessage = sentMessage;
            throw;
        }
        catch (Exception e)
        {
            var ue = new UnknownMessageException(message, e) { SentMessage = sentMessage };
            throw ue;
        }
    }
}
