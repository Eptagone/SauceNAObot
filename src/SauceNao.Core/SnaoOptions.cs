// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.Core;

/// <summary>
/// Configuration options for SauceNAO bot.
/// </summary>
public record SnaoOptions
{
	/// <summary>
	/// Gets or sets the bot token.
	/// </summary>
	public string? BotToken { get; init; }

	/// <summary>
	/// The API key to use SauceNAO API.
	/// </summary>
	public string? ApiKey { get; init; }

	/// <summary>
	/// A URL to the support chat.
	/// </summary>
	public string? SupportChatLink { get; init; }

	/// <summary>
	/// Optional. The URL to the application. This is used to generate temporal URLs for the files.
	/// If this is not set, the application will be working in local mode.
	/// </summary>
	public string? ApplicationUrl { get; init; }

	/// <summary>
	/// Optional. The FFMpeg executable path. This is used to extract the first frame of a video/gif.
	/// </summary>
	public string? FFmpegExec { get; init; }

	/// <summary>
	/// Optional. Secret Token to configure the webhook. If this is set, the bot will use a webhook instead of long polling.
	/// </summary>
	public string? SecretToken { get; init; }

	/// <summary>
	/// Optional. The path to the certificate file. This will be used to configure the webhook.
	/// </summary>
	public string? CertificatePath { get; init; }

	/// <summary>
	/// Optional. If true, the bot will drop all pending updates when it starts.
	/// </summary>
	public bool DropPendingUpdates { get; init; }
}
