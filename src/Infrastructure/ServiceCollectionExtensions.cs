// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SauceNAO.Core.Data;
using SauceNAO.Core.Services;
using SauceNAO.Infrastructure.Data;
using SauceNAO.Infrastructure.Data.Repositories;
using SauceNAO.Infrastructure.Services;

namespace SauceNAO.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Configure the database
        services.AddDbContext<SnaoDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("Default"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSnakeCaseNamingConvention()
        );

        // Configure the SauceNAO service
        services.AddHttpClient<ISauceNAOClient, SauceNAOClient>(
            (client) => client.BaseAddress = new Uri(SauceNAOClient.SAUCENAO_ENDPOINT)
        );

        // Add repositories
        services.AddScoped<IApiKeyRespository, ApiKeyRepository>();
        services.AddScoped<IChatRepostory, ChatRepository>();
        services.AddScoped<IMediaFileRepository, MediaFileRepository>();
        services.AddScoped<ISearchHistoryRepository, SearchHistoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
