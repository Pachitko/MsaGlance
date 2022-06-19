using TelegramBot.Api.Options;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Api.Handlers.Abstractions;

public interface IUpdateHandler
{
    void AddTransitionsToOptions(UpdateHandlerOptions config);
    Task<string> HandleAsync(string currentState, Update update, TelegramBotClient botClient);
}