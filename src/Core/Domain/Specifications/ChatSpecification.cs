// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Linq.Expressions;
using SauceNAO.Domain.Entities.ChatAggregate;

namespace SauceNAO.Domain.Specifications;

/// <summary>
/// Represents a specification to filter chats by their unique identifier.
/// </summary>
/// <param name="chatId">The chat identifier.</param>f
public class ChatSpecification(long chatId) : SpecificationBase<TelegramChat>
{
    /// <inheritdoc/>
    protected override Expression<Func<TelegramChat, bool>> Expression =>
        chat => chat.ChatId == chatId;
}
