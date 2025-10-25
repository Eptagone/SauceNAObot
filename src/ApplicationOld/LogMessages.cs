// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Logging;

namespace SauceNAO.Application;

internal static partial class LogMessages
{
    #region Setup and initialization
    [LoggerMessage(
        EventId = 10,
        Level = LogLevel.Information,
        Message = "Setting up the bot commands"
    )]
    internal static partial void LogSettingUpBotCommands(this ILogger logger);

    [LoggerMessage(
        EventId = 11,
        Level = LogLevel.Information,
        Message = "Commands registered successfully"
    )]
    internal static partial void LogCommandsRegistered(this ILogger logger);

    [LoggerMessage(EventId = 12, Level = LogLevel.Information, Message = "Setting up the webhook")]
    internal static partial void LogWebhookSetUp(this ILogger logger);

    [LoggerMessage(
        EventId = 13,
        Level = LogLevel.Information,
        Message = "Webhook set up successfully at {Url}"
    )]
    internal static partial void LogWebhookSetUpSuccessful(this ILogger logger, string url);

    [LoggerMessage(
        EventId = 14,
        Level = LogLevel.Information,
        Message = "Setup completed successfully"
    )]
    internal static partial void LogSetupCompleted(this ILogger logger);

    #endregion


    #region User states
    [LoggerMessage(
        EventId = 30,
        Level = LogLevel.Critical,
        Message = "There are not any handlers for the user state with scope {Scope}"
    )]
    internal static partial void LogFailedToHandleUserState(this ILogger logger, string scope);
    #endregion
}
