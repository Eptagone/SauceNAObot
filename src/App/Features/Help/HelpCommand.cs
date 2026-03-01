using SauceNAO.App.Resources;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.App.Features.Help;

[BotCommand("help", "How to use", ["ayuda"]), LocalizedBotCommand("es", "Como usar")]
sealed class HelpCommand(
    IContextProvider contextProvider,
    ITelegramBotClient client,
    IBetterStringLocalizer<HelpCommand> localizer
) : ICommandHandler
{
    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        await contextProvider.LoadAsync(message, cancellationToken);
        await client.SendMessageAsync(
            message.Chat.Id,
            localizer["Help"],
            parseMode: FormatStyles.HTML,
            replyParameters: new()
            {
                AllowSendingWithoutReply = true,
                MessageId = message.MessageId,
            },
            cancellationToken: cancellationToken
        );
    }
}
