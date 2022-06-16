using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Serilog;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using System;
using Telegram.Bot.Types.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TelegramBot.Api.Options;
using TelegramBot.Api;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Override(typeof(TelegramBotWrapper).Namespace, LogEventLevel.Debug)
    .CreateLogger();

builder.Host.UseSerilog();

// todo: Extract to a separate extension method
builder.Services.AddOptions<TelegramBotOptions>().Configure<IConfiguration>((o, c) =>
{
    o.BotToken = c.GetValue<string>("BOT_TOKEN");
    o.WebHookEndpoint = c.GetValue<string>("WEB_HOOK_ENDPOINT");
});

builder.Services.AddSingleton<TelegramBotWrapper>();
builder.Services.AddHostedService<TelegramBotInitializationHostedService>();

var app = builder.Build();

app.MapGet("/", () => $"TelegramBot {DateTime.UtcNow}");

app.MapPost("/update", Update);

await app.RunAsync();

static async Task<IResult> Update([FromBody] object updateDto, TelegramBotWrapper bot)
{
    TelegramBotClient botClient = await bot.GetClientAsync();
    Update update = JsonConvert.DeserializeObject<Update>(updateDto.ToString());
    if (update.Type == UpdateType.Message)
    {
        if (update.Message!.Type == MessageType.Text)
        {
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, update.Message.Text!, ParseMode.Markdown);
        }
    }

    return Results.Ok(botClient.BotId);
}