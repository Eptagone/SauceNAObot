// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Core.Services;

/// <summary>
/// Defines the priority of a service
/// </summary>
/// <param name="priority">The priority of the service</param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class PriorityAttribute(int priority) : Attribute
{
    /// <summary>
    /// The priority of the service
    /// </summary>
    public int Priority { get; set; } = priority;
}
