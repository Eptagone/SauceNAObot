// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Domain.Services;
using TUnit.Core.Exceptions;

namespace SauceNAO.Tests;

/// <summary>
/// Tests for the TelegramFileManager class.
/// </summary>
public class TelegramFileManagerTests()
{
    [ClassDataSource<WebAppFactory>(Shared = SharedType.PerTestSession)]
    public required WebAppFactory WebAppFactory { get; init; }

    [Test]
    public async Task TestGetImagePath()
    {
        var fileId = this.GetTestFileId("Image");

        using var scope = this.WebAppFactory.Services.CreateScope();
        var fileService = scope.ServiceProvider.GetRequiredService<ITelegramFileService>();
        var path = await fileService.GetFilePathAsync(fileId, default);
        await Assert.That(path).IsNotNull();
        // Verify that the file exists
        await Assert.That(File.Exists(path)).IsTrue();
    }

    [Test]
    public async Task TestGetImageUrl()
    {
        var fileId = this.GetTestFileId("Image");

        using var scope = this.WebAppFactory.Services.CreateScope();
        var fileService = scope.ServiceProvider.GetRequiredService<ITelegramFileService>();
        var url = await fileService.GetFileUrlAsync(fileId, default);
        await Assert.That(url).IsNotNull();
    }

    // Helper method to get a file ID from the configuration.
    private string GetTestFileId(string key)
    {
        var configuration = this
            .WebAppFactory.Services.CreateScope()
            .ServiceProvider.GetRequiredService<IConfiguration>();
        var fileId = configuration[$"TestFileIds:{key}"];
        if (string.IsNullOrEmpty(fileId))
        {
            throw new TUnitException("A test file ID is required to run this test.");
        }

        return fileId;
    }
}
