// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using IKB = Telegram.BotAPI.AvailableTypes.InlineKeyboardButton;
using IKM = Telegram.BotAPI.AvailableTypes.InlineKeyboardMarkup;

namespace SauceNao.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed class SauceUrlList : List<SauceUrl>
    {
        public SauceUrlList()
        {
        }

        public SauceUrlList(IEnumerable<SauceUrl> collection) : base(collection)
        {
        }

        public SauceUrlList(int capacity) : base(capacity)
        {
        }

        internal void Add(string url, float similarity)
        {
            var sauceUrl = new SauceUrl(url, similarity);
            // Ignore duplicate
            if (this.Any(s => s.Url == sauceUrl.Url))
            {
                return;
            }
            Add(sauceUrl);
        }
        internal void AddRange(IEnumerable<string> urls, float similarity)
        {
            foreach (var url in urls)
            {
                Add(url, similarity);
            }
        }

        internal IKM ToInlineKeyboardMarkup()
        {
            var cols = new List<IKB[]>();
            var rows = new List<IKB>();
            for (int i = 0; i < Count; i++)
            {
                if (rows.Count < 3)
                {
                    rows.Add(this[i].ToInlineButton());
                }
                else
                {
                    cols.Add(rows.ToArray());
                    rows.Clear();
                    rows.Add(this[i].ToInlineButton());
                }
            }
            if (rows.Any())
            {
                cols.Add(rows.ToArray());
            }
            return new IKM(cols);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
