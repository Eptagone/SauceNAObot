// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SauceNAO.Infrastructure.Data;

namespace SauceNAO.Infrastructure;

class DatabaseSetup(IServiceProvider serviceProvider) : IHostedService
{
    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Apply migrations
        context.Database.Migrate();

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    Task IHostedService.StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
