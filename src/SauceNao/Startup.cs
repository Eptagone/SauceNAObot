// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SauceNao.Data;
using SauceNao.Services;

namespace SauceNao
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<SauceNaoContext>(options => options.UseSqlite(Configuration.GetConnectionString("LiteDB")));

            services.AddTransient<SauceNaoBot>();
            services.AddTransient<SauceNaoService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<SauceNaoContext>();
                // context.Database.EnsureDeleted();
                context.Database.Migrate();
            }

            SauceNaoBot.InitialSetup(
                Configuration["SauceNao:BotToken"],
                Configuration["SauceNao:ApiKey"],
                Configuration["SauceNao:WebhookUrl"],
                Configuration["SauceNao:SecretToken"]);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "SauceNaoWebhook",
                    pattern: $"{Configuration["SauceNao:SecretToken"]}/{{action = Index}}/{{id?}}",
                    defaults: new { controller = "SauceBot" });
                endpoints.MapControllers();
            });
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
