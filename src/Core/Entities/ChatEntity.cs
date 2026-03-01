using System.ComponentModel.DataAnnotations;

namespace SauceNAO.Core.Entities;

/// <summary>
/// Represents a telegram group or channel.
/// </summary>
/// <param name="chatId">Unique identifier for this chat.</param>
/// <param name="title">Title of the group or channel.</param>
public class ChatEntity(long chatId, string title) : LocalizableEntityBase
{
    /// <summary>
    /// Unique identifier for this chat.
    /// </summary>
    public long ChatId { get; set; } = chatId;

    /// <summary>
    /// Title of the group or channel.
    /// </summary>
    [StringLength(256)]
    public string Title { get; set; } = title;

    /// <summary>
    /// Optional. Username of the group or channel.
    /// </summary>
    [StringLength(32)]
    public string? Username { get; set; }
}
