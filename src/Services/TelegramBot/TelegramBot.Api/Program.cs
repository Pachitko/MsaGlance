using Microsoft.AspNetCore.Builder;
using TelegramBot.Api.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelegramBot.Api.Services;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using TelegramBot.Api;
using Serilog.Events;
using Serilog;
using System;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Override(typeof(TelegramBotWrapper).Namespace, LogEventLevel.Debug)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddTelegramBot();

var app = builder.Build();

app.MapGet("/", () => $"TelegramBot {DateTime.UtcNow}");

app.MapPost("/update", Update);

await app.RunAsync();

static async Task<IResult> Update([FromBody] object updateDto, ICommandExecutor commandExecutor, ILogger<Program> logger)
{
    try
    {
        Update update = JsonConvert.DeserializeObject<Update>(updateDto.ToString());
        await commandExecutor.ExecuteAsync(update);
    }
    catch (InvalidOperationException e)
    {
        logger.LogError(e, "Command does not exist");
    }

    return Results.Ok();
}