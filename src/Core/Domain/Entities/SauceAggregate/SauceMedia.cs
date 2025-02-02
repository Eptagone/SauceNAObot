// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain.Entities.SauceAggregate;

/// <summary>
/// Represents a media like an image or video that which sauce was searched for.
/// </summary>
public class SauceMedia : EntityBase
{
    /// <summary>
    /// Unique identifier for this file, which is supposed to be the same over time and for different bots.
    /// </summary>
    public required string FileUniqueId { get; set; }

    /// <summary>
    /// Identifier for this file, which can be used to download or reuse the file.
    /// </summary>
    public required string FileId { get; set; }

    /// <summary>
    /// Identifier for the thumbnail file, if any.
    /// </summary>
    public string? ThumbnailFileId { get; set; }

    /// <summary>
    /// The file's media type (photo, video, etc.).
    /// Used to send a message to the user with the media if needed.
    /// </summary>
    public virtual TelegramMediaType MediaType { get; set; }

    /// <summary>
    /// List of sauces that were found for this media.
    /// </summary>
    public virtual ICollection<Sauce> Sauces { get; set; } = [];
}
