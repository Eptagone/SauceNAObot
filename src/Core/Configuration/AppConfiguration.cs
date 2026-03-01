namespace SauceNAO.Core.Configuration;

/// <summary>
/// Defines the basic configuration for the application.
/// </summary>
public sealed record AppConfiguration
{
    /// <summary>
    /// The section name in the configuration.
    /// </summary>
    public static string SectionName => "App";

    /// <summary>
    /// The application url. Required to enable the webhook feature and generate temporary urls for images.
    /// </summary>
    public string? ApplicationUrl { get; set; }

    /// <summary>
    /// The invitation link to the bot's support chat.
    /// </summary>
    public string? SupportChatInvitationLink { get; set; }
}
