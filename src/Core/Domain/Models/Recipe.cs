// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain;

/// <summary>
/// Represents a sauce recipe.
/// </summary>
/// <param name="Similarity">Score indicating how similar the result is to the search query.</param>
/// <param name="IndexId">Index ID of the sauce.</param>
/// <param name="IndexName">Name of the index.</param>
/// <param name="Urls">Links to the source of the image or video.</param>
/// <param name="Title">Title associated with the image.</param>
/// <param name="Author">Author of the image.</param>
/// <param name="Characters">Characters depicted in the image.</param>
/// <param name="Material">Material related to the image.</param>
/// <param name="Part">Part of the series (e.g., sequel, third part) the image is derived from.</param>
/// <param name="Year">Year associated with the image.</param>
/// <param name="EstimationTime">Estimated timestamp in the video from which the image is captured.</param>
public record Recipe(
    float Similarity,
    SauceIndex IndexId,
    string IndexName,
    IEnumerable<string> Urls,
    string? Title,
    string? Author,
    string? Characters,
    string? Material,
    string? Part,
    string? Year,
    string? EstimationTime
);
