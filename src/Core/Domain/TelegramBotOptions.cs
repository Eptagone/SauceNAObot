// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain;

/// <summary>
/// Defines options for the Telegram bot.
/// </summary>
public sealed class TelegramBotOptions
{
    /// <summary>
    /// The section name in the configuration.
    /// </summary>
    public static string SectionName => "TelegramBot";

    /// <summary>
    /// The bot token provided by BotFather.
    /// </summary>
    public required string BotToken { get; set; }

    /// <summary>
    /// The webhook URL. If not set, the application url will be used instead if available.
    /// </summary>
    public string? WebhookUrl { get; set; }

    /// <summary>
    /// The secret token used to configure the webhook. If not set, if not set, the bot will use long polling.
    /// </summary>f
    public string? SecretToken { get; set; }

    /// <summary>
    /// A custom endpoint where the bot will be sending requests instead of the default one (https://api.telegram.org).
    /// </summary>
    public string? ServerAddress { get; set; }
}
