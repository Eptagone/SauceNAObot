// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Domain.Entities.ChatAggregate;
using SauceNAO.Domain.Repositories;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Commands;

[TelegramBotCommand(
    "anticheats",
    "Manage the anti-cheats feature.",
    ["antitrampas", "notrampas", "cheatcontrol"]
)]
[BotCommandVisibility(BotCommandVisibility.GroupAdmin)]
class AnticheatsCommand(ITelegramBotClient client, IChatRepository chatRepository) : BotCommandBase
{
    private string HelpMsg => this.Context.Localizer["AnticheatsHelp"];
    private string BotAddedMsg => this.Context.Localizer["AnticheatsAdded"];
    private string BotRemovedMsg => this.Context.Localizer["AnticheatsRemoved"];
    private string BotAlreadyAddedMsg => this.Context.Localizer["AnticheatsAlreadyAdded"];
    private string BotAlreadyRemovedMsg => this.Context.Localizer["AnticheatsAlreadyRemoved"];
    private string TargetUserIsNotABotMsg => this.Context.Localizer["AntiCheatsNotABot"];
    private string NotAllowedMsg => this.Context.Localizer["NotAllowed"];

    /// <inheritdoc />
    protected override async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken = default
    )
    {
        if (this.Context.Group != null)
        {
            var admins = await client.GetChatAdministratorsAsync(
                this.Context.Group.Id,
                cancellationToken
            );

            // If the user is not an admin, send an error message.
            if (!admins.Any(a => a.User.Id == message.From!.Id))
            {
                await client.SendMessageAsync(
                    message.Chat.Id,
                    this.NotAllowedMsg,
                    replyParameters: new ReplyParameters
                    {
                        MessageId = message.MessageId,
                        AllowSendingWithoutReply = true
                    },
                    cancellationToken: cancellationToken
                );
            }

            // Send a message indicating that the bot is processing the command.
            await client.SendChatActionAsync(
                message.Chat.Id,
                ChatActions.Typing,
                cancellationToken: cancellationToken
            );

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
                        await client.SendMessageAsync(
                            message.Chat.Id,
                            this.TargetUserIsNotABotMsg,
                            replyParameters: new ReplyParameters
                            {
                                MessageId = message.MessageId,
                                AllowSendingWithoutReply = true
                            },
                            cancellationToken: cancellationToken
                        );
                        break;
                    }

                    // If the bot is already added, send an error message.
                    if (
                        this.Context.Group.Restrictions.Any(r =>
                            r.RestrictedBotId == message.ReplyToMessage.From.Id
                        )
                    )
                    {
                        await client.SendMessageAsync(
                            message.Chat.Id,
                            this.BotAlreadyAddedMsg,
                            replyParameters: new ReplyParameters
                            {
                                MessageId = message.MessageId,
                                AllowSendingWithoutReply = true
                            },
                            cancellationToken: cancellationToken
                        );
                        break;
                    }

                    // Add the bot to the group.
                    this.Context.Group.Restrictions.Add(
                        new AntiCheatRestriction(message.ReplyToMessage.From.Id)
                    );
                    await chatRepository.UpdateAsync(this.Context.Group, cancellationToken);

                    await client.SendMessageAsync(
                        message.Chat.Id,
                        this.BotAddedMsg,
                        replyParameters: new ReplyParameters
                        {
                            MessageId = message.MessageId,
                            AllowSendingWithoutReply = true
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
                        await client.SendMessageAsync(
                            message.Chat.Id,
                            this.TargetUserIsNotABotMsg,
                            replyParameters: new ReplyParameters
                            {
                                MessageId = message.MessageId,
                                AllowSendingWithoutReply = true
                            },
                            cancellationToken: cancellationToken
                        );
                        break;
                    }

                    // Find the bot in the group.
                    var restriction = this.Context.Group.Restrictions.FirstOrDefault(r =>
                        r.RestrictedBotId == message.ReplyToMessage.From.Id
                    );

                    // If the bot is not found, send an error message.
                    if (restriction is null)
                    {
                        await client.SendMessageAsync(
                            message.Chat.Id,
                            this.BotAlreadyRemovedMsg,
                            replyParameters: new ReplyParameters
                            {
                                MessageId = message.MessageId,
                                AllowSendingWithoutReply = true
                            },
                            cancellationToken: cancellationToken
                        );
                        break;
                    }

                    // Remove the bot from the group.
                    this.Context.Group.Restrictions.Remove(restriction);
                    await chatRepository.UpdateAsync(this.Context.Group, cancellationToken);

                    await client.SendMessageAsync(
                        message.Chat.Id,
                        this.BotRemovedMsg,
                        replyParameters: new ReplyParameters
                        {
                            MessageId = message.MessageId,
                            AllowSendingWithoutReply = true
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
                            AllowSendingWithoutReply = true
                        },
                        cancellationToken: cancellationToken
                    );
                    break;
            }
        }
    }
}
