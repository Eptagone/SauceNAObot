// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain;

/// <summary>
/// Represents a pantry.
/// </summary>
/// <param name="Recipes">A list of recipes to prepare sauces</param>
/// <param name="Ok">Indicates if the operation was successful.</param>
/// <param name="IsSearchLimitReached">Indicates if the search limit is reached.</param>
public record Pantry(IEnumerable<Recipe> Recipes, bool Ok, bool IsSearchLimitReached);
