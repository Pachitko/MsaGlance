using TelegramBot.Api.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using TelegramBot.Api.Exceptions;
using TelegramBot.Api.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelegramBot.Api.Services;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog;
using System;

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

static async Task<IResult> Update([FromBody] object updateDto, IHandlerExecutor commandExecutor, ILogger<Program> logger)
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