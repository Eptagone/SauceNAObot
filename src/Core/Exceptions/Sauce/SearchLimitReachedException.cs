namespace SauceNAO.Core.Exceptions.Sauce;

/// <summary>
/// Thrown when the search limit has been reached for the user.
/// </summary>
/// <param name="innerException">The inner exception</param>
public sealed class SearchLimitReachedException(Exception innerException)
    : SauceNAOException(innerException: innerException) { }
