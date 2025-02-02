using SauceNAO.Domain.Entities.UserAggregate;

namespace SauceNAO.Domain.Entities;

/// <summary>
/// Represents an advertisement.
/// </summary>
/// <param name="text">The message text of the advertisement.</param>
public class Advertisement(string text) : EntityBase
{
    /// <summary>
    /// The message text of the advertisement.
    /// </summary>
    public string Text { get; set; } = text;

    /// <summary>
    /// The number of times the advertisement has been sent.
    /// </summary>
    public int SendCount { get; set; }

    /// <summary>
    /// Optional. The maximum number of times the advertisement can be sent.
    /// </summary>
    public int? MaxSendCount { get; set; }

    /// <summary>
    /// Optional. The date and time when the advertisement was published.
    /// </summary>
    public DateTimeOffset? PublishedAt { get; set; }

    /// <summary>
    /// The person who created the advertisement.
    /// </summary>
    public virtual TelegramUser CreatedBy { get; set; } = default!;
}
