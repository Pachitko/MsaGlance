using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Api.Services;

public interface ICommandExecutor
{
    Task ExecuteAsync(Update update);
}