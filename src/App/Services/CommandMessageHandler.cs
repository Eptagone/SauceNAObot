using System.Text.RegularExpressions;
using Castle.Core.Internal;
using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.App.Services;

/// <summary>
/// Handles bot commands in messages
/// </summary>
[Priority(1)]
partial class CommandMessageHandler(
    ITelegramBotClient client,
    IEnumerable<ICommandHandler> commands
) : IMessageHandler
{
    private static string? BotUsername;

    [GeneratedRegex(@"""(?<arg>[^""]+)""|(?<arg>\S+)", RegexOptions.Compiled)]
    private static partial Regex GetArgPattern();

    public async Task<bool> TryHandleAsync(Message message, CancellationToken cancellationToken)
    {
        if (BotCommandParser.TryParse(message, out var result))
        {
            var (commandName, commandArgs, username) = result;

            // Get the bot username if not already set
            if (string.IsNullOrEmpty(BotUsername))
            {
                var me = await client.GetMeAsync(cancellationToken);
                BotUsername = me.Username;
            }

            // Only handle commands for the current bot
            if (string.IsNullOrEmpty(username) || username == BotUsername)
            {
                var command = commands.FirstOrDefault(c =>
                {
                    var type = c.GetType();
                    var aliases = type.GetAttributes<BotCommandAttribute>()
                        .SelectMany(a => a.Aliases.Concat([a.Command]))
                        .Concat(
                            type.GetAttributes<LocalizedBotCommandAttribute>()
                                .Select(a => a.Command)
                        )
                        .Where(a => !string.IsNullOrEmpty(a))
                        .Distinct();
                    return aliases.Any(a => a == commandName);
                });
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
                    catch (DownloadFailedException exp)
                    {
                        throw new CommandException(commandName, message, exp)
                        {
                            SentMessage = exp.SentMessage,
                        };
                    }
                    catch (UnknownMessageException exp)
                    {
                        throw new CommandException(commandName, message, exp.InnerException)
                        {
                            SentMessage = exp.SentMessage,
                        };
                    }
                    catch (MessageException)
                    {
                        throw;
                    }
                    catch (Exception exp)
                    {
                        throw new CommandException(commandName, message, exp);
                    }
                }
            }

            return true;
        }

        return false;
    }
}
