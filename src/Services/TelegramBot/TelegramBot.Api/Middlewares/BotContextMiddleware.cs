using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.FSM.Models;
using TelegramBot.Api.Services;
using Telegram.Bot.Types;
using Newtonsoft.Json;

namespace TelegramBot.Api.Middlewares;

public class BotContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BotContextMiddleware> _logger;
    private readonly TelegramBotWrapper _botWrapper;

    public BotContextMiddleware(
        RequestDelegate next,
        ILogger<BotContextMiddleware> logger,
        TelegramBotWrapper botWrapper)
    {
        _next = next;
        _logger = logger;
        _botWrapper = botWrapper;
    }

    public async Task Invoke(HttpContext context, IBotContextAccessor botContextAccessor)
    {
        using StreamReader sr = new(context.Request.Body);
        string bodyString = await sr.ReadToEndAsync();
        _logger.LogDebug("Body: {BodyString}", bodyString);
        Update? update = JsonConvert.DeserializeObject<Update>(bodyString);

        botContextAccessor.Context = new BotContext()
        {
            Input = update,
            BotClient = await _botWrapper.GetClientAsync()
        };

        await _next(context);
    }
}