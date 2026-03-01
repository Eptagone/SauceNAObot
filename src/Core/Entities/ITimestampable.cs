namespace SauceNAO.Core.Entities;

/// <summary>
/// Defines properties for entities that have a creation and update date.
/// </summary>
public interface ITimestampable : IWithCreationDate
{
    /// <summary>
    /// Date and time of last update.
    /// </summary>
    DateTimeOffset UpdatedAt { get; set; }
}
