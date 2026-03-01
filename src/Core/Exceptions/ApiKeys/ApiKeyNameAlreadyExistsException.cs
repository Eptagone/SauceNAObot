using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Exceptions.ApiKeys;

/// <summary>
/// Thrown when the user attempts to create an api key with a name that already exists
/// </summary>
/// <param name="receivedMessage">The message sent by the user that triggered the exception</param>
public sealed class ApiKeyNameAlreadyExistsException(Message receivedMessage)
    : ApiKeyException(receivedMessage) { }
