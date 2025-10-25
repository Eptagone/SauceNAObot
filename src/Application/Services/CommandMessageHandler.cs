using System.Text.RegularExpressions;
using Castle.Core.Internal;
using SauceNAO.Core.Services;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.Application.Services;

partial class CommandMessageHandler(
    ILogger<CommandMessageHandler> logger,
    IBotHelper botHelper,
    IEnumerable<IBotCommandHandler> commands
) : IMessageHandler
{
    [GeneratedRegex(@"""(?<arg>[^""]+)""|(?<arg>\S+)", RegexOptions.Compiled)]
    private static partial Regex GetArgPattern();

    public async Task<bool> HandleAsync(Message message, CancellationToken cancellationToken)
    {
        if (BotCommandParser.TryParse(message, out var result))
        {
            if (!string.IsNullOrEmpty(result.username))
            {
                var me = await botHelper.GetMeAsync(cancellationToken);
                if (me.Username != result.username)
                {
                    return true;
                }
            }

            var (commandName, commandArgs, _) = result;
            var command = commands.FirstOrDefault(c =>
                c.GetType().GetAttributes<BotCommandAttribute>().Any(a => a.Command == commandName)
            );
            if (command is not null)
            {
                IEnumerable<string> args = string.IsNullOrEmpty(commandArgs)
                    ? []
                    : GetArgPattern()
                        .Matches(commandArgs)
                        .Select(match => match.Groups["arg"].Value);
                try
                {
                    await command.InvokeAsync(message, [.. args], cancellationToken);
                }
                catch (Exception exp)
                {
                    logger.LogFailedToProcessCommand(commandName, exp);
                }
            }
        }

        return false;
    }
}

internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 100,
        Message = "An unexpected error occurred while executing the command \"{command}\".",
        Level = LogLevel.Error,
        SkipEnabledCheck = true
    )]
    internal static partial void LogFailedToProcessCommand(
        this ILogger<CommandMessageHandler> logger,
        string command,
        Exception exception
    );
}
