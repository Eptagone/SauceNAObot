// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SauceNAO.Core.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSauceBot<TSauceDatabase>(this IServiceCollection services, IConfiguration configuration)
            where TSauceDatabase : ISauceDatabase
        {
            var telegram = configuration.GetSection("Telegram");
            var snao = configuration.GetSection("SauceNAO");

            var botToken = telegram["BotToken"];
            var apikey = snao["ApiKey"];
            var supportChatLink = telegram["SupportChatLink"];

            var applicationUrl = configuration["Application Url"];

            SnaoBotProperties properties;

            if (string.IsNullOrEmpty(applicationUrl))
            {
                properties = new SnaoBotProperties(botToken, apikey, supportChatLink);
                properties.Initialize();
            }
            else
            {
                var accessToken = telegram["AccessToken"];
                var certificate = configuration["Certificate"];

                var ffmpegExec = configuration["FFmpegExec"];

                var webhook = string.Format("{0}/bot/{1}", applicationUrl, accessToken);
                var filesUrl = string.Format("{0}/temp/{{0}}", applicationUrl);

                properties = new SnaoBotProperties(botToken, apikey, filesUrl, ffmpegExec, supportChatLink);
                properties.Initialize(webhook, certificate);
            }

            properties.SetBotCommands();

            // Register bot properties
            services.AddSingleton(properties);

            services.AddScoped(typeof(ISauceDatabase), typeof(TSauceDatabase));

            services.AddScoped<SauceNaoBot>();

            return services;
        }
    }
}
