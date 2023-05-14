// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Resources;
using System.Globalization;

namespace SauceNAO.Core.Models;

public sealed class Sauce : ISauce
{
	public string? Title { get; set; }
	public string? Characters { get; set; }
	public string? Material { get; set; }
	public string? Part { get; set; }
	public string? Year { get; set; }
	public string? EstTime { get; set; }
	public string? By { get; set; }

	internal string GetInfo(CultureInfo lang)
	{
		string info = string.Empty;
		if (!string.IsNullOrEmpty(this.Title))
		{
			info += this.Title;
		}
		if (!string.IsNullOrEmpty(this.Characters))
		{
			var characters = this.Characters;
			info += string.Format(characters, characters.Contains(',') ? MSG.ResultCharacters(lang) : MSG.ResultCharacter(lang));
		}
		if (!string.IsNullOrEmpty(this.Material))
		{
			info += string.Format(this.Material, MSG.ResultMaterial(lang));
		}
		if (!string.IsNullOrEmpty(this.Part))
		{
			info += string.Format(this.Part, MSG.ResultPart(lang));
		}
		if (!string.IsNullOrEmpty(this.By))
		{
			info += string.Format(this.By, MSG.ResultCreator(lang));
		}
		if (!string.IsNullOrEmpty(this.Year))
		{
			info += string.Format(this.Year, MSG.ResultYear(lang));
		}
		if (!string.IsNullOrEmpty(this.EstTime))
		{
			info += string.Format(this.EstTime, MSG.ResultTime(lang));
		}
		return string.IsNullOrEmpty(info) ? "unknown name" : info;
	}
}
