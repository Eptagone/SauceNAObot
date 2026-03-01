// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using SauceNAO.Core.Configuration;

namespace SauceNAO.App.Controllers;

[ApiController]
public sealed class TemporalFileController(
    IHttpClientFactory httpClientFactory,
    IOptions<BotConfiguration> options
) : ControllerBase
{
    [HttpGet("temp/{fileName}")]
    [ResponseCache(Duration = 3600)]
    public IActionResult GetTemporalFile(string fileName)
    {
        var tempFolderPath = Path.Join(
            Path.GetTempPath(),
            Assembly.GetExecutingAssembly().GetName().Name
        );
        if (!Directory.Exists(tempFolderPath))
        {
            return this.NotFound("No temporal files found.");
        }

        var filePath = Path.Join(tempFolderPath, fileName);
        if (!Path.Exists(filePath))
        {
            return this.NotFound("No temporal file found.");
        }

        // Try to get the content type of the file.
        new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var contentType);
        return this.PhysicalFile(filePath, contentType ?? "application/octet-stream");
    }

    [HttpGet("file/{*fileName}")]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetFile(string fileName)
    {
        if (string.IsNullOrEmpty(options.Value.ServerAddress))
        {
            return this.NotFound("File not found.");
        }

        var downloadUrl = new Uri(
            new Uri(options.Value.ServerAddress),
            $"/file/bot{options.Value.BotToken}/{fileName}"
        );
        using var httpClient = httpClientFactory.CreateClient();

        try
        {
            var stream = await httpClient.GetStreamAsync(downloadUrl);
            // Try to get the content type of the file.
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType);
            return this.File(stream, contentType ?? "application/octet-stream");
        }
        catch (HttpRequestException exp)
        {
            if (exp.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound("File not found.");
            }

            throw;
        }
    }
}
