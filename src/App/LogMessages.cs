namespace SauceNAO.App;

internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Critical,
        Message = "Failed to add the update {UpdateId} to the updates pool"
    )]
    internal static partial void LogFailedToAddUpdateToPool(this ILogger logger, int updateId);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Failed to process the update {UpdateId}"
    )]
    internal static partial void LogFailedToProcessUpdate(
        this ILogger logger,
        int updateId,
        Exception e
    );

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Error,
        Message = "The bot took too long to process the update {UpdateId}, so it was cancelled"
    )]
    internal static partial void LogCancelledUpdate(this ILogger logger, int updateId);

    [LoggerMessage(
        EventId = 10,
        Level = LogLevel.Error,
        Message = "Command \"{CommandName}\" failed to execute"
    )]
    internal static partial void LogFailedToProcessCommand(
        this ILogger logger,
        string commandName,
        Exception? e
    );
}
