// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.AspNetCore.Mvc.Testing;
using SauceNAO.Domain.Services;
using Xunit.Sdk;

namespace SauceNAO.Tests;

/// <summary>
/// Tests for the TelegramFileManager class.
/// </summary>
/// <param name="applicationFactory">The application factory.</param>
public class TelegramFileManagerTests(WebApplicationFactory<Program> applicationFactory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task TestGetImagePath()
    {
        var fileId = this.GetTestFileId("Image");

        using var scope = applicationFactory.Services.CreateScope();
        var fileService = scope.ServiceProvider.GetRequiredService<ITelegramFileService>();
        var path = await fileService.GetFilePathAsync(fileId, default);
        Assert.NotNull(path);
        // Verify that the file exists
        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task TestGetImageUrl()
    {
        var fileId = this.GetTestFileId("Image");

        using var scope = applicationFactory.Services.CreateScope();
        var fileService = scope.ServiceProvider.GetRequiredService<ITelegramFileService>();
        var url = await fileService.GetFileUrlAsync(fileId, default);
        Assert.NotNull(url);
    }

    // Helper method to get a file ID from the configuration.
    private string GetTestFileId(string key)
    {
        var configuration = applicationFactory.Services.GetRequiredService<IConfiguration>();
        var fileId = configuration[$"TestFileIds:{key}"];
        if (string.IsNullOrEmpty(fileId))
        {
            throw new XunitException("A test file ID is required to run this test.");
        }

        return fileId;
    }
}
