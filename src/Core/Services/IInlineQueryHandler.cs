using Telegram.BotAPI.InlineMode;

namespace SauceNAO.Core.Services;

/// <summary>
/// Defines a handler for inline queries.
/// </summary>
public interface IInlineQueryHandler
{
    /// <summary>
    /// Try to handle the inline query.
    /// </summary>
    /// <param name="inlineQuery">The inline query.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>True if the inline query was handled, false otherwise.</returns>
    Task<bool> HandleAsync(InlineQuery inlineQuery, CancellationToken cancellationToken = default);
}
