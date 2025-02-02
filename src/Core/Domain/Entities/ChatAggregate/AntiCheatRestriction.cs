// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain.Entities.ChatAggregate;

/// <summary>
/// Represents a restriction to prevent cheating by blocking reverse image searches for specific game bots in a group.
/// </summary>
/// <remarks>
/// Initializes a new instance of AntiCheats with the specified user and userId.
/// </remarks>
/// <param name="restrictedBotId">Unique identifier for the bot that users are restricted from reverse searching images of.</param>
public class AntiCheatRestriction(long restrictedBotId) : EntityBase
{
    /// <summary>
    /// Unique identifier for the bot that users are restricted from reverse searching images of.
    /// </summary>
    public long RestrictedBotId { get; set; } = restrictedBotId;

    /// <summary>
    /// The group that the restriction is applied to.
    /// </summary>
    public virtual TelegramChat Group { get; set; } = default!;
}
