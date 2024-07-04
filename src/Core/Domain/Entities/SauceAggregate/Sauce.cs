// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain.Entities.SauceAggregate;

/// <summary>
/// Represents a result from a search using the SauceNAO API.
/// </summary>
public class Sauce : EntityBase
{
    /// <summary>
    /// Score indicating how similar the result is to the search query.
    /// </summary>
    public float Similarity { get; set; }

    /// <summary>
    /// Links to the source of the image or video.
    /// </summary>
    public IList<string> Links { get; set; } = [];

    /// <summary>
    /// Title associated with the image.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Author of the image.
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Characters depicted in the image.
    /// </summary>
    public string? Characters { get; set; }

    /// <summary>
    /// Material related to the image.
    /// </summary>
    public string? Material { get; set; }

    /// <summary>
    /// Part of the series (e.g., sequel, third part) the image is derived from.
    /// </summary>
    public string? Part { get; set; }

    /// <summary>
    /// Year associated with the image.
    /// </summary>
    public string? Year { get; set; }

    /// <summary>
    /// Estimated timestamp in the video from which the image is captured.
    /// </summary>
    public string? EstimationTime { get; set; }

    /// <summary>
    /// A list of all media that were associated with this sauce.
    /// </summary>
    public virtual ICollection<SauceMedia> AssociatedMedias { get; set; } = [];
}
