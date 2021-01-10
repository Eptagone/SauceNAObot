// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using SauceNAO.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.BotAPI.Available_Types;
using File = System.IO.File;
using IKB = Telegram.BotAPI.Available_Types.InlineKeyboardButton;
using SDIR = SauceNAO.Resources.SauceDirectory;

namespace SauceNAO
{
    public static class Utilities
    {
        /// <summary>Base Url. Example: https://saucenaotelegramnetbot.net/ </summary>
        internal static string BaseUrl { get; set; }
        /// <summary>Address to recover files. Used to recover temporary files. Example: https://saucenaotelegramnetbot.net/temp?file={0}</summary>
        internal static string TempFilesPath { get; set; }
        public static List<int> ToIntList(this string[] input)
        {
            var sitems = input;
            var items = new int[sitems.Length];
            for (int i = 0; i < sitems.Length; i++)
            {
                items[i] = int.Parse(sitems[i]);
            }
            return items.ToList();
        }
        internal static string ParseHTMLTags(this string input)
            => input
            .Replace("<", "&lt;")
            .Replace(">", "&gt;");

        public static HttpClient Client { get; set; } = new HttpClient();

        internal static TargetFile GetMedia(this Message message)
        {
            // Create new output object
            TargetFile output = new TargetFile
            {
                Ok = false,
                FileId = string.Empty,
                UniqueId = string.Empty
            };
            // If media is a Photo
            if (message.Photo != null)
            {
                output.Ok = true; output.FileId = message.Photo[0].File_id;
                output.UniqueId = message.Photo[0].File_unique_id;
                output.HasMedia = true; output.Type = "photo";
                output.OriginalFileId = message.Photo[0].File_id;
                return output;
            }
            // If media is a Sticker
            if (message.Sticker != null)
            {
                output.HasMedia = true;
                if (!message.Sticker.Is_animated)
                {
                    output.Ok = true; output.FileId = message.Sticker.File_id;
                    output.UniqueId = message.Sticker.File_unique_id;
                    output.Type = "sticker";
                    output.OriginalFileId = message.Sticker.File_id;
                    return output;
                }
            }
            // If media is an Animation
            if (message.Animation != null)
            {
                output.Ok = true;
                if (message.Animation.Thumb != null)
                {
                    output.FileId = message.Animation.Thumb.File_id;
                    output.IsThumb = true;
                }
                else
                {
                    output.FileId = message.Animation.File_id;
                }
                output.HasMedia = true;
                output.UniqueId = message.Animation.File_unique_id;
                output.Type = "animation";
                output.OriginalFileId = message.Animation.File_id;
                return output;
            }
            // If media is a Video
            if (message.Video != null)
            {
                output.Ok = true;
                if (message.Video.Thumb != null)
                {
                    output.FileId = message.Video.Thumb.File_id;
                    output.IsThumb = true;
                }
                else
                {
                    output.FileId = message.Video.File_id;
                }
                output.HasMedia = true;
                output.UniqueId = message.Video.File_unique_id;
                output.Type = "video";
                output.OriginalFileId = message.Video.File_id;
                return output;
            }
            // If media is a Document
            if (message.Document != null)
            {
                output.HasMedia = true;
                if (message.Document.Mime_type.Contains("image"))
                {
                    output.Ok = true; output.FileId = message.Document.File_id;
                    output.UniqueId = message.Document.File_unique_id;
                    output.Type = "document";
                    output.OriginalFileId = message.Document.File_id;
                }
            }
            return output;
        }

        internal static async Task<byte[]> DownloadFileAsync(Uri path)
            => await Client.GetByteArrayAsync(path)
            .ConfigureAwait(false);

        internal static async Task<string> GenerateTmpFile(string filename, byte[] filedata)
        {
            string inpath = $"{Path.GetTempPath()}{filename}";
            if (File.Exists(inpath))
            {
                return string.Format(TempFilesPath, filename);
            }

            await WriteOnDisk(inpath, filedata).ConfigureAwait(false);
            return string.Format(TempFilesPath, filename);
        }

        internal static async Task<string> GenerateTmpFile(string filename, string origin)
        {
            byte[] data = await DownloadFileAsync(new Uri(origin))
                .ConfigureAwait(false);
            return await GenerateTmpFile(filename, data)
                .ConfigureAwait(false);
        }

        internal static async Task WriteOnDisk(string path, byte[] bytes)
        {
            try
            {
                await File.WriteAllBytesAsync(path, bytes).ConfigureAwait(false);
            }
            catch (IOException exp)
            {
                if (exp.Message == "There is not enough space on the disk")
                {
                    File.Delete($"{Path.GetTempPath()}*.*");
                    await File.WriteAllBytesAsync(path, bytes).ConfigureAwait(false);
                }
            }
        }

        internal static IKB[][] ButtonsFromUrls(string[] urls)
        {
            List<IKB> sources = new List<IKB>();
            foreach (string url in urls)
            {
                sources.Add(NewSourceButton(url));
            }
            int n_buttons = sources.Count;
            List<IKB[]> cols = new List<IKB[]>();
            List<IKB> rows = new List<IKB>();
            for (int i = 0; i < n_buttons; i++)
            {
                if (rows.Count < 3)
                {
                    rows.Add(sources[i]);
                }
                else
                {
                    cols.Add(rows.ToArray());
                    rows.Clear();
                    rows.Add(sources[i]);
                }
                if (i == n_buttons - 1)
                {
                    cols.Add(rows.ToArray());
                }
            }
            return cols.ToArray();
        }

        internal static IKB NewSourceButton(string url)
        {
            string text = "URL";
            if (url.Contains("i.pximg.net"))
            {
                string[] urlsplit = url.Split('/');
                url = string.Format(SDIR.PixivId, urlsplit[^1]);
            }
            if (url.Contains("pixiv"))
            {
                text = "Pixiv";
            }

            if (url.Contains("danbooru"))
            {
                text = "Danbooru";
            }

            if (url.Contains("gelbooru"))
            {
                text = "Gelbooru";
            }

            if (url.Contains("sankaku"))
            {
                text = "Sankaku";
            }

            if (url.Contains("anime-pictures.net"))
            {
                text = "Anime Pictures";
            }

            if (url.Contains("yande.re"))
            {
                text = "Yandere";
            }

            if (url.Contains("imdb"))
            {
                text = "IMDB";
            }

            if (url.Contains("deviantart"))
            {
                text = "Deviantart";
            }

            if (url.Contains("twitter"))
            {
                text = "Twitter";
            }

            if (url.Contains("nijie.info"))
            {
                text = "Nijie";
            }

            if (url.Contains("pawoo.net"))
            {
                text = "Pawoo";
            }

            if (url.Contains("seiga.nicovideo.jp"))
            {
                text = "Seiga Nicovideo";
            }

            if (url.Contains("tumblr.com"))
            {
                text = "Tumblr";
            }

            if (url.Contains("anidb.net"))
            {
                text = "Anidb";
            }

            if (url.Contains("sankakucomplex.com"))
            {
                text = "Sankaku";
            }

            if (url.Contains("mangadex.org"))
            {
                text = "MangaDex";
            }

            if (url.Contains("mangaupdates.com"))
            {
                text = "MangaUpdates";
            }

            if (url.Contains("myanimelist.net"))
            {
                text = "MyAnimeList";
            }

            return new IKB
            {
                Text = text,
                Url = url
            };
        }
    }
}
