using TelegramBot.Api.UpdateSpecifications.Abstractions;
using TelegramBot.Api.Handlers.Abstractions;
using TelegramBot.Api.Options;
using TelegramBot.Api.Domain;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBot.Api.Models;

namespace TelegramBot.Api.Handlers;

public class FallbackHandler : IUpdateHandler
{

    public void AddTransitionsToOptions(UpdateHandlerOptions config)
    {
    }

    public FallbackHandler()
    {
    }

    public Task<string> HandleAsync(UpdateContext context)
    {
        // _logger.LogInformation("Fallback handler invoked!");

        long? userId = context.Update.Message?.From?.Id;

        if (userId is not null)
        {
            // await botClient.SendTextMessageAsync(userId!, "Unknown message", ParseMode.MarkdownV2);
        }

        return Task.FromResult(GlobalStates.Any);
    }
}