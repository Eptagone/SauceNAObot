namespace SauceNAO.Core.Configuration;

/// <summary>
/// Represents the directory to store bot commands.
/// </summary>
public static class BotCommandDirectory
{
    private static readonly ICollection<BotCommandDetails> commands = [];

    /// <summary>
    /// Gets all registered commands.
    /// </summary>
    /// <returns>All registered commands.</returns>
    public static IEnumerable<BotCommandDetails> GetAll() => commands;

    /// <summary>
    /// Gets a command by its name or alias.
    /// </summary>
    /// <param name="nameOrAlias">The command name or alias.</param>
    /// <returns>The command details, or null if not found.</returns>
    public static BotCommandDetails? GetCommand(string nameOrAlias) =>
        commands.FirstOrDefault(c =>
            c.Name == nameOrAlias || c.Aliases?.Contains(nameOrAlias) == true
        );

    /// <summary>
    /// Registers a bot command.
    /// </summary>
    /// <param name="name">The command name.</param>
    /// <param name="description">A description for the command.</param>
    /// <param name="type">The service type to handle the command.</param>
    /// <param name="aliases">The aliases of the command.</param>
    /// <param name="scope">The command scope.</param>
    public static void RegisterCommand(
        string name,
        string description,
        Type type,
        IEnumerable<string>? aliases,
        BotCommandScopeValue? scope
    )
    {
        commands.Add(new BotCommandDetails(name, description, type, aliases, scope));
    }
}
