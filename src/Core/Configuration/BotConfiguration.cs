namespace SauceNAO.Core.Configuration;

/// <summary>
/// Defines options to configure the Telegram bot client.
/// </summary>
public sealed record BotConfiguration
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
    /// The webhook URL to which the bot will send updates.
    /// If not set, the application url will be used instead.
    /// </summary>
    public string? WebhookUrl { get; set; }

    /// <summary>
    /// The secret token used to configure the webhook. If not set, the bot will use long polling.
    /// </summary>f
    public string? SecretToken { get; set; }

    /// <summary>
    /// A custom endpoint where the bot will be sending requests instead of the default one (https://api.telegram.org).
    /// </summary>
    public string? ServerAddress { get; set; }
}
