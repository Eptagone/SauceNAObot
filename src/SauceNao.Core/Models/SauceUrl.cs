// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json.Serialization;
using SDIR = SauceNAO.Core.Resources.SauceDirectory;



namespace SauceNAO.Core.Models;

public sealed class SauceUrl
{
	public SauceUrl() { }
	public SauceUrl(string url, float similitary)
	{
		this.Url = url;
		if (url.Contains("i.pximg.net"))
		{
			string pid = Path.GetFileNameWithoutExtension(url);
			this.Url = string.Format(SDIR.PixivId, pid.Split('_')[0]);
		}
		this.Similarity = similitary;
		if (this.Url.Contains("pixiv"))
		{
			this.Text = "Pixiv";
		}
		else if (this.Url.Contains("danbooru"))
		{
			this.Text = "Danbooru";
		}
		else if (this.Url.Contains("gelbooru"))
		{
			this.Text = "Gelbooru";
		}
		else if (this.Url.Contains("sankaku"))
		{
			this.Text = "Sankaku";
		}
		else if (this.Url.Contains("anime-pictures.net"))
		{
			this.Text = "Anime Pictures";
		}
		else if (this.Url.Contains("yande.re"))
		{
			this.Text = "Yandere";
		}
		else if (this.Url.Contains("imdb"))
		{
			this.Text = "IMDB";
		}
		else if (this.Url.Contains("deviantart"))
		{
			this.Text = "Deviantart";
		}
		else if (this.Url.Contains("twitter"))
		{
			this.Text = "Twitter";
		}
		else if (this.Url.Contains("nijie.info"))
		{
			this.Text = "Nijie";
		}
		else if (this.Url.Contains("pawoo.net"))
		{
			this.Text = "Pawoo";
		}
		else if (this.Url.Contains("seiga.nicovideo.jp"))
		{
			this.Text = "Seiga Nicovideo";
		}
		else if (this.Url.Contains("tumblr.com"))
		{
			this.Text = "Tumblr";
		}
		else if (this.Url.Contains("anidb.net"))
		{
			this.Text = "Anidb";
		}
		else if (this.Url.Contains("sankakucomplex.com"))
		{
			this.Text = "Sankaku";
		}
		else if (this.Url.Contains("mangadex.org"))
		{
			this.Text = "MangaDex";
		}
		else if (this.Url.Contains("mangaupdates.com"))
		{
			this.Text = "MangaUpdates";
		}
		else if (this.Url.Contains("myanimelist.net"))
		{
			this.Text = "MyAnimeList";
		}
		else if (this.Url.Contains("furaffinity.net"))
		{
			this.Text = "FurAffinity";
		}
		else
		{
			this.Text = "URL";
		}
	}

	[JsonPropertyName("Text")]
	public string Text { get; set; }
	[JsonPropertyName("url")]
	public string Url { get; set; }
	[JsonPropertyName("similarity")]
	public float Similarity { get; set; }
}
