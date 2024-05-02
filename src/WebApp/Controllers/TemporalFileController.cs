// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using SauceNAO.Domain;

namespace SauceNAO.WebApp.Controllers;

public class TemporalFileController(
    IOptions<GeneralOptions> options,
    IOptions<TelegramBotOptions> botOptions
) : ControllerBase
{
    private readonly string botToken = botOptions.Value.BotToken;
    private readonly string? filesPath = options.Value.FilesPath;

    [HttpGet("file/{fileName}")]
    public IActionResult GetFile(string fileName)
    {
        var tempPath = Path.GetTempPath();
        var filePath = Path.Join(tempPath, fileName);

        if (!Path.Exists(filePath)) {
            return this.NotFound();
        }        

        // Try to get the content type of the file.
        new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var contentType);

        return this.PhysicalFile(filePath, contentType ?? "application/octet-stream");
    }

    [HttpGet("file/{folderName}/{fileName}")]
    public IActionResult GetFile(string folderName, string fileName)
    {
        // If no local server path is provided, return a 404.
        if (string.IsNullOrWhiteSpace(this.filesPath))
        {
            return this.NotFound();
        }

        // Make sure the file path contains one of the allowed folders.
        var allowedFolders = new string[]
        {
            "thumbnails",
            "profile_photos",
            "photos",
            "voice",
            "videos",
            "documents",
            "stickers",
            "music",
            "animations",
            "secret_thumbnails",
            "video_notes",
            "passport",
            "wallpapers",
            "notification_sounds",
            "stories"
        };

        if (!allowedFolders.Contains(folderName))
        {
            return this.NotFound();
        }

        var filesPath = this.filesPath.Replace(this.botToken, "{BotToken}")
            .Replace("C:\\", "{WindowsDrive}");
        var appFilesPath = Path.TrimEndingDirectorySeparator(filesPath.Split(":").First())
            .Replace("{BotToken}", this.botToken)
            .Replace("{WindowsDrive}", "C:\\");
        string filePath = Path.Combine(appFilesPath, folderName, fileName);

        if (!Path.Exists(filePath))
        {
            return this.NotFound();
        }
        // Try to get the content type of the file.
        new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var contentType);

        return this.PhysicalFile(filePath, contentType ?? "application/octet-stream");
    }
}
