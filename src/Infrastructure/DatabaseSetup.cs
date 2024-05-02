using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SauceNAO.Infrastructure.Data;

namespace SauceNAO.Infrastructure;

class DatabaseSetup(IServiceProvider serviceProvider) : IHostedService
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = this.serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Apply migrations
        context.Database.Migrate();

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    Task IHostedService.StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
