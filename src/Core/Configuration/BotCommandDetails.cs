using SauceNAO.Core.Services;

namespace SauceNAO.Core.Configuration;

/// <summary>
/// Represents the details of a bot command.
/// </summary>
/// <param name="Name">The command name.</param>
/// <param name="Description">A description for the command.</param>
/// <param name="Type">The service type to handle the command.</param>
/// <param name="Aliases">The aliases of the command.</param>
/// <param name="Scope">The command scope.</param>
public record BotCommandDetails(
    string Name,
    string Description,
    Type Type,
    IEnumerable<string>? Aliases,
    BotCommandScopeValue? Scope
) { }
