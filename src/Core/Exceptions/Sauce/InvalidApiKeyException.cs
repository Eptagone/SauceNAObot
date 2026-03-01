namespace SauceNAO.Core.Exceptions.Sauce;

/// <summary>
/// Thrown when the api key is invalid or expired
/// </summary>
/// <param name="innerException"></param>
public sealed class InvalidApiKeyException(Exception? innerException = null)
    : SauceNAOException(innerException: innerException) { }
