using SauceNAO.App.Resources;
using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.UpdatingMessages;

namespace SauceNAO.App.Exceptions;

sealed class CommandExceptionHandler(
    IContextProvider contextProvider,
    ILogger<CommandExceptionHandler> logger,
    IBetterStringLocalizer<CommandExceptionHandler> localizer,
    ITelegramBotClient client
) : IMessageExceptionHandler
{
    public async Task<bool> TryHandleAsync(
        MessageException exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is CommandException ce)
        {
            await this.HandleAsync(ce, cancellationToken);
            return true;
        }

        return false;
    }

    private async Task HandleAsync(CommandException exception, CancellationToken cancellationToken)
    {
        logger.LogFailedToProcessCommand(exception.CommandName, exception.InnerException);
        await contextProvider.LoadAsync(exception.ReceivedMessage, cancellationToken);

        if (exception.SentMessage is null)
        {
            await client.SendMessageAsync(
                exception.ReceivedMessage.Chat.Id,
                localizer["Message"],
                cancellationToken: cancellationToken
            );
        }
        else
        {
            await client.EditMessageTextAsync(
                exception.ReceivedMessage.Chat.Id,
                exception.SentMessage.MessageId,
                localizer["Message"],
                cancellationToken: cancellationToken
            );
        }
    }
}
