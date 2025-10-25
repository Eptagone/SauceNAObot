using SauceNAO.Core.Entities.ChatAggregate;
using SauceNAO.Core.Entities.UserAggregate;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Defines utility methods to handle messages and commands
/// </summary>
public interface IBotHelper
{
    /// <summary>
    /// Retrieves information about the bot
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns></returns>
    Task<User> GetMeAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Upserts data from a message and returns the user, group, and language code for further processing.
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns>The user, group, and language code</returns>
    Task<(TelegramUser user, TelegramChat? group, string? languageCode)> UpsertDataFromMessageAsync(
        Message message,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Checks if a user is an admin in the specified chat
    /// </summary>
    /// <param name="userId">The user id</param>
    /// <param name="chatId">The chat id</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns>True if the user is an admin, false otherwise</returns>
    Task<bool> IsAdminAsync(long userId, long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if the message contains a sauce mention
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns></returns>
    Task<bool> IsSauceMentioned(Message message, CancellationToken cancellationToken);
}
