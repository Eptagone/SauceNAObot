using SauceNAO.Core.Entities.SauceAggregate;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Services;

/// <summary>
/// Provides methods to build messages with the sauces.
/// </summary>
public interface ISauceMessageBuilder
{
    /// <summary>
    /// Build the inline keyboard from the sauces.
    /// </summary>
    /// <param name="sauces">A collection of sauces</param>
    /// <param name="similarity">Minimum similarity to filter the sauces</param>
    /// <returns>The inline keyboard</returns>
    InlineKeyboardMarkup BuildKeyboard(IEnumerable<Sauce> sauces, float similarity);

    /// <summary>
    /// Build the message text from the sauces.
    /// </summary>
    /// <param name="sauces">A collection of sauces</param>
    /// <param name="similarity">Minimum similarity to filter the sauces</param>
    /// <param name="languageCode">The language code to use when building the message</param>
    /// <returns>The message text</returns>
    string BuildText(IEnumerable<Sauce> sauces, float similarity, string? languageCode);
}
