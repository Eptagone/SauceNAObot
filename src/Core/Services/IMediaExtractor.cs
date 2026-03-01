using SauceNAO.Core.Entities;
using SauceNAO.Core.Models;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Provides a method to extract media from a Telegram message.
/// </summary>
public interface IMediaExtractor
{
    /// <summary>
    /// Extracts media from a Telegram message.
    /// </summary>
    /// <param name="message">The message to extract media from.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<MediaTarget?> ExtractAsync(Message message, CancellationToken cancellationToken);

    /// <summary>
    /// Extracts media from the given message, starting from the message's reply or fallback to the message itself.
    /// </summary>
    /// <param name="message">The message to extract media from.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<MediaTarget?> DeepExtractAsync(Message message, CancellationToken cancellationToken);
}
