// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.


using System.Globalization;
using System.Resources;

using SDIR = SauceNao.Resources.SauceDirectory;

namespace SauceNao.Resources
{
    internal static class MSG
    {
        private static readonly ResourceManager Res = Messages.ResourceManager;

        internal static string About(CultureInfo lang)
        {
            return Res.GetString("About", lang);
        }

        internal static string Anticheats(CultureInfo lang)
        {
            return Res.GetString("Anticheats", lang);
        }

        internal static string AnticheatsAdded(CultureInfo lang, string username)
        {
            return string.Format(Res.GetString("AnticheatsAdded", lang), username);
        }

        internal static string AnticheatsDeleted(CultureInfo lang, string username)
        {
            return string.Format(Res.GetString("AnticheatsDeleted", lang), username);
        }

        internal static string AnticheatsMessage(CultureInfo lang)
        {
            return Res.GetString("AnticheatsMessage", lang);
        }

        internal static string BugReports(CultureInfo lang, int botExceptions, int exceptions, int failedSauces)
        {
            return string.Format(Res.GetString("BugReports", lang), botExceptions, exceptions, failedSauces);
        }
        internal static string Busy(CultureInfo lang)
        {
            return Res.GetString("Busy", lang);
        }

        internal static string BuyMeACookie(CultureInfo lang)
        {
            return Res.GetString("BuyMeACookie", lang);
        }

        internal static string Cancel(CultureInfo lang)
        {
            return Res.GetString("Cancel", lang);
        }

        internal static string Cancelled(CultureInfo lang)
        {
            return Res.GetString("Cancelled", lang);
        }

        internal static string CleanUp(CultureInfo lang, int sauces, int files)
        {
            return string.Format(Res.GetString("CleanUp", lang), sauces, files);
        }

        internal static string Confirm(CultureInfo lang)
        {
            return Res.GetString("Confirm", lang);
        }

        internal static string Creator(CultureInfo lang)
        {
            return Res.GetString("Creator", lang);
        }

        internal static string EmptyRequest(CultureInfo lang)
        {
            return Res.GetString("EmptyRequest", lang);
        }

        internal static string FailedConvertFile(CultureInfo lang)
        {
            return Res.GetString("FailedConvertFile", lang);
        }

        internal static string GeneratingTmpUrl(CultureInfo lang)
        {
            return Res.GetString("GeneratingTmpUrl", lang);
        }

        internal static string GroupsOnly(CultureInfo lang)
        {
            return Res.GetString("GroupsOnly", lang);
        }

        internal static string Help(CultureInfo lang)
        {
            return Res.GetString("Help", lang);
        }

        internal static string History(CultureInfo lang, string botUsername)
        {
            return string.Format(Res.GetString("History", lang), botUsername);
        }

        internal static string HistoryButton(CultureInfo lang)
        {
            return Res.GetString("HistoryButton", lang);
        }

        internal static string HistoryErased(CultureInfo lang)
        {
            return Res.GetString("HistoryErased", lang);
        }

        internal static string InvalidPhoto(CultureInfo lang)
        {
            return Res.GetString("InvalidPhoto", lang);
        }

        internal static string Langs(CultureInfo lang)
        {
            return Res.GetString("Langs", lang);
        }

        internal static string MissingParams(CultureInfo lang)
        {
            return Res.GetString("MissingParams", lang);
        }

        internal static string NoInfo(CultureInfo lang)
        {
            return Res.GetString("NoInfo", lang);
        }

        internal static string NotFound(string url, CultureInfo lang)
        {
            string _msg = Res.GetString("NotFound", lang);
            string google = string.Format(SDIR.GoogleImageSearch, url);
            string yandex = string.Format(SDIR.YandexUrl, url);
            string snao = string.Format(SDIR.SauceNAOsearch, url);
            return string.Format(_msg, google, yandex, snao);
        }
        internal static string ResultCharacter(CultureInfo lang)
        {
            return Res.GetString("ResultCharacter", lang);
        }

        internal static string ResultCharacters(CultureInfo lang)
        {
            return Res.GetString("ResultCharacters", lang);
        }

        internal static string ResultCreator(CultureInfo lang)
        {
            return Res.GetString("ResultCreator", lang);
        }

        internal static string ResultMaterial(CultureInfo lang)
        {
            return Res.GetString("ResultMaterial", lang);
        }

        internal static string ResultPart(CultureInfo lang)
        {
            return Res.GetString("ResultPart", lang);
        }

        internal static string ResultTime(CultureInfo lang)
        {
            return Res.GetString("ResultTime", lang);
        }

        internal static string ResultYear(CultureInfo lang)
        {
            return Res.GetString("ResultYear", lang);
        }

        internal static string Searching(CultureInfo lang)
        {
            return Res.GetString("Searching", lang);
        }

        internal static string SetLang(CultureInfo lang)
        {
            return Res.GetString("SetLang", lang);
        }

        internal static string SetLangSaved(CultureInfo lang)
        {
            return Res.GetString("SetLangSaved", lang);
        }

        internal static string Source(CultureInfo lang)
        {
            return Res.GetString("Source", lang);
        }

        internal static string Statistics(CultureInfo lang, int searchs, int users, int groupChats)
        {
            return string.Format(Res.GetString("Statistics", lang), searchs, users, groupChats);
        }

        internal static string SupportChat(CultureInfo lang)
        {
            return Res.GetString("SupportChat", lang);
        }

        internal static string TmpUrlDone(CultureInfo lang, string tmpurl)
        {
            return string.Format(Res.GetString("TmpUrlDone", lang), tmpurl);
        }

        internal static string TooBigFile(CultureInfo lang)
        {
            return Res.GetString("TooBigFile", lang);
        }
    }
}
