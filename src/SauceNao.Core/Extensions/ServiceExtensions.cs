// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace SauceNAO.Core.Extensions;

public static class ServiceExtensions
{
	public static IServiceCollection AddSauceBot<TSauceDatabase>(this IServiceCollection services)
		where TSauceDatabase : ISauceDatabase
	{
		// Register bot properties
		services.AddSingleton<SnaoBotProperties>();
		services.AddScoped(typeof(ISauceDatabase), typeof(TSauceDatabase));
		services.AddScoped<SauceNaoBot>();

		return services;
	}
}
