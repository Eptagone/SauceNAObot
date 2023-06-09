// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Entities;
using SauceNAO.Core.Models;
using IBB = Telegram.BotAPI.InlineButtonBuilder;
using IKB = Telegram.BotAPI.AvailableTypes.InlineKeyboardButton;
using IKM = Telegram.BotAPI.AvailableTypes.InlineKeyboardMarkup;

namespace SauceNAO.Core.Extensions;

internal static class SauceExtensions
{
	internal static IKB ToInlineButton(this SauceUrl sauce)
	{
		return IBB.SetUrl(sauce.Text, sauce.Url);
	}
	internal static IKM ToInlineKeyboardMarkup<TValue>(this TValue sauces)
		where TValue : IEnumerable<SauceUrl>
	{
		var cols = new List<IKB[]>();
		var rows = new List<IKB>();

		foreach (var s in sauces)
		{
			if (rows.Count < 3)
			{
				rows.Add(s.ToInlineButton());
			}
			else
			{
				cols.Add(rows.ToArray());
				rows.Clear();
				rows.Add(s.ToInlineButton());
			}
		}
		if (rows.Any())
		{
			cols.Add(rows.ToArray());
		}
		return new IKM(cols);
	}
}
