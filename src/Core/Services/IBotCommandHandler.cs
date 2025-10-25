using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Represents a command
/// </summary>
public interface IBotCommandHandler
{
    /// <summary>
    /// Invokes the command
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task InvokeAsync(Message message, string[] args, CancellationToken cancellationToken);
}
