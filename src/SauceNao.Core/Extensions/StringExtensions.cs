// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableMethods.FormattingOptions;

namespace SauceNAO.Core.Extensions
{
    public static class StringExtensions
    {
        private static readonly IStyleParser _parser = StyleParser.Default;
        private static readonly FormattingHelper _formattingHelper = new();

        /// <summary>
        /// Remove a substring from the end of a <see cref="string"/>.
        /// </summary>
        /// <param name="input">Input <see cref="string"/>.</param>
        /// <param name="suffix">Suffix.</param>
        /// <returns>The new <see cref="string"/>.</returns>
        public static string RemoveFromEnd(this string input, string? suffix)
        {
            if (!string.IsNullOrEmpty(suffix) && input.EndsWith(suffix))
            {
                return input[..^suffix.Length];
            }
            else
            {
                return input;
            }
        }

        /// <summary>
        /// Replaces symbols that are not part of an HTML tag or entity with HTML entities
        ///     (&lt; with &amp;lt;, &gt; with &amp;gt; and &amp; with &amp;amp;).
        /// </summary>
        /// <param name="text">Input text.</param>
        /// <returns>The parsed <see cref="string"/>.</returns>
        public static string ParseHtmlTags(this string? text)
        {
            return _parser.ToHTML(text);
        }

        /// <summary>
        /// Format text using HTML.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Stylized <see cref="string"/>.</returns>
        public static string AsHtmlNormalText(this string input)
        {
            return _parser.ToHTML(input);
        }
        
        /// <summary>
        /// Format text using HTML. Bold.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Stylized <see cref="string"/>.</returns>
        public static string AsHtmlBoldText(this string input)
        {
            return _formattingHelper.Bold(input, ParseModeKind.HTML);
        }

        /// <summary>
        /// Format text using HTML. Code.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Stylized <see cref="string"/>.</returns>
        public static string AsHtmlCodeText(this string input)
        {
            return _formattingHelper.Code(input, ParseModeKind.HTML);
        }

        /// <summary>
        /// Format text using HTML. Italic.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Stylized <see cref="string"/>.</returns>
        public static string AsHtmlItalicText(this string input)
        {
            return _formattingHelper.Code(input, ParseModeKind.HTML);
        }

        /// <summary>
        /// Format text using HTML. Pre.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Stylized <see cref="string"/>.</returns>
        public static string AsHtmlPreText(this string input)
        {
            return _formattingHelper.Pre(input, ParseModeKind.HTML);
        }

        /// <summary>
        /// Format text using HTML. Pre.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <param name="language">Programming language. Ex: python</param>
        /// <returns>Stylized <see cref="string"/>.</returns>
        public static string AsHtmlPreText(this string input, string language)
        {
            return _formattingHelper.Pre(input, language, ParseModeKind.HTML);
        }

        /// <summary>
        /// Format text using HTML. Spoiler.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Stylized <see cref="string"/>.</returns>
        public static string AsHtmlSpoiler(this string input)
        {
            return _formattingHelper.Spoiler(input);
        }

        /// <summary>
        /// Format text using HTML. Strikethrough.
        /// </summary>
        /// <param name="input">Input text</param>
        /// <returns></returns>
        public static string AsHtmlStrikethrough(this string input)
        {
            return _formattingHelper.Strikethrough(input, ParseModeKind.HTML);
        }

        /// <summary>
        /// Format text using HTML. Text link.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <param name="url">Url.</param>
        /// <returns>Stylized <see cref="string"/>.</returns>
        public static string AsHtmlTextLink(this string input, string url)
        {
            return _formattingHelper.TextLink(input, url, ParseModeKind.HTML);
        }

        /// <summary>
        /// Format text using HTML. Text Mention.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <param name="userId">Unique identifier for this user or bot.</param>
        /// <returns>Stylized <see cref="string"/>.</returns>
        public static string AsHtmlTextMention(this string input, long userId)
        {
            return _formattingHelper.TextMention(input, userId, ParseModeKind.HTML);
        }

        /// <summary>
        /// Format text using HTML. Underline.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Stylized <see cref="string"/>.</returns>
        public static string AsHtmlUnderline(this string input)
        {
            return _formattingHelper.Underline(input, ParseModeKind.HTML);
        }
    }
}
