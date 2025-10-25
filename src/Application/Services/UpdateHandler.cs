using SauceNAO.Core.Services;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.InlineMode;

namespace SauceNAO.Application.Services;

/// <summary>
/// Handles updates for the SauceNAO bot.
/// </summary>
partial class UpdateHandler(
    IEnumerable<IMessageHandler> messageHandlers,
    IEnumerable<IInlineQueryHandler> inlineHandlers,
    IEnumerable<IExceptionHandler> exceptionHandlers
) : SimpleUpdateHandlerBase, IAsyncUpdateHandler
{
    protected override async Task OnMessageAsync(
        Message message,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var handler in messageHandlers)
        {
            var okey = await handler.HandleAsync(message, cancellationToken);
            if (okey)
            {
                break;
            }
        }
    }

    protected override async Task OnInlineQueryAsync(
        InlineQuery inlineQuery,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var handler in inlineHandlers)
        {
            var okey = await handler.HandleAsync(inlineQuery, cancellationToken);
            if (okey)
            {
                break;
            }
        }
    }

    protected override async Task OnExceptionAsync(
        Exception exp,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var handler in exceptionHandlers)
        {
            var okey = await handler.TryHandleAsync(exp, cancellationToken);
            if (okey)
            {
                return;
            }
        }

        throw exp;
    }
}
