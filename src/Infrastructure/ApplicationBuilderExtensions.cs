using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SauceNAO.Infrastructure.Data;

namespace SauceNAO.Infrastructure;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds all the SauceNAO infrastructure services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder ApplyDatabaseMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Apply migrations if needed
        if (context.Database.HasPendingModelChanges())
        {
            context.Database.Migrate();
        }

        return app;
    }
}
