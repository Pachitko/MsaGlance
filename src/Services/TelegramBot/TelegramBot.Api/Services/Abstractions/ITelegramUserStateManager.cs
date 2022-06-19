using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Api.Services.Abstractions;

public interface ITelegramUserStateManager
{
    Task<string> GetStateAsync(User user, long chatId);
    Task SetStateAsync(User user, long chatId, string nextState);
}