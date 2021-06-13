// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.IO;
using System.Text.Json.Serialization;
using IKB = Telegram.BotAPI.AvailableTypes.InlineKeyboardButton;
using SDIR = SauceNao.Resources.SauceDirectory;

namespace SauceNao.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class SauceUrl
    {
        public SauceUrl() { }
        public SauceUrl(string url, float similitary)
        {
            Url = url;
            if (url.Contains("i.pximg.net"))
            {
                string pid = Path.GetFileNameWithoutExtension(url);
                Url = string.Format(SDIR.PixivId, pid.Split('_')[0]);
            }
            Similarity = similitary;
            if (Url.Contains("pixiv"))
            {
                Text = "Pixiv";
            }
            else if (Url.Contains("danbooru"))
            {
                Text = "Danbooru";
            }
            else if (Url.Contains("gelbooru"))
            {
                Text = "Gelbooru";
            }
            else if (Url.Contains("sankaku"))
            {
                Text = "Sankaku";
            }
            else if (Url.Contains("anime-pictures.net"))
            {
                Text = "Anime Pictures";
            }
            else if (Url.Contains("yande.re"))
            {
                Text = "Yandere";
            }
            else if (Url.Contains("imdb"))
            {
                Text = "IMDB";
            }
            else if (Url.Contains("deviantart"))
            {
                Text = "Deviantart";
            }
            else if (Url.Contains("twitter"))
            {
                Text = "Twitter";
            }
            else if (Url.Contains("nijie.info"))
            {
                Text = "Nijie";
            }
            else if (Url.Contains("pawoo.net"))
            {
                Text = "Pawoo";
            }
            else if (Url.Contains("seiga.nicovideo.jp"))
            {
                Text = "Seiga Nicovideo";
            }
            else if (Url.Contains("tumblr.com"))
            {
                Text = "Tumblr";
            }
            else if (Url.Contains("anidb.net"))
            {
                Text = "Anidb";
            }
            else if (Url.Contains("sankakucomplex.com"))
            {
                Text = "Sankaku";
            }
            else if (Url.Contains("mangadex.org"))
            {
                Text = "MangaDex";
            }
            else if (Url.Contains("mangaupdates.com"))
            {
                Text = "MangaUpdates";
            }
            else if (Url.Contains("myanimelist.net"))
            {
                Text = "MyAnimeList";
            }
            else if (Url.Contains("furaffinity.net"))
            {
                Text = "FurAffinity";
            }
            else
            {
                Text = "URL";
            }
        }

        [JsonPropertyName("Text")]
        public string Text { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("similarity")]
        public float Similarity { get; set; }

        internal IKB ToInlineButton()
        {
            return IKB.SetUrl(Text, Url);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
