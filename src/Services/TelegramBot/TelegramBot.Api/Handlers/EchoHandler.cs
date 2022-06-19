using TelegramBot.Api.UpdateSpecifications;
using TelegramBot.Api.Extensions;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Options;
using TelegramBot.Api.Domain;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBot.Api.Handlers.Abstractions;

namespace TelegramBot.Api.Handlers;

public class EchoHandler : IUpdateHandler
{
    public void AddTransitionsToOptions(UpdateHandlerOptions config)
    {
        config.From(GlobalStates.Any).With<NotCommandUpdateSpecification>().To<EchoHandler>();
    }

    public async Task<string> HandleAsync(string currentState, Update update, TelegramBotClient botClient)
    {
        string? messageText = update.Message?.Text;
        long? userId = update.Message?.From?.Id;

        await botClient.SendTextMessageAsync(userId!, messageText!, ParseMode.MarkdownV2);

        return GlobalStates.Any;
    }
}