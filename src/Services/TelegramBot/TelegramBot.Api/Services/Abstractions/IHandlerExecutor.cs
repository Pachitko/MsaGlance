using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Api.Services.Abstractions;

public interface IHandlerExecutor
{
    Task HandleAsync(Update update);
}