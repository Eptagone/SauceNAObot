using Telegram.BotAPI.InlineMode;

namespace SauceNAO.Core.Services;

/// <summary>
/// Handles inline queries
/// </summary>
public interface IInlineQueryHandler
{
    /// <summary>
    /// Handles an incoming inline query
    /// </summary>
    /// <param name="inlineQuery">The inline query to handle</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns><see langword="true"/> if the message was handled</returns>
    Task<bool> TryHandleAsync(
        InlineQuery inlineQuery,
        CancellationToken cancellationToken = default
    );
}
