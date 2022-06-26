using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.FSM.Models;

namespace TelegramBot.Api.FSM;

public class BotContextAccessor : IBotContextAccessor
{
    public BotContext Context { get; set; } = default!;
}