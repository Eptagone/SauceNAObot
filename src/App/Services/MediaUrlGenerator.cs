using System.Reflection;
using Microsoft.Extensions.Options;
using SauceNAO.App.Controllers;
using SauceNAO.Core.Configuration;
using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Models;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.Extensions;
using TelegramFile = Telegram.BotAPI.AvailableTypes.File;

namespace SauceNAO.App.Services;

sealed class MediaUrlGenerator(
    IOptions<AppConfiguration> appOptions,
    IOptions<BotConfiguration> botOptions,
    ITelegramBotClient client,
    IFrameExtractor frameExtractor,
    HttpClient httpClient,
    LinkGenerator links
) : IMediaUrlGenerator
{
    private readonly Dictionary<string, TelegramFile> files = [];

    public async Task<string> GenerateAsync(MediaTarget target, CancellationToken cancellationToken)
    {
        // Use the Telegram API download method if no custom server is set
        if (string.IsNullOrEmpty(botOptions.Value.ServerAddress))
        {
            var file = await this.GetFileAsync(
                target.Media.ThumbnailFileId ?? target.Media.FileId,
                cancellationToken
            );

            return client.BuildFileDownloadLink(file)!;
        }

        // Otherwise, use a safe URL
        return await this.GenerateSafeAsync(target, cancellationToken);
    }

    public async Task<string> GenerateSafeAsync(
        MediaTarget target,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrEmpty(appOptions.Value.ApplicationUrl))
        {
            throw new MissingApplicationUrlException(target.Message);
        }

        var applicationUrl = new Uri(appOptions.Value.ApplicationUrl);
        var targetFileId = target.Media.ThumbnailFileId ?? target.Media.FileId;
        var file = await this.GetFileAsync(targetFileId, cancellationToken);

        var tempFolderPath = Path.Join(
            Path.GetTempPath(),
            Assembly.GetExecutingAssembly().GetName().Name
        );
        string? safeUrl = null;

        // If the file is a video, download the first frame
        if (
            target.Media.MimeType?.StartsWith("video/") is true
            && targetFileId == target.Media.FileId
        )
        {
            var fileName = $"{target.Media.FileUniqueId}.jpg";
            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }
            var outputPath = Path.Join(tempFolderPath, $"{target.Media.FileUniqueId}.jpg");
            if (!File.Exists(outputPath))
            {
                var ext = Path.GetExtension(file.FilePath);
                var inputFileName = Path.Join(tempFolderPath, $"{target.Media.FileId}{ext}");
                if (!File.Exists(inputFileName))
                {
                    var downloadUrl = client.BuildFileDownloadLink(file)!;
                    Console.WriteLine(
                        "Download URL: {0}. FilePath: {1}",
                        downloadUrl,
                        file.FilePath
                    );
                    try
                    {
                        var stream = await httpClient.GetStreamAsync(
                            downloadUrl,
                            cancellationToken
                        );
                        using var fileStream = File.Create(inputFileName);
                        await stream.CopyToAsync(fileStream, cancellationToken);
                    }
                    catch (Exception exp)
                    {
                        throw new DownloadFailedException(target.Message, downloadUrl, exp);
                    }
                }

                await frameExtractor.ExtractAsync(inputFileName, outputPath, cancellationToken);
            }
            safeUrl = links.GetUriByAction(
                nameof(TemporalFileController.GetTemporalFile),
                nameof(TemporalFileController).Replace("Controller", string.Empty),
                new { fileName },
                applicationUrl.Scheme,
                new HostString(applicationUrl.Host, applicationUrl.Port),
                new PathString(applicationUrl.PathAndQuery),
                new FragmentString(applicationUrl.Fragment)
            );
        }
        // If not custom api server is set, download the file to the temp folder
        else if (string.IsNullOrEmpty(botOptions.Value.ServerAddress))
        {
            var fileExt = Path.GetExtension(file.FilePath);
            var fileName = $"{target.Media.FileUniqueId}{fileExt}";

            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }
            var output = Path.Join(tempFolderPath, fileName);
            if (!File.Exists(output))
            {
                var downloadUrl = client.BuildFileDownloadLink(file)!;
                try
                {
                    var stream = await httpClient.GetStreamAsync(downloadUrl, cancellationToken);
                    using var fileStream = File.Create(output);
                    await stream.CopyToAsync(fileStream, cancellationToken);
                }
                catch (Exception exp)
                {
                    throw new DownloadFailedException(target.Message, downloadUrl, exp);
                }
            }
            safeUrl = links.GetUriByAction(
                nameof(TemporalFileController.GetTemporalFile),
                nameof(TemporalFileController).Replace("Controller", string.Empty),
                new { fileName },
                applicationUrl.Scheme,
                new HostString(applicationUrl.Host, applicationUrl.Port),
                new PathString(applicationUrl.PathAndQuery),
                new FragmentString(applicationUrl.Fragment)
            );
        }
        else
        {
            safeUrl = links.GetUriByAction(
                nameof(TemporalFileController.GetFile),
                nameof(TemporalFileController).Replace("Controller", string.Empty),
                new { fileName = file.FilePath },
                applicationUrl.Scheme,
                new HostString(applicationUrl.Host, applicationUrl.Port),
                new PathString(applicationUrl.PathAndQuery),
                new FragmentString(applicationUrl.Fragment)
            );
        }

        safeUrl = safeUrl?.Replace("%2F", Path.AltDirectorySeparatorChar.ToString());
        return safeUrl ?? throw new InvalidOperationException("Failed to generate safe media url.");
    }

    private async Task<TelegramFile> GetFileAsync(
        string fileId,
        CancellationToken cancellationToken
    )
    {
        if (this.files.TryGetValue(fileId, out TelegramFile? value))
        {
            return value;
        }

        value = await client.GetFileAsync(fileId, cancellationToken);
        this.files[fileId] = value;
        return value;
    }
}
