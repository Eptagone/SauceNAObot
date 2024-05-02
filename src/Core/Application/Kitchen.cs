// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text;
using Microsoft.Extensions.Localization;
using SauceNAO.Application.Resources;
using SauceNAO.Domain;
using SauceNAO.Domain.Entities.SauceAggregate;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application;

/// <summary>
/// Defines a class to cook sauces.
/// </summary>
/// <param name="localizer">The localizer.</param>
class Kitchen(IStringLocalizer localizer)
{
    private static readonly IReadOnlyDictionary<string, string> SITES = new Dictionary<
        string,
        string
    >()
    {
        { "pixiv", "Pixiv" },
        { "danbooru", "Danbooru" },
        { "gelbooru", "Gelbooru" },
        { "sankaku", "Sankaku" },
        { "anime-pictures.net", "Anime Pictures" },
        { "yande.re", "Yandere" },
        { "imdb", "IMDB" },
        { "deviantart", "Deviantart" },
        { "patreon", "Patreon" },
        { "anilist", "AniList" },
        { "artstation", "ArtStation" },
        { "twitter", "Twitter" },
        { "nijie.info", "Nijie" },
        { "pawoo.net", "Pawoo" },
        { "seiga.nicovideo.jp", "Seiga Nicovideo" },
        { "tumblr.com", "Tumblr" },
        { "anidb.net", "Anidb" },
        { "sankakucomplex.com", "Sankaku" },
        { "mangadex.org", "MangaDex" },
        { "mangaupdates.com", "MangaUpdates" },
        { "myanimelist.net", "MyAnimeList" },
        { "furaffinity.net", "FurAffinity" },
        { "fakku.net", "FAKKU!" },
        { "nhentai.net", "nhentai" },
        { "e-hentai.org", "E-Hentai" },
        { "e621.net", "e621" },
        { "kemono.su", "Kemono" },
    };

    private readonly IStringLocalizer localizer = localizer;

    private string CharacterLabel => this.localizer["CharacterLabel"];
    private string CharactersLabel => this.localizer["CharactersLabel"];
    private string MaterialLabel => this.localizer["MaterialLabel"];
    private string PartLabel => this.localizer["PartLabel"];
    private string CreatorLabel => this.localizer["CreatorLabel"];
    private string YearLabel => this.localizer["YearLabel"];
    private string EstimationTimeLabel => this.localizer["EstimationTimeLabel"];

    /// <summary>
    /// Build the sauce message and the buttons.
    /// </summary>
    /// <param name="foundSauces">The found sauces.</param>
    /// <param name="similarity">The similarity threshold.</param>
    /// <returns></returns>
    public ValueTuple<string, InlineKeyboardMarkup> CookSauce(
        IEnumerable<Sauce> foundSauces,
        float similarity
    )
    {
        var sauces = foundSauces
            .Where(recipe => recipe.Similarity >= similarity)
            .OrderByDescending(recipe => recipe.Similarity);
        var builder = new StringBuilder();
        var title = sauces.FirstNonEmpty(r => r.Title);
        var author = sauces.FirstNonEmpty(r => r.Author);
        var characters = sauces.FirstNonEmpty(r => r.Characters);
        var material = sauces.FirstNonEmpty(r => r.Material);
        var part = sauces.FirstNonEmpty(r => r.Part);
        var year = sauces.FirstNonEmpty(r => r.Year);
        var estimationTime = sauces.FirstNonEmpty(r => r.EstimationTime);

        // Add the title to the sauce.
        if (!string.IsNullOrEmpty(title))
        {
            builder.AppendLine(HtmlTextFormatter.Bold(title));
            builder.AppendLine();
        }
        // Characters
        if (!string.IsNullOrEmpty(characters))
        {
            var label = characters.Contains(',') ? this.CharactersLabel : this.CharacterLabel;
            builder.Append(HtmlTextFormatter.Bold($"{this.localizer[label]}: "));
            builder.AppendLine(HtmlTextFormatter.EncodeHtmlCharacters(characters));
        }
        // Material
        if (!string.IsNullOrEmpty(material))
        {
            builder.Append(HtmlTextFormatter.Bold($"{this.MaterialLabel}: "));
            builder.AppendLine(HtmlTextFormatter.EncodeHtmlCharacters(material));
        }
        // Part
        if (!string.IsNullOrEmpty(part))
        {
            builder.Append(HtmlTextFormatter.Bold($"{this.PartLabel}: "));
            builder.AppendLine(HtmlTextFormatter.EncodeHtmlCharacters(part));
        }
        // Creator
        if (!string.IsNullOrEmpty(author))
        {
            builder.Append(HtmlTextFormatter.Bold($"{this.CreatorLabel}: "));
            builder.AppendLine(HtmlTextFormatter.EncodeHtmlCharacters(author));
        }
        // Year
        if (!string.IsNullOrEmpty(year))
        {
            builder.Append(HtmlTextFormatter.Bold($"{this.YearLabel}: "));
            builder.AppendLine(HtmlTextFormatter.EncodeHtmlCharacters(year));
        }
        // Estimation Time
        if (!string.IsNullOrEmpty(estimationTime))
        {
            builder.Append(HtmlTextFormatter.Bold($"{this.EstimationTimeLabel}: "));
            builder.AppendLine(HtmlTextFormatter.EncodeHtmlCharacters(estimationTime));
        }

        var keyboard = new InlineKeyboardBuilder();
        foreach (var originalUrl in sauces.SelectMany(r => r.Links).Distinct())
        {
            var finalUrl = originalUrl;
            var text = "URL";
            // Fix Pixiv URLs
            if (originalUrl.Contains("i.pximg.net"))
            {
                string pid = Path.GetFileNameWithoutExtension(originalUrl);
                finalUrl = string.Format(SauceDirectory.PixivId, pid.Split('_')[0]);
            }
            // Fix Kemono URLs
            else if (originalUrl.Contains("kemono.party"))
            {
                finalUrl = originalUrl.Replace("kemono.party", "kemono.su");
            }

            // Get the site name from the URL.
            foreach (var site in SITES)
            {
                if (finalUrl.Contains(site.Key))
                {
                    text = site.Value;
                    break;
                }
            }

            // Append a new row if the last row has 3 buttons.
            if (keyboard.Any() && keyboard.Last().Count() >= 3)
            {
                keyboard.AppendRow();
            }
            // Append the button to the buttons.
            keyboard.AppendUrl(text, finalUrl);
        }

        var sauceMsg = builder.ToString();
        // Truncate the text if it's longer than 4096 characters.
        if (sauceMsg.Length > 4096)
        {
            sauceMsg = sauceMsg[..4096];
        }
        else if (string.IsNullOrWhiteSpace(sauceMsg))
        {
            sauceMsg = "Unknown sauce";
        }

        // Return the sauce and the buttons.
        return (sauceMsg, new InlineKeyboardMarkup(keyboard));
    }
}
