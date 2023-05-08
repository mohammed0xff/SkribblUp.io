using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SignalRChat.Hubs;
using DrawAndGuess.CustomLogging;
using DrawAndGuess;
using DrawAndGuess.CommandProcessor;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddProvider(new GameLoggerProvider());

builder.Services.AddSignalR();
builder.Services.AddSingleton<Game>();
builder.Services.AddSingleton<ICommandProcessor, CommandProcessor>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .WithOrigins("http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCors();
app.MapHub<GameHub>("/game");

await app.RunAsync();