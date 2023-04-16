using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Logging
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//DI
builder.Services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());

//Ocelot config
builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);


//BUILD WEB APPLICATION
var app = builder.Build();

//Configure application

await app.UseOcelot();


app.Run();
