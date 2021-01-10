// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using System.Globalization;
using System.Resources;

using SDIR = SauceNAO.Resources.SauceDirectory;

namespace SauceNAO.Resources
{
    internal static class MSG
    {
        private static readonly ResourceManager Res = Messages.ResourceManager;

        internal static string About(CultureInfo lang) => Res.GetString("About", lang);
        internal static string Anticheats(CultureInfo lang) => Res.GetString("Anticheats", lang);
        internal static string AnticheatsAdded(CultureInfo lang, string username) => string.Format(Res.GetString("AnticheatsAdded", lang), username);
        internal static string AnticheatsDeleted(CultureInfo lang, string username) => string.Format(Res.GetString("AnticheatsDeleted", lang), username);
        internal static string AnticheatsMessage(CultureInfo lang) => Res.GetString("AnticheatsMessage", lang);
        internal static string AnticheatsOff(CultureInfo lang) => Res.GetString("AnticheatsOff", lang);
        internal static string AnticheatsOn(CultureInfo lang) => Res.GetString("AnticheatsOn", lang);
        internal static string Busy(CultureInfo lang) => Res.GetString("Busy", lang);
        internal static string Cancel(CultureInfo lang) => Res.GetString("Cancel", lang);
        internal static string Cancelled(CultureInfo lang) => Res.GetString("Cancelled", lang);
        internal static string Confirm(CultureInfo lang) => Res.GetString("Confirm", lang);
        internal static string EmptyRequest(CultureInfo lang) => Res.GetString("EmptyRequest", lang);
        internal static string HistoryDelConfirm(CultureInfo lang) => Res.GetString("HistoryDelConfirm", lang);
        internal static string FailedConvertFile(CultureInfo lang) => Res.GetString("FailedConvertFile", lang);
        internal static string GeneratingTmpUrl(CultureInfo lang) => Res.GetString("GeneratingTmpUrl", lang);
        internal static string Help(CultureInfo lang) => Res.GetString("Help", lang);
        internal static string History(CultureInfo lang) => Res.GetString("History", lang);
        internal static string HistoryErased(CultureInfo lang) => Res.GetString("HistoryErased", lang);
        internal static string InvalidPhoto(CultureInfo lang) => Res.GetString("InvalidPhoto", lang);
        internal static string MissingParams(CultureInfo lang) => Res.GetString("MissingParams", lang);
        internal static string NotFound(string url, CultureInfo lang)
        {
            string _msg = Res.GetString("NotFound", lang);
            string google = string.Format(SDIR.GoogleImageSearch, url);
            string yandex = string.Format(SDIR.YandexUrl, url);
            string snao = string.Format(SDIR.SauceNAOsearch, url);
            return string.Format(_msg, google, yandex, snao);
        }
        internal static string ResultCharacter(CultureInfo lang) => Res.GetString("ResultCharacter", lang);
        internal static string ResultCharacters(CultureInfo lang) => Res.GetString("ResultCharacters", lang);
        internal static string ResultCreator(CultureInfo lang) => Res.GetString("ResultCreator", lang);
        internal static string ResultMaterial(CultureInfo lang) => Res.GetString("ResultMaterial", lang);
        internal static string ResultPart(CultureInfo lang) => Res.GetString("ResultPart", lang);
        internal static string ResultTime(CultureInfo lang) => Res.GetString("ResultTime", lang);
        internal static string ResultYear(CultureInfo lang) => Res.GetString("ResultYear", lang);
        internal static string Searching(CultureInfo lang) => Res.GetString("Searching", lang);
        internal static string Source(CultureInfo lang) => Res.GetString("Source", lang);
        internal static string TmpUrlDone(CultureInfo lang, string tmpurl) => string.Format(Res.GetString("TmpUrlDone", lang), tmpurl);
        internal static string TooBigFile(CultureInfo lang) => Res.GetString("TooBigFile", lang);
    }
}
