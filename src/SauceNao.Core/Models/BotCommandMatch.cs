// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Core.Models;

/// <summary>This object represents the result of a bot command match.</summary>
public sealed class BotCommandMatch
{
	internal BotCommandMatch()
	{
		this.Name = string.Empty;
		this.Params = string.Empty;
	}
	internal BotCommandMatch(string name, string args)
	{
		this.Success = true;
		this.Name = name;
		this.Params = args;
	}

	/// <summary>Gets a value indicating whether the match is successful.</summary>
	/// <returns>true if the match is successful; otherwise, false.</returns>
	public bool Success { get; }
	/// <summary>Command name.</summary>
	public string Name { get; }
	/// <summary>Command parameters.</summary>
	public string Params { get; }
}
