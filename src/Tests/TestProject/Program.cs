// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add infrastructure services to the container.
builder.Services.AddSnaoBotServices(builder.Configuration);
builder.Services.AddSnaoUtilityServices();

var app = builder.Build();

app.Run();

public partial class Program { }
