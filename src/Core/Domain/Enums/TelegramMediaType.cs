// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain;

/// <summary>
/// Represents a media type of a Telegram message.
/// </summary>
public enum TelegramMediaType
{
    /// <summary>
    /// Represents an animation.
    /// </summary>
    Animation,

    /// <summary>
    /// Represents a document.
    /// </summary>
    Document,

    /// <summary>
    /// Represents a photo.
    /// </summary>
    Photo,

    /// <summary>
    /// Represents a video.
    /// </summary>
    Video,

    /// <summary>
    /// Represents a sticker.
    /// </summary>
    Sticker,
}
