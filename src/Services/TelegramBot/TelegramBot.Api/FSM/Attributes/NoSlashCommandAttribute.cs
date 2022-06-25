using Telegram.Bot.Types;

namespace TelegramBot.Api.FSM.Attributes;

public class NoSlashCommand : UpdateValidatableAttribute
{
    public NoSlashCommand(string fromState)
        : base(fromState)
    {
    }

    protected override bool IsValidForUpdate(Update update)
    {
        return update.Message?.Text is not null && !update.Message.Text.StartsWith("/");
    }
}