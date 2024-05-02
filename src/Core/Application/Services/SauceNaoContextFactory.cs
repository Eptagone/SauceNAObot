// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Application.Models;
using SauceNAO.Domain.Entities.ChatAggregate;
using SauceNAO.Domain.Entities.UserAggregate;
using SauceNAO.Domain.Repositories;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Application.Services;

/// <summary>
/// Represents the factory that creates instances of <see cref="ISauceNaoContext"/>.
/// </summary>
class SauceNaoContextFactory(IUserRepository userRepository, IChatRepository chatRepository)
    : ISauceNaoContextFactory
{
    private readonly IUserRepository userRepository = userRepository;
    private readonly IChatRepository groupRepository = chatRepository;

    /// <inheritdoc/>
    public ISauceNaoContext Create(Update update)
    {
        TelegramUser? userEntity = null;
        TelegramChat? groupEntity = null;

        var user =
            update.Message?.From
            ?? update.CallbackQuery?.From
            ?? update.EditedMessage?.From
            ?? update.InlineQuery?.From;

        // If the user is not null, retrieve or update the user information in the database.
        if (user is not null)
        {
            userEntity = this.userRepository.GetByUserId(user.Id);
            // Update the user information if it has changed.
            if (userEntity is null)
            {
                userEntity = new TelegramUser()
                {
                    UserId = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    LanguageCode = user.LanguageCode,
                };
                this.userRepository.Insert(userEntity);
            }
            else
            {
                userEntity.Username = user.Username;
                userEntity.FirstName = user.FirstName;
                userEntity.LastName = user.LastName;
                userEntity.LanguageCode ??= user.LanguageCode;
                this.userRepository.Update(userEntity);
            }
        }

        var chat =
            update.Message?.Chat
            ?? update.CallbackQuery?.Message?.Chat
            ?? update.EditedMessage?.Chat;
        // If the chat is not null and it is a group or supergroup, retrieve or update the group information in the database.
        if (chat?.Type == ChatTypes.Group || chat?.Type == ChatTypes.Supergroup)
        {
            groupEntity = this.groupRepository.GetByChatId(
                update.Message?.MigrateFromChatId ?? chat.Id
            );
            if (groupEntity is null)
            {
                groupEntity = new TelegramChat(chat.Id, chat.Title!) { Username = chat.Username, };
                this.groupRepository.Insert(groupEntity);
            }
            else
            {
                groupEntity.ChatId = update.Message?.MigrateToChatId ?? chat.Id;
                groupEntity.Title = chat.Title!;
                groupEntity.Username = chat.Username;
                this.groupRepository.Update(groupEntity);
            }
        }

        var languageCode = userEntity?.AlwaysUseOwnLanguage is true
            ? userEntity.LanguageCode ?? groupEntity?.LanguageCode
            : groupEntity?.LanguageCode ?? userEntity?.LanguageCode;

        // Set the language of the context.
        var localizer = new BotMessagesLocalizer(languageCode);

        // Create a new context with the initializer action.
        return new SauceNaoContext(userEntity, groupEntity, localizer);
    }
}
