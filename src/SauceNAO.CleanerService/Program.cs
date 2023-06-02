// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SauceNAO.CleanerService;
using SauceNAO.Infrastructure;

IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices((context, services) =>
	{
		var connectionString = context.Configuration.GetConnectionString("Default");

		services.AddDbContext<SauceNaoContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
		services.AddHostedService<Worker>();
	})
	.Build();

await host.RunAsync();
