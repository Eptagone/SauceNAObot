using SauceNAO.App.Resources;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.App.Features.Help;

[BotCommand("start", "Start the bot"), BotCommandVisibility(BotCommandVisibility.Hidden)]
sealed class StartCommand(
    IContextProvider contextProvider,
    ITelegramBotClient client,
    IBetterStringLocalizer<StartCommand> localizer
) : ICommandHandler
{
    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        if (message.Chat.Type != ChatTypes.Private)
        {
            return;
        }

        await contextProvider.LoadAsync(message, cancellationToken);
        await client.SendMessageAsync(
            message.Chat.Id,
            localizer["About"],
            parseMode: FormatStyles.HTML,
            linkPreviewOptions: new() { IsDisabled = true },
            replyParameters: new()
            {
                AllowSendingWithoutReply = true,
                MessageId = message.MessageId,
            },
            cancellationToken: cancellationToken
        );
    }
}
