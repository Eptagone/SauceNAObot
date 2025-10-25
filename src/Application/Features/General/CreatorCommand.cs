using SauceNAO.Application.Resources;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.Application.Features.General;

[BotCommand("creator", "Shows the bot creator.", "creador", "author")]
[BotCommandVisibility(BotCommandVisibility.Hidden)]
class CreatorCommand(
    ITelegramBotClient client,
    IBotHelper helper,
    IBetterStringLocalizer<CreatorCommand> localizer
) : IBotCommandHandler
{
    private string CreatorMessage => localizer["Creator"];

    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        var (_, _, languageCode) = await helper.UpsertDataFromMessageAsync(
            message,
            cancellationToken
        );
        if (!string.IsNullOrEmpty(languageCode))
        {
            localizer.ChangeCulture(languageCode);
        }

        await client.SendMessageAsync(
            message.Chat.Id,
            this.CreatorMessage,
            parseMode: FormatStyles.HTML,
            replyParameters: new ReplyParameters
            {
                MessageId = message.MessageId,
                AllowSendingWithoutReply = true,
            },
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
            cancellationToken: cancellationToken
        );
    }
}
