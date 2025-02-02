// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Domain;

public sealed class GeneralOptions
{
    /// <summary>
    /// The section name in the configuration.
    /// </summary>
    public static string SectionName => "General";

    /// <summary>
    /// The application url.
    /// </summary>
    public string? ApplicationURL { get; set; }

    /// <summary>
    /// The invitation link to the bot's support chat.
    /// </summary>
    public required string SupportChatInvitationLink { get; set; }

    /// <summary>
    /// The path where the files are stored when using the local bot api server. Examples:
    /// <list type="bullet">
    /// <item>/opt/tba/{BotToken}</item>
    /// <item>/var/files:/opt/tba/{BotToken}</item>
    /// </list>
    /// </summary>
    public string? FilesPath { get; set; }

    /// <summary>
    /// Path to the FFmpeg executable.
    /// </summary>
    public string? FFmpegPath { get; set; }
}
