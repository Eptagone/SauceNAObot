// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Data;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SauceNAO.Application.Commands;
using SauceNAO.Application.Services;
using SauceNAO.Domain.Services;

namespace SauceNAO.Application;

/// <summary>
/// Defines extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the SauceNAO bot core services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddSauceNAOCore(this IServiceCollection services)
    {
        // Register the frame extractor service.
        services.AddSingleton<IFrameExtractor, FrameExtractor>();

        // Register the user state manager.
        services.AddScoped<IUserStateManager, UserStateManager>();

        // Register all commands as services.
        var commandTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(ITelegramBotCommand))
            );
        foreach (var commandType in commandTypes)
        {
            var cmd = new SauceNAOCommand(commandType);
            CommandDirectory.AddCommand(cmd);
            services.AddKeyedScoped(typeof(ITelegramBotCommand), cmd.Command, commandType);
            if (commandType.IsAssignableTo(typeof(IUserStateHandler)))
            {
                services.AddKeyedScoped(typeof(IUserStateHandler), cmd.Command, commandType);
            }
        }

        // Register the bot services.
        services.AddScoped<ISauceNaoContextFactory, SauceNaoContextFactory>();
        services.AddScoped<ISauceNaoBot, SauceNaoBot>();
        services.AddSingleton<IUpdateReceiver, UpdateReceiver>();

        // Register the update receiver as a hosted service.
        services.AddHostedService(provider =>
            (UpdateReceiver)provider.GetRequiredService<IUpdateReceiver>()
        );

        // Register the setup service.
        services.AddHostedService<SauceNaoBotSetup>();

        return services;
    }
}
