using SauceNAO.App;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEssentialServices(builder.Configuration);
builder.Services.AddSauceNaoServices(builder.Configuration);

var app = builder.Build();

app.MapControllers();
app.MapGet("/", () => "Hello World!");
app.Run();
