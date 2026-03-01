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
