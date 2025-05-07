// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.AspNetCore.Mvc.Testing;
using TUnit.Core.Interfaces;

namespace SauceNAO.Tests;

public class WebAppFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
    public Task InitializeAsync()
    {
        // You can also override certain services here to mock things out

        // Grab a reference to the server
        // This forces it to initialize.
        // By doing it within this method, it's thread safe.
        // And avoids multiple initialisations from different tests if parallelisation is switched on
        _ = this.Server;

        return Task.CompletedTask;
    }
}
