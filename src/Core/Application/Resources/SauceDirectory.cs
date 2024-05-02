// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Globalization;
using System.Resources;

namespace SauceNAO.Application.Resources;

public class SauceDirectory
{
    private static ResourceManager? resourceMan;

    /// <summary>
    ///   Returns the cached ResourceManager instance used by this class.
    /// </summary>
    public static ResourceManager ResourceManager =>
        resourceMan ??= new ResourceManager(
            "SauceNAO.Application.Resources.SauceDirectory",
            typeof(SauceDirectory).Assembly
        );

    /// <summary>
    ///   Looks up a localized string similar to https://anidb.net/perl-bin/animedb.pl?show=anime&amp;aid={0}.
    /// </summary>
    public static string AnidbAid
    {
        get { return ResourceManager.GetString("AnidbAid", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://anime-pictures.net/pictures/view_post/.
    /// </summary>
    public static string AnimePicturesId
    {
        get { return ResourceManager.GetString("AnimePicturesId", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://bcy.net/.
    /// </summary>
    public static string BcyId
    {
        get { return ResourceManager.GetString("BcyId", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://deviantart.com/view/.
    /// </summary>
    public static string DA
    {
        get { return ResourceManager.GetString("DA", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://danbooru.donmai.us/post/show/.
    /// </summary>
    public static string DanbooruId
    {
        get { return ResourceManager.GetString("DanbooruId", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to http://drawr.net/show.php?id={0}.
    /// </summary>
    public static string DrawrId
    {
        get { return ResourceManager.GetString("DrawrId", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://e621.net/post/show/.
    /// </summary>
    public static string e621Id
    {
        get { return ResourceManager.GetString("e621Id", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://gelbooru.com/index.php?page=post&amp;s=view&amp;id={0}.
    /// </summary>
    public static string GelbooruId
    {
        get { return ResourceManager.GetString("GelbooruId", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://www.google.com/searchbyimage?image_url={0}.
    /// </summary>
    public static string GoogleImageSearch
    {
        get
        {
            return ResourceManager.GetString("GoogleImageSearch", CultureInfo.InvariantCulture)!;
        }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://www.imdb.com/title/.
    /// </summary>
    public static string ImdbId
    {
        get { return ResourceManager.GetString("ImdbId", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to http://myanimelist.net/anime.php?.
    /// </summary>
    public static string MyAnimeList
    {
        get { return ResourceManager.GetString("MyAnimeList", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://nijie.info/view.php?id={0}.
    /// </summary>
    public static string NijieId
    {
        get { return ResourceManager.GetString("NijieId", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://pawoo.net/@.
    /// </summary>
    public static string PawooId
    {
        get { return ResourceManager.GetString("PawooId", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to http://www.pixiv.net/member_illust.php?mode=medium&amp;illust_id={0}.
    /// </summary>
    public static string PixivId
    {
        get { return ResourceManager.GetString("PixivId", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://gimmeproxy.com/api/getProxy?.
    /// </summary>
    public static string ProxyList
    {
        get { return ResourceManager.GetString("ProxyList", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://chan.sankakucomplex.com/post/show/.
    /// </summary>
    public static string SankakuId
    {
        get { return ResourceManager.GetString("SankakuId", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://saucenao.com/.
    /// </summary>
    public static string SauceNAO
    {
        get { return ResourceManager.GetString("SauceNAO", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://saucenao.com/search.php?url={0}.
    /// </summary>
    public static string SauceNAOsearch
    {
        get { return ResourceManager.GetString("SauceNAOsearch", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://seiga.nicovideo.jp/.
    /// </summary>
    public static string SeigaId
    {
        get { return ResourceManager.GetString("SeigaId", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://tineye.com/search?.
    /// </summary>
    public static string TinEye
    {
        get { return ResourceManager.GetString("TinEye", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://yande.re/post/show/.
    /// </summary>
    public static string YandereId
    {
        get { return ResourceManager.GetString("YandereId", CultureInfo.InvariantCulture)!; }
    }

    /// <summary>
    ///   Looks up a localized string similar to https://yandex.com/images/search?url={0}&amp;rpt=imageview.
    /// </summary>
    public static string YandexUrl
    {
        get { return ResourceManager.GetString("YandexUrl", CultureInfo.InvariantCulture)!; }
    }
}
