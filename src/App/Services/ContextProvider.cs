// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.App.Resources;
using SauceNAO.Core.Data;
using SauceNAO.Core.Entities;
using SauceNAO.Core.Entities.UserAggregate;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.App.Services;

sealed class ContextProvider(
    IUserRepository users,
    IChatRepostory groups,
    IBetterStringLocalizerFactory localizerFactory
) : IContextProvider
{
    public async Task<UserEntity> LoadAsync(Message message, CancellationToken cancellationToken)
    {
        var (user, _) = await this.LoadAllAsync(message, cancellationToken);
        return user;
    }

    public async Task<UserEntity> LoadAsync(User from, CancellationToken cancellationToken)
    {
        var user = await users.UpsertAsync(from, false, cancellationToken);
        if (user.LanguageCode is not null)
        {
            localizerFactory.ChangeCulture(user.LanguageCode);
        }
        return user;
    }

    public async Task<(UserEntity user, ChatEntity? group)> LoadAllAsync(
        Message message,
        CancellationToken cancellationToken
    )
    {
        var isPrivateChat = message.Chat.Type == ChatTypes.Private;
        var user = await users.UpsertAsync(message.From!, isPrivateChat, cancellationToken);
        var group = isPrivateChat
            ? null
            : await groups.UpsertFromMessageAsync(message, cancellationToken);
        if (user.LanguageCode is not null)
        {
            localizerFactory.ChangeCulture(user.LanguageCode);
        }
        else if (group?.LanguageCode is not null)
        {
            localizerFactory.ChangeCulture(group.LanguageCode);
        }
        return (user, group);
    }
}
