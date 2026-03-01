using SauceNAO.Core.Services;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.App.Services;

/// <summary>
/// Handles user state messages
/// </summary>
/// <param name="handlers">A list of the available handlers</param>
public sealed class UserStateMessageHandler(IEnumerable<IUserStateHandler> handlers)
    : IMessageHandler
{
    public async Task<bool> TryHandleAsync(Message message, CancellationToken cancellationToken)
    {
        foreach (var handler in handlers)
        {
            if (await handler.TryContinueAsync(message, cancellationToken))
            {
                return true;
            }
        }

        return false;
    }
}
