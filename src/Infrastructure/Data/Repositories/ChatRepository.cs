// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.Domain.Entities.ChatAggregate;
using SauceNAO.Domain.Repositories;
using SauceNAO.Domain.Specifications;

namespace SauceNAO.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a repository for the chat entity.
/// </summary>
/// <param name="context">The database context.</param>
class ChatRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationDbContext, TelegramChat>(context),
        IChatRepository
{
    /// <inheritdoc/>
    public TelegramChat? GetByChatId(long chatId)
    {
        var spec = new ChatSpecification(chatId);
        return spec.Evaluate(this.Context.Chats).SingleOrDefault();
    }
}
