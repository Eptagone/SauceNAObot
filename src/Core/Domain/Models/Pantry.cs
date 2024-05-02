// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain;

/// <summary>
/// Represents a pantry.
/// </summary>
/// <param name="Recipes">A list of recipes to prepare sauces</param>
/// <param name="IsChefAvailable">Indicates if a chef is available to cook sauces. (Only premium accounts can cook)</param>
/// <param name="RemainingSauces">The number of sauces that can be prepared.</param>
/// <param name="IsKitchenBusy">Indicates if the kitchen is busy. (Search limit reached)</param>
public record Pantry(
    IEnumerable<Recipe> Recipes,
    bool IsChefAvailable,
    int RemainingSauces,
    bool IsKitchenBusy
);
