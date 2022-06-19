using TelegramBot.Api.Handlers.Abstractions;
using TelegramBot.Api.Options;
using TelegramBot.Api.Domain;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Api.Handlers;

public class FallbackHandler : IUpdateHandler
{

    public void AddTransitionsToOptions(UpdateHandlerOptions config)
    {
    }

    public FallbackHandler()
    {
    }

    public Task<string> HandleAsync(string currentState, Update update, TelegramBotClient botClient)
    {
        // _logger.LogInformation("Fallback handler invoked!");

        long? userId = update.Message?.From?.Id;

        if (userId is not null)
        {
            // await botClient.SendTextMessageAsync(userId!, "Unknown message", ParseMode.MarkdownV2);
        }

        return Task.FromResult(GlobalStates.Any);
    }
}