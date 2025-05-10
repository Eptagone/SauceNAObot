// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SauceNAO.Domain;
using SauceNAO.Domain.Repositories;
using SauceNAO.Domain.Services;
using SauceNAO.Infrastructure.Data;
using SauceNAO.Infrastructure.Data.Repositories;
using SauceNAO.Infrastructure.Services;
using Telegram.BotAPI;

namespace SauceNAO.Infrastructure;

/// <summary>
/// Defines extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all the SauceNAO infrastructure services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddSauceNaoInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddSnaoDataServices(configuration)
            .AddSnaoBotServices(configuration)
            .AddSnaoUtilityServices();
    }

    private static IServiceCollection AddSnaoDataServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Register the application database context and repositories.
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Default"));
        });
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IApiKeyRespository, ApiKeyRepository>();
        services.AddScoped<ISauceMediaRepository, SauceMediaRepository>();

        return services;
    }

    /// <summary>
    /// Adds the Telegram bot services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>
    /// This methods is already called by the <see cref="AddSauceNaoInfrastructure"/> method. This method is used for testing purposes.
    /// </remarks>
    /// <param name="services">The services collection.</param>
    /// <param name="configuration">Configuration.</param>
    /// <returns>The services collection.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddSnaoBotServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Configuration binding.
        services.Configure<TelegramBotOptions>(
            configuration.GetSection(TelegramBotOptions.SectionName)
        );
        services.Configure<GeneralOptions>(configuration.GetSection(GeneralOptions.SectionName));

        // Add HTTP client services.
        services.AddHttpClient();
        // Configure and register the Telegram bot client using Configuration.
        services.AddSingleton<ITelegramBotClient>(provider =>
        {
            var botOptions = provider.GetRequiredService<IOptions<TelegramBotOptions>>();
            var options = new TelegramBotClientOptions(botOptions.Value.BotToken);
            // Read the address to a local bot api server from the configuration.
            var serverAddress = botOptions.Value.ServerAddress;
            if (!string.IsNullOrWhiteSpace(serverAddress))
            {
                options.ServerAddress = serverAddress;
            }
            return new TelegramBotClient(options);
        });

        // Configure HttpClient for the file service.
        services.AddHttpClient(nameof(TelegramFileService));

        // Add caching services.
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "snao";
            });
        }
        // Register the Telegram file service.
        services.AddSingleton<ITelegramFileService, TelegramFileService>();

        return services;
    }

    /// <summary>
    /// Adds the utility services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>
    /// This methods is already called by the <see cref="AddSauceNaoInfrastructure"/> method. This method is used for testing purposes.
    /// </remarks>
    /// <param name="services">The services collection.</param>
    /// <returns>The services collection.</returns>
    public static IServiceCollection AddSnaoUtilityServices(this IServiceCollection services)
    {
        // Register the SauceNAO service.
        services.AddScoped<ISauceNaoService, SauceNaoService>();

        return services;
    }
}
