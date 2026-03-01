// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Core.Exceptions;

/// <summary>
/// Defines a method to handle message exceptions
/// </summary>
public interface IMessageExceptionHandler
{
    /// <summary>
    /// Tries to handle the given message exception
    /// </summary>
    /// <param name="exception">The message exception</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<bool> TryHandleAsync(MessageException exception, CancellationToken cancellationToken);
}
