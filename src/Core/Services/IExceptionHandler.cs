namespace SauceNAO.Core.Services;

/// <summary>
/// Defines a method to handle exceptions during the processing of an update.
/// </summary>
public interface IExceptionHandler
{
    /// <summary>
    /// Try to handle an exception during the processing of an update.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>True if the exception was handled, false otherwise.</returns>
    public Task<bool> TryHandleAsync(Exception exception, CancellationToken cancellationToken);
}
