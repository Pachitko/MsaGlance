using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Api.Commands;

public interface IBotCommandHandler
{
    Task<UserState> HandleAsync(UserState currentState, Update update, TelegramBotClient botClient);
}