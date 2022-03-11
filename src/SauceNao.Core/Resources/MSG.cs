// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Globalization;
using System.Resources;

using SDIR = SauceNAO.Core.Resources.SauceDirectory;

#nullable disable

namespace SauceNAO.Core.Resources
{
    public static class MSG
    {
        private static readonly ResourceManager Res = BotMessages.ResourceManager;

        public static string About(CultureInfo lang)
        {
            return Res.GetString(nameof(About), lang);
        }

        public static string Anticheats(CultureInfo lang)
        {
            return Res.GetString(nameof(Anticheats), lang);
        }

        public static string AnticheatsAdded(CultureInfo lang, string botname)
        {
            return string.Format(
                Res.GetString(nameof(AnticheatsAdded), lang),
                botname);
        }

        public static string AnticheatsAlreadyExists(CultureInfo lang, string botname)
        {
            return string.Format(
                Res.GetString(nameof(AnticheatsAlreadyExists), lang),
                botname);
        }

        public static string AnticheatsDeleted(CultureInfo lang, string username)
        {
            return string.Format(
                Res.GetString(nameof(AnticheatsDeleted), lang),
                username);
        }

        public static string AnticheatsDoesNotExist(CultureInfo lang, string username)
        {
            return string.Format(
                Res.GetString(nameof(AnticheatsDoesNotExist), lang),
                username);
        }

        public static string AnticheatsMessage(CultureInfo lang)
        {
            return Res.GetString(nameof(AnticheatsMessage), lang);
        }

        public static string AntiCheatsNotABot(CultureInfo lang)
        {
            return Res.GetString(nameof(AntiCheatsNotABot), lang);
        }

        public static string ApiKey(CultureInfo lang)
        {
            return Res.GetString(nameof(ApiKey), lang);
        }

        public static string ApiKeyDeleted(CultureInfo lang)
        {
            return Res.GetString(nameof(ApiKeyDeleted), lang);
        }

        public static string ApiKeyNew(CultureInfo lang)
        {
            return Res.GetString(nameof(ApiKeyNew), lang);
        }

        public static string ApiKeyNone(CultureInfo lang)
        {
            return Res.GetString(nameof(ApiKeyNone), lang);
        }

        public static string ApiKeyStatus(CultureInfo lang, string apikey)
        {
            return string.Format(
                Res.GetString(nameof(ApiKeyStatus), lang),
                apikey);
        }

        public static string Busy(CultureInfo lang, string supportChatLink)
        {
            return string.Format(
                Res.GetString(nameof(Busy), lang),
                supportChatLink);
        }

        public static string BuyMeACookie(CultureInfo lang)
        {
            return Res.GetString(nameof(BuyMeACookie), lang);
        }

        public static string Creator(CultureInfo lang)
        {
            return Res.GetString(nameof(Creator), lang);
        }

        public static string EmptyRequest(CultureInfo lang)
        {
            return Res.GetString(nameof(EmptyRequest), lang);
        }

        public static string FailedConvertFile(CultureInfo lang)
        {
            return Res.GetString(nameof(FailedConvertFile), lang);
        }

        public static string GeneratingTmpUrl(CultureInfo lang)
        {
            return Res.GetString(nameof(GeneratingTmpUrl), lang);
        }

        public static string GroupsOnly(CultureInfo lang)
        {
            return Res.GetString(nameof(GroupsOnly), lang);
        }

        public static string Help(CultureInfo lang)
        {
            return Res.GetString(nameof(Help), lang);
        }

        public static string History(CultureInfo lang, string botUsername)
        {
            return string.Format(
                Res.GetString(nameof(History), lang),
                botUsername);
        }

        public static string HistoryButton(CultureInfo lang)
        {
            return Res.GetString(nameof(HistoryButton), lang);
        }

        public static string HistoryErased(CultureInfo lang)
        {
            return Res.GetString(nameof(HistoryErased), lang);
        }

        public static string HistoryNone(CultureInfo lang)
        {
            return Res.GetString(nameof(HistoryNone), lang);
        }

        public static string InvalidPhoto(CultureInfo lang)
        {
            return Res.GetString(nameof(InvalidPhoto), lang);
        }

        public static string LocalMode(CultureInfo lang)
        {
            return Res.GetString(nameof(LocalMode), lang);
        }

        public static string LocalModeFile(CultureInfo lang)
        {
            return Res.GetString(nameof(LocalModeFile), lang);
        }

        public static string MissingParams(CultureInfo lang)
        {
            return Res.GetString(nameof(MissingParams), lang);
        }

        public static string NotFound(string url, CultureInfo lang)
        {
            string _msg = Res.GetString(nameof(NotFound), lang);
            string google = string.Format(SDIR.GoogleImageSearch, url);
            string yandex = string.Format(SDIR.YandexUrl, url);
            string snao = string.Format(SDIR.SauceNAOsearch, url);
            return string.Format(_msg, google, yandex, snao);
        }

        public static string NotFoundLocalMode(CultureInfo lang)
        {
            return Res.GetString(nameof(NotFoundLocalMode), lang);
        }
        
        public static string PrivateChatsOnly(CultureInfo lang)
        {
            return Res.GetString(nameof(PrivateChatsOnly), lang);
        }

        public static string ResultCharacter(CultureInfo lang)
        {
            return Res.GetString(nameof(ResultCharacter), lang);
        }

        public static string ResultCharacters(CultureInfo lang)
        {
            return Res.GetString(nameof(ResultCharacters), lang);
        }

        public static string ResultCreator(CultureInfo lang)
        {
            return Res.GetString(nameof(ResultCreator), lang);
        }

        public static string ResultMaterial(CultureInfo lang)
        {
            return Res.GetString(nameof(ResultMaterial), lang);
        }

        public static string ResultPart(CultureInfo lang)
        {
            return Res.GetString(nameof(ResultPart), lang);
        }

        public static string ResultTime(CultureInfo lang)
        {
            return Res.GetString(nameof(ResultTime), lang);
        }

        public static string ResultYear(CultureInfo lang)
        {
            return Res.GetString(nameof(ResultYear), lang);
        }

        public static string Searching(CultureInfo lang)
        {
            return Res.GetString(nameof(Searching), lang);
        }

        public static string SetLang(CultureInfo lang)
        {
            return Res.GetString(nameof(SetLang), lang);
        }

        public static string SetLangSaved(CultureInfo lang)
        {
            return Res.GetString(nameof(SetLangSaved), lang);
        }

        public static string Source(CultureInfo lang)
        {
            return Res.GetString(nameof(Source), lang);
        }

        public static string Statistics(CultureInfo lang, int searchs, int users, int groupChats)
        {
            return string.Format(Res.GetString(nameof(Statistics), lang), searchs, users, groupChats);
        }

        public static string SupportChat(CultureInfo lang)
        {
            return Res.GetString(nameof(SupportChat), lang);
        }

        public static string TemporalUrlDone(CultureInfo lang, string tmpurl)
        {
            return string.Format(Res.GetString(nameof(TemporalUrlDone), lang), tmpurl);
        }

        public static string TooBigFile(CultureInfo lang)
        {
            return Res.GetString(nameof(TooBigFile), lang);
        }
    }
}
