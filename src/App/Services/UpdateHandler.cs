using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Services;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.App.Services;

sealed class UpdateHandler(
    IEnumerable<IMessageHandler> messageHandlers,
    IEnumerable<IInlineQueryHandler> inlineQueryHandlers,
    IEnumerable<IMessageExceptionHandler> messageExceptionHandlers
) : IUpdateHandler
{
    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message is not null)
            {
                foreach (var handler in messageHandlers)
                {
                    if (await handler.TryHandleAsync(update.Message, cancellationToken))
                    {
                        break;
                    }
                }
            }
            else if (update.InlineQuery is not null)
            {
                foreach (var handler in inlineQueryHandlers)
                {
                    if (await handler.TryHandleAsync(update.InlineQuery, cancellationToken))
                    {
                        break;
                    }
                }
            }
        }
        catch (MessageException e)
        {
            foreach (var handler in messageExceptionHandlers)
            {
                if (await handler.TryHandleAsync(e, cancellationToken))
                {
                    return;
                }
            }

            // This should never happen
            throw;
        }
        catch (Exception)
        {
            // Handle other exceptions
            throw;
        }
    }
}
