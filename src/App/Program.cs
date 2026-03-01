// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.App;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEssentialServices(builder.Configuration);
builder.Services.AddSauceNaoServices(builder.Configuration);

var app = builder.Build();

app.MapControllers();
app.MapGet("/", () => "Hello World!");
app.Run();
