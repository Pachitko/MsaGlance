using TelegramBot.Api.Middlewares;
using TelegramBot.Api.Extensions;
using TelegramBot.Api.Services;
using Serilog.Events;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Override(typeof(TelegramBotWrapper).Namespace, LogEventLevel.Debug)
    .MinimumLevel.Override(typeof(BotContextMiddleware).Namespace, LogEventLevel.Debug)
    .CreateLogger();

builder.Host.UseSerilog();

services.AddHttpClient();
services.AddTelegramBot();

var app = builder.Build();

app.UseExceptionHandler(CustomExceptionHandler.HandleException);

app.UseMiddleware<BotContextMiddleware>();
app.UseMiddleware<BotRouteRewriteMiddleware>();
app.UseRouting(); // replace default UseRouting()

app.MapGet("/", () => $"TelegramBot {DateTime.UtcNow}");

app.UseEndpoints(builder => builder.MapControllers());

app.Run(async (ctx) =>
{
    ctx.RequestServices.GetRequiredService<ILoggerFactory>()
        .CreateLogger("NoEndpoint")
        .LogWarning("Bot endpoint not found for path: \"{Path}\"", ctx.Request.Path);

    await Results.Ok().ExecuteAsync(ctx);
});

await app.RunAsync();