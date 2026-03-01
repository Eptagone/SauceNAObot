namespace SauceNAO.Core.Exceptions.Sauce;

/// <summary>
/// Represents an exception thrown while processing a request to the SauceNAO API
/// </summary>
/// <param name="message">The error message</param>
/// <param name="innerException">The inner exception</param>
public class SauceNAOException(string? message = null, Exception? innerException = null)
    : Exception(message, innerException) { }
