// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Logging;

namespace SauceNAO.Infrastructure;

internal static partial class LogMessages
{
    [LoggerMessage(
        Message = "An error occurred while calling the SauceNAO API. Message: {message}",
        Level = LogLevel.Error,
        SkipEnabledCheck = true
    )]
    internal static partial void LogSnaoError(this ILogger logger, string message);

    [LoggerMessage(
        Message = "An error occurred while retrieving the SauceNAO API response. No response was returned or the response could not be deserialized.",
        Level = LogLevel.Error,
        SkipEnabledCheck = true
    )]
    internal static partial void LogSnaoEmptyResponse(this ILogger logger);

    // This case would not be possible in the current implementation.
    [LoggerMessage(
        Message = "The response from the SauceNAO API was not recognized.",
        Level = LogLevel.Error,
        SkipEnabledCheck = true
    )]
    internal static partial void LogSnaoUnknownResponse(this ILogger logger);

    [LoggerMessage(
        Message = "An error occurred while calling the SauceNAO API. The request failed. Message: {Message}",
        Level = LogLevel.Error,
        SkipEnabledCheck = true
    )]
    internal static partial void LogSnaoRequestFailed(
        this ILogger logger,
        Exception e,
        string message
    );

    [LoggerMessage(
        Message = "Could not create a proper URL to access telegram files from the local server. The application URL was not configured.",
        Level = LogLevel.Critical,
        SkipEnabledCheck = true
    )]
    internal static partial void LogTelegramFileUrlError(this ILogger logger);
}
