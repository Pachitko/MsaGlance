using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.FSM.Attributes;
using TelegramBot.Api.FSM.Models;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Domain;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Api.FSM.Handlers;

public class RootHandler : BaseFsmHandler<BotFsmContext, Update>
{
    public RootHandler(IContextAccessor<BotFsmContext, Update> updateContextAccessor)
        : base(updateContextAccessor)
    {
    }

    [NoSlashCommand(GlobalStates.Any)]
    public string Echo()
    {
        Context.BotClient.SendTextMessageAsync(Context.UserId, Context.SafeTextPayload!, ParseMode.MarkdownV2);
        return "any";
    }
}