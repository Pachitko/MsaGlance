using TelegramBot.Api.Handlers.Abstractions;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Options;
using TelegramBot.Api.Domain;
using System.Threading.Tasks;
using TelegramBot.Api.Models;
using Telegram.Bot;

namespace TelegramBot.Api.Handlers;

public class FallbackHandler : IFsmHandler
{
    private readonly ILogger<FallbackHandler> _logger;

    public void AddTransitionsToFsmOptions(FsmOptions config)
    {
    }

    public FallbackHandler(ILogger<FallbackHandler> logger)
    {
        _logger = logger;
    }

    public async Task<string> HandleAsync(UpdateContext context)
    {
        _logger.LogInformation("Fallback handler invoked!");

        long? userId = context.Update.Message?.From?.Id;

        if (userId is not null)
        {
            await context.BotClient.SendTextMessageAsync(userId!, "Unknown message", ParseMode.MarkdownV2);
        }

        return GlobalStates.Any;
    }
}