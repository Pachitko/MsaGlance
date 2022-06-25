using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.FSM.Models;
using Telegram.Bot.Types;

namespace TelegramBot.Api.FSM;

public class BotContextAccessor : IContextAccessor<BotFsmContext, Update>
{
    public BotFsmContext Context { get; set; } = default!;
}