using SauceNAO.Core.Models;

namespace SauceNAO.Core.Entities;

/// <summary>
/// Represents a media like an image or video that which sauce was searched for.
/// </summary>
public class MediaFile(string fileUniqueId, string fileId) : EntityBase, ITimestampable
{
    /// <summary>
    /// Unique identifier for this file, which is supposed to be the same over time and for different bots.
    /// </summary>
    public string FileUniqueId { get; set; } = fileUniqueId;

    /// <summary>
    /// Identifier for this file, which can be used to download or reuse the file.
    /// </summary>
    public string FileId { get; set; } = fileId;

    /// <summary>
    /// The file's mime type.
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// Identifier for the thumbnail file, if any.
    /// </summary>
    public string? ThumbnailFileId { get; set; }

    /// <summary>
    /// The thumbnail's mime type.
    /// </summary>
    public string? ThumbnailMimeType { get; set; }

    /// <summary>
    /// The file's media type (photo, video, etc.).
    /// Used to send a message to the user with the media if needed.
    /// </summary>
    public TelegramMediaType MediaType { get; set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// List of sauces that were found for this media.
    /// </summary>
    public IList<Sauce> Sauces { get; set; } = [];
}
