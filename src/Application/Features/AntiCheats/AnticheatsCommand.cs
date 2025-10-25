// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Application.Resources;
using SauceNAO.Core.Entities.ChatAggregate;
using SauceNAO.Core.Repositories;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.Application.Features.AntiCheats;

[BotCommand("anticheats", "Manage the anti-cheats feature.", ["notrampas", "cheatcontrol"])]
[LocalizedBotCommand("es", "antitrampas", "Administrar la funcionalidad de anti-trampas.")]
[BotCommandVisibility(BotCommandVisibility.Administrators)]
class AnticheatsCommand(
    ITelegramBotClient client,
    IBotHelper helper,
    IChatRepostory groupManager,
    IBetterStringLocalizer<AnticheatsCommand> localizer
) : IBotCommandHandler
{
    private string HelpMsg => localizer["AnticheatsHelp"];
    private string BotAddedMsg => localizer["AnticheatsAdded"];
    private string BotRemovedMsg => localizer["AnticheatsRemoved"];

    /// <inheritdoc />
    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken = default
    )
    {
        var (user, group, languageCode) = await helper.UpsertDataFromMessageAsync(
            message,
            cancellationToken
        );

        if (group is null)
        {
            return;
        }

        if (!await helper.IsAdminAsync(message.From!.Id, message.Chat.Id, cancellationToken))
        {
            return;
        }

        // Send a message indicating that the bot is processing the command.
        await client.SendChatActionAsync(
            message.Chat.Id,
            ChatActions.Typing,
            cancellationToken: cancellationToken
        );

        if (languageCode is not null)
        {
            localizer.ChangeCulture(languageCode);
        }

        var operation = args.ElementAtOrDefault(0);
        switch (operation)
        {
            case "add":
            case "insert":
            case "new":
                if (message.ReplyToMessage?.From is null)
                {
                    goto default;
                }
                else if (!message.ReplyToMessage.From.IsBot == false)
                {
                    throw new NotABotException(message);
                }

                // If the bot is already added, send an error message.
                if (
                    group.Restrictions.Any(r => r.RestrictedBotId == message.ReplyToMessage.From.Id)
                )
                {
                    throw new AlreadyAddedAntiCheatsException(message, message.ReplyToMessage.From);
                }

                // Add the bot to the group.
                group.Restrictions.Add(new AntiCheatRestriction(message.ReplyToMessage.From.Id));
                await groupManager.UpdateAsync(group, cancellationToken);

                await client.SendMessageAsync(
                    message.Chat.Id,
                    this.BotAddedMsg,
                    replyParameters: new ReplyParameters
                    {
                        MessageId = message.MessageId,
                        AllowSendingWithoutReply = true,
                    },
                    cancellationToken: cancellationToken
                );

                break;

            case "del":
            case "delete":
            case "rem":
            case "remove":
                if (message.ReplyToMessage?.From is null)
                {
                    goto default;
                }
                else if (!message.ReplyToMessage.From.IsBot == false)
                {
                    throw new NotABotException(message);
                }

                // Find the bot in the group.
                var restriction =
                    group.Restrictions.FirstOrDefault(r =>
                        r.RestrictedBotId == message.ReplyToMessage.From.Id
                    )
                    ?? throw new AlreadyRemovedAntiCheatsException(
                        message,
                        message.ReplyToMessage.From
                    );

                // Remove the bot from the group.
                group.Restrictions.Remove(restriction);
                await groupManager.UpdateAsync(group, cancellationToken);

                await client.SendMessageAsync(
                    message.Chat.Id,
                    this.BotRemovedMsg,
                    replyParameters: new ReplyParameters
                    {
                        MessageId = message.MessageId,
                        AllowSendingWithoutReply = true,
                    },
                    cancellationToken: cancellationToken
                );
                break;

            default:
                await client.SendMessageAsync(
                    message.Chat.Id,
                    this.HelpMsg,
                    parseMode: FormatStyles.HTML,
                    replyParameters: new ReplyParameters
                    {
                        MessageId = message.MessageId,
                        AllowSendingWithoutReply = true,
                    },
                    cancellationToken: cancellationToken
                );
                break;
        }
    }
}
