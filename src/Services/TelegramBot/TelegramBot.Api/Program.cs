using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.Exceptions;
using TelegramBot.Api.Extensions;
using TelegramBot.Api.FSM.Models;
using Microsoft.AspNetCore.Mvc;
using TelegramBot.Api.Services;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Override(typeof(TelegramBotWrapper).Namespace, LogEventLevel.Debug)
    .CreateLogger();

builder.Host.UseSerilog();

services.AddHttpClient();
builder.Services.AddTelegramBot();

var app = builder.Build();

app.MapGet("/", () => $"TelegramBot {DateTime.UtcNow}");

app.MapPost("/update", Update);

await app.RunAsync();

static async Task<IResult> Update([FromBody] object updateDto, IFsmHandlerExecutor<BotFsmContext, Update> commandExecutor, ILogger<Program> logger)
{
    try
    {
        Update update = JsonConvert.DeserializeObject<Update>(updateDto.ToString());
        await commandExecutor.HandleAsync(update);
    }
    catch (BotCommandDoesNotExistException e)
    {
        logger.LogError(e, "Bot command does not exist");
    }
    catch (Exception e)
    {
        logger.LogError(e, "Error has occured: ");
    }

    return Results.Ok();
}