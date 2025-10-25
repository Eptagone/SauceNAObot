namespace SauceNAO.Core.Services;

/// <summary>
/// Helper attribute to specify the priority of a service when it is registered.
/// Services with higher priority values are registered before services with lower priority values when registering services of the same type.
/// </summary>
/// <param name="priority">The priority value.</param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ServicePriorityAttribute(int priority) : Attribute
{
    /// <summary>
    /// Priority level to use when registering the target service.
    /// </summary>
    public int Priority => priority;
}
