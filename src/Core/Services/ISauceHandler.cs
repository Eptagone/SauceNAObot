using SauceNAO.Core.Models;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Handles the sauce search
/// </summary>
public interface ISauceHandler
{
    /// <summary>
    /// Perform the sauce search to the given message
    /// </summary>
    /// <param name="target">The target media</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns></returns>
    Task HandleAsync(MediaTarget target, CancellationToken cancellationToken = default);

    /// <summary>
    /// Perform the sauce search to the given message
    /// </summary>
    /// <param name="target">The target media</param>
    /// <param name="similarity">The similarity threshold</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns></returns>
    Task HandleAsync(
        MediaTarget target,
        float similarity,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Build the sauce info and keyboard for the given sauces
    /// </summary>
    /// <param name="sauces">List of sauces</param>
    /// <returns></returns>
    ValueTuple<string, InlineKeyboardMarkup> CookSauces(IEnumerable<Sauce> sauces);
}
