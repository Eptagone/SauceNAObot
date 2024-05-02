// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SauceNAO.Domain;
using SauceNAO.Domain.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.Extensions;
using TelegramFile = Telegram.BotAPI.AvailableTypes.File;

namespace SauceNAO.Infrastructure.Services;

/// <summary>
/// Represents a service for working with Telegram files.
/// </summary>
/// <param name="botClient">The Telegram bot client.</param>
/// <param name="memoryCache">The memory cache.</param>
partial class TelegramFileService(
    ILogger<TelegramFileService> logger,
    ITelegramBotClient botClient,
    IMemoryCache memoryCache,
    IHttpClientFactory httpClientFactory,
    IOptions<GeneralOptions> options,
    IOptions<TelegramBotOptions> botOptions
) : ITelegramFileService
{
    private readonly ILogger<TelegramFileService> logger = logger;
    private readonly ITelegramBotClient client = botClient;
    private readonly IMemoryCache cache = memoryCache;
    private readonly HttpClient httpClient = httpClientFactory.CreateClient(
        nameof(TelegramFileService)
    );
    private readonly string? applicationUrl = options.Value.ApplicationURL;
    private readonly string? filesPath = options.Value.FilesPath;
    private readonly string BotToken = botOptions.Value.BotToken;

    /// <inheritdoc />
    public async Task<string?> GetFilePathAsync(string fileId, CancellationToken cancellationToken)
    {
        // Get the file information.
        var telegramFile = await this.GetTelegramFileAsync(fileId, cancellationToken);
        if (string.IsNullOrEmpty(telegramFile?.FilePath))
        {
            return null;
        }

        // If the bot is using a local bot api server and the files path was provided, use it.
        if (!string.IsNullOrWhiteSpace(this.filesPath))
        {
            var filesPath = this
                .filesPath.Replace(this.BotToken, "{BotToken}")
                .Replace("C:\\", "{WindowsDrive}");
            // Try to match the local path pattern.
            var appFilesPath = Path.TrimEndingDirectorySeparator(filesPath.Split(":").First())
                .Replace("{BotToken}", this.BotToken)
                .Replace("{WindowsDrive}", "C:\\");
            var originFilesPath = Path.TrimEndingDirectorySeparator(filesPath.Split(":").Last())
                .Replace("{BotToken}", this.BotToken)
                .Replace("{WindowsDrive}", "C:\\");

            return telegramFile.FilePath.Replace(originFilesPath, appFilesPath);
        }

        // Get the file from the cache or download it.
        return await this.cache.GetOrCreateAsync(
            $"FilePath:{fileId}",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);

                // Download the file and save it to the temporary directory.
                var downloadUrl = this.client.BuildFileDownloadLink(telegramFile);
                if (string.IsNullOrWhiteSpace(downloadUrl))
                {
                    return null;
                }
                var response = await this.httpClient.GetAsync(downloadUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var stream = await response.Content.ReadAsStreamAsync();
                var tmpPath = Path.Join(
                    Path.GetTempPath(),
                    Path.ChangeExtension(
                        Path.GetRandomFileName(),
                        Path.GetExtension(telegramFile.FilePath)
                    )
                );
                using var fileStream = File.Create(tmpPath);
                stream.CopyTo(fileStream);

                return tmpPath;
            }
        );
    }

    /// <inheritdoc />
    public Task<string?> GetFileUrlAsync(string fileId, CancellationToken cancellationToken) =>
        this.GetFileUrlAsync(fileId, false, cancellationToken);

    /// <inheritdoc />
    public async Task<string?> GetFileUrlAsync(
        string fileId,
        bool publicAccess,
        CancellationToken cancellationToken
    )
    {
        var telegramFile = await this.GetTelegramFileAsync(fileId, cancellationToken);
        if (string.IsNullOrEmpty(telegramFile?.FilePath))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(this.applicationUrl))
        {
            // If a public url is requested but there is no application URL then return null because the file is not accessible for public.
            if (publicAccess)
            {
                this.logger.LogTelegramFileUrlError();
                return null;
            }
        }
        // If the bot is using a local bot api server and the files path was provided then return the same path without token prefix.
        else if (!string.IsNullOrWhiteSpace(this.filesPath))
        {
            var filesPath = this
                .filesPath.Replace(this.BotToken, "{BotToken}")
                .Replace("C:\\", "{WindowsDrive}");
            var originFilesPath = Path.TrimEndingDirectorySeparator(filesPath.Split(":").Last())
                .Replace("{BotToken}", this.BotToken)
                .Replace("{WindowsDrive}", "C:\\");
            var path = telegramFile.FilePath.Replace(originFilesPath, string.Empty);

            if (path.Contains(this.BotToken))
            {
                throw new SecurityException("The generated path contains the bot token.");
            }

            // Create a URL to the file using the application URL.
            // This kind of URL is always public.
            return new Uri(new Uri(this.applicationUrl), $"/file{path}").ToString();
        }

        if (publicAccess)
        {
            if (!string.IsNullOrWhiteSpace(this.applicationUrl))
            {
                var localPath = await this.GetFilePathAsync(fileId, cancellationToken);
                if (string.IsNullOrWhiteSpace(localPath))
                {
                    return null;
                }
                // Ensure the token is not included in the URL.
                if (localPath.Contains(this.client.Options.BotToken))
                {
                    // This should be impossible because it can only happen when a local server is used and that case is already handled above.
                    throw new InvalidOperationException("The file path contains the bot token.");
                }

                // Remove the prefix from the local path.
                var tempPath = Path.GetTempPath();
                var relativePath = localPath[tempPath.Length..];
                return new Uri(new Uri(this.applicationUrl), $"/file/{relativePath}").ToString();
            }
            else
            {
                return null;
            }
        }
        else
        {
            return this.client.BuildFileDownloadLink(telegramFile);
        }
    }

    // Get the Telegram file information.
    private async Task<TelegramFile?> GetTelegramFileAsync(
        string fileId,
        CancellationToken cancellationToken
    )
    {
        TelegramFile? file = null;
        try
        {
            file = await this.cache.GetOrCreateAsync(
                $"TelegramFile:{fileId}",
                entry =>
                {
                    // It's guaranteed that the file will be available for at least 1 hour according to the Telegram Bot API documentation.
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return this.client.GetFileAsync(fileId, cancellationToken);
                }
            );

            file ??= await this.client.GetFileAsync(fileId, cancellationToken);
        }
        catch (BotRequestException exp)
        {
            if (exp.Message != "Bad Request: file is too big")
            {
                throw;
            }
        }

        return file;
    }
}
