// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Models;

using IKB = Telegram.BotAPI.AvailableTypes.InlineKeyboardButton;
using IKM = Telegram.BotAPI.AvailableTypes.InlineKeyboardMarkup;

namespace SauceNAO.Core.Extensions
{
    internal static class SauceExtensions
    {
        internal static TCollection Add<TCollection>(this TCollection collection, string url, float similarity)
            where TCollection : ICollection<SauceUrl>
        {
            var sauceUrl = new SauceUrl(url, similarity);
            // Ignore duplicate
            if (!collection.Any(s => s.Url == sauceUrl.Url))
            {
                collection.Add(sauceUrl);
            }
            return collection;
        }
        internal static TCollection AddRange<TCollection>(this TCollection collection, IEnumerable<string> urls, float similarity)
            where TCollection : ICollection<SauceUrl>
        {
            foreach (var url in urls)
            {
                collection.Add(new SauceUrl(url, similarity));
            }
            return collection;
        }
        internal static IKB ToInlineButton(this SauceUrl sauce)
        {
            return IKB.SetUrl(sauce.Text, sauce.Url);
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
}
