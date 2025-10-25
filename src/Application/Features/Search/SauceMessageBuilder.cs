using System.Text;
using SauceNAO.Application.Resources;
using SauceNAO.Core;
using SauceNAO.Core.Entities.SauceAggregate;
using SauceNAO.Core.Services;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace SauceNAO.Application.Features.Search;

class SauceMessageBuilder(IBetterStringLocalizer<SauceMessageBuilder> localizer)
    : ISauceMessageBuilder
{
    private static readonly Dictionary<string, string> SITES =
        new()
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
            { "kemono.cr", "Kemono" },
        };

    private static readonly Dictionary<string, string> SITE_TEMPLATES =
        new()
        {
            { "AnidbAid", "https://anidb.net/perl-bin/animedb.pl?show=anime&amp;aid={0}" },
            { "AnimePicturesId", "https://anime-pictures.net/pictures/view_post/" },
            { "BcyId", "https://bcy.net/" },
            { "DA", "https://deviantart.com/view/" },
            { "DanbooruId", "https://danbooru.donmai.us/post/show/" },
            { "DrawrId", "http://drawr.net/show.php?id={0}" },
            { "e621Id", "https://e621.net/post/show/" },
            { "GelbooruId", "https://gelbooru.com/index.php?page=post&amp;s=view&amp;id={0}" },
            {
                "GoogleImageSearch",
                "https://www.google.com/searchbyimage?image_url={0}&amp;client=app"
            },
            { "ImdbId", "https://www.imdb.com/title/" },
            { "MyAnimeList", "http://myanimelist.net/anime.php?" },
            { "NijieId", "https://nijie.info/view.php?id={0}" },
            { "PawooId", "https://pawoo.net/@" },
            { "PixivId", "http://www.pixiv.net/member_illust.php?mode=medium&amp;illust_id={0}" },
            { "ProxyList", "https://gimmeproxy.com/api/getProxy?" },
            { "SankakuId", "https://chan.sankakucomplex.com/post/show/" },
            { "SauceNAO", "https://saucenao.com/" },
            { "SauceNAOsearch", "https://saucenao.com/search.php?url={0}" },
            { "SeigaId", "https://seiga.nicovideo.jp/" },
            { "TinEye", "https://tineye.com/search?" },
            { "YandereId", "https://yande.re/post/show/" },
            { "YandexUrl", "https://yandex.com/images/search?url={0}&amp;rpt=imageview" },
        };

    private string CharacterLabel => localizer["CharacterLabel"];
    private string CharactersLabel => localizer["CharactersLabel"];
    private string MaterialLabel => localizer["MaterialLabel"];
    private string PartLabel => localizer["PartLabel"];
    private string CreatorLabel => localizer["CreatorLabel"];
    private string YearLabel => localizer["YearLabel"];
    private string EstimationTimeLabel => localizer["EstimationTimeLabel"];

    public InlineKeyboardMarkup BuildKeyboard(IEnumerable<Sauce> sauces, float similarity)
    {
        var filteredSauces = sauces
            .Where(recipe => recipe.Similarity >= similarity)
            .OrderByDescending(recipe => recipe.Similarity);

        var keyboard = new InlineKeyboardBuilder();
        foreach (var originalUrl in filteredSauces.SelectMany(r => r.Links).Distinct())
        {
            var finalUrl = originalUrl;
            var text = "URL";
            // Fix Pixiv URLs
            if (originalUrl.Contains("i.pximg.net"))
            {
                string pid = Path.GetFileNameWithoutExtension(originalUrl);
                finalUrl = string.Format(SITE_TEMPLATES["PixivId"], pid.Split('_')[0]);
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

        return new InlineKeyboardMarkup(keyboard);
    }

    public string BuildText(IEnumerable<Sauce> sauces, float similarity, string? languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode))
        {
            localizer.ChangeCulture(languageCode);
        }

        var filteredSauces = sauces
            .Where(recipe => recipe.Similarity >= similarity)
            .OrderByDescending(recipe => recipe.Similarity);

        var builder = new StringBuilder();
        var title = filteredSauces.FirstNonEmpty(r => r.Title);
        var author = filteredSauces.FirstNonEmpty(r => r.Author);
        var characters = filteredSauces.FirstNonEmpty(r => r.Characters);
        var material = filteredSauces.FirstNonEmpty(r => r.Material);
        var part = filteredSauces.FirstNonEmpty(r => r.Part);
        var year = filteredSauces.FirstNonEmpty(r => r.Year);
        var estimationTime = filteredSauces.FirstNonEmpty(r => r.EstimationTime);

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
            builder.Append(HtmlTextFormatter.Bold($"{localizer[label]}: "));
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

        var messageText = builder.ToString();
        // Truncate the text if it's longer than 4096 characters.
        if (messageText.Length > 4096)
        {
            messageText = messageText[..4096];
        }
        else if (string.IsNullOrWhiteSpace(messageText))
        {
            messageText = "Unknown sauce";
        }

        return messageText;
    }
}
