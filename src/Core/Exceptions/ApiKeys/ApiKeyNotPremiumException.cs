using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions.ApiKeys;

/// <summary>
/// Thrown when the user attempts to Add a non-premium api key
/// </summary>
/// <param name="receivedMessage">The message sent by the user that triggered the exception</param>
public sealed class ApiKeyNotPremiumException(Message receivedMessage)
    : ApiKeyException(receivedMessage) { }
