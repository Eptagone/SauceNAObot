using Microsoft.Extensions.Options;
using SauceNAO.Application.Resources;
using SauceNAO.Core;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.Application.Features.General;

[BotCommand("help", "How to use the bot.", ["ayuda"])]
[LocalizedBotCommand("es", "ayuda", "Como usar el bot.")]
class HelpCommand(
    IOptions<GeneralOptions> options,
    ITelegramBotClient client,
    IBotHelper helper,
    IBetterStringLocalizer<HelpCommand> localizer
) : IBotCommandHandler
{
    private readonly string? SupportChatLink = options.Value.SupportChatInvitationLink;
    private string SupportChatText => localizer["SupportChat"];
    private string HelpMessage => localizer["About"];

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

        var keyboard = string.IsNullOrEmpty(this.SupportChatLink)
            ? null
            : new InlineKeyboardMarkup(
                new InlineKeyboardBuilder().AppendUrl(this.SupportChatText, this.SupportChatLink)
            );

        await client.SendMessageAsync(
            message.Chat.Id,
            this.HelpMessage,
            parseMode: FormatStyles.HTML,
            replyParameters: new ReplyParameters
            {
                MessageId = message.MessageId,
                AllowSendingWithoutReply = true,
            },
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }
}
