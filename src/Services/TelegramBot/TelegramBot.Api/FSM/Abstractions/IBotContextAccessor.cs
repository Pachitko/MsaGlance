using TelegramBot.Api.FSM.Models;
using Telegram.Bot.Types;

namespace TelegramBot.Api.FSM.Abstractions;

public interface IBotContextAccessor : IFsmContextAccessor<BotContext, Update>
{

}