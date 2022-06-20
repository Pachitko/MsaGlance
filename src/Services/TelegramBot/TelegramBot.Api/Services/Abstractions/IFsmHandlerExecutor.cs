using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Api.Services.Abstractions;

public interface IFsmHandlerExecutor
{
    Task HandleAsync(Update update);
}