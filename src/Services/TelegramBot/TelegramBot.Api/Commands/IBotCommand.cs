using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Api.Commands;

public interface IBotCommand
{
    BotCommandType CommandType { get; }
    Task ExecuteAsync(Update update, TelegramBotClient botClient);
}