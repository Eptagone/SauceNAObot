using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using NeoSmart.Caching.Sqlite;
using SauceNAO.App.Resources;
using SauceNAO.App.Services;
using SauceNAO.Core.Configuration;
using SauceNAO.Core.Exceptions;
using SauceNAO.Core.Services;
using SauceNAO.Infrastructure;
using Telegram.BotAPI;

namespace SauceNAO.App;

static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEssentialServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Add caching services.
        var redisConnectionString = configuration.GetConnectionString("Cache");
        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddSqliteCache(Path.Combine(Path.GetTempPath(), "snao-cache.db"));
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "snao";
            });
        }

        // Add localization services.
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.TryAddSingleton<IBetterStringLocalizerFactory, BetterStringLocalizerFactory>();
        services.TryAddTransient(typeof(IBetterStringLocalizer<>), typeof(BetterStringLocalizer<>));

        // Add the infrastructure layer
        services.AddInfrastructureServices(configuration);

        // Add controllers
        services.AddControllers();

        return services;
    }

    public static IServiceCollection AddSauceNaoServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Bind the options from the configuration
        services.Configure<AppConfiguration>(options =>
            configuration.GetSection(AppConfiguration.SectionName).Bind(options)
        );
        services.Configure<BotConfiguration>(options =>
            configuration.GetSection(BotConfiguration.SectionName).Bind(options)
        );
        services.Configure<FilesConfiguration>(options =>
            configuration.GetSection(FilesConfiguration.SectionName).Bind(options)
        );

        // Configure the Telegram bot client
        services
            .AddHttpClient(
                nameof(TelegramBotClient),
                (provider, client) =>
                {
                    var options = provider.GetRequiredService<IOptions<BotConfiguration>>().Value;
                    client.BaseAddress = new Uri(
                        string.IsNullOrEmpty(options.ServerAddress)
                            ? TelegramBotClientOptions.BaseServerAddress
                            : options.ServerAddress
                    );
                }
            )
            .RemoveAllLoggers();
        services.AddScoped<ITelegramBotClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var botConfig = provider.GetRequiredService<IOptions<BotConfiguration>>().Value;
            var options = new TelegramBotClientOptions(
                botConfig.BotToken,
                httpClientFactory.CreateClient(nameof(TelegramBotClient))
            );
            if (!string.IsNullOrWhiteSpace(botConfig.ServerAddress))
            {
                options.ServerAddress = botConfig.ServerAddress;
            }
            return new TelegramBotClient(options);
        });

        // Register core services
        services.AddSingleton<IFrameExtractor, FrameExtractor>();
        services.AddScoped<IStateManager, StateManager>();
        services.AddScoped<IContextProvider, ContextProvider>();
        services.AddScoped<IMediaExtractor, MediaExtractor>();
        services.AddScoped<IMediaUrlGenerator, MediaUrlGenerator>();
        services.AddScoped<ISauceHandler, SauceHandler>();

        // Register all required handlers from current assembly
        services.AddInternalHandlers<IInlineQueryHandler>();
        services.AddInternalHandlers<IMessageExceptionHandler>();
        services.AddInternalHandlers<IUserStateHandler>();
        services.AddInternalHandlers<ICommandHandler>();
        services.AddInternalHandlers<IMessageHandler>();
        services.AddScoped<IUpdateHandler, UpdateHandler>();

        // Register the update handler pool
        services.AddSingleton<IUpdateHandlerPool, UpdateHandlerPool>();
        services.AddHostedService(provider =>
            (UpdateHandlerPool)provider.GetRequiredService<IUpdateHandlerPool>()
        );

        // Registers workers
        services.AddHostedService<SetupWorker>();
        services.AddHostedService<LongPollingWorker>();

        return services;
    }

    private static IServiceCollection AddInternalHandlers<T>(this IServiceCollection services)
    {
        // Register all command handlers from current assembly
        var commandHandlerTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(T)))
            .OrderBy(t =>
            {
                var attr = t.GetCustomAttribute<PriorityAttribute>();
                return attr is null ? 0 : attr.Priority;
            });
        foreach (var type in commandHandlerTypes)
        {
            services.AddScoped(typeof(T), type);
        }

        return services;
    }
}
