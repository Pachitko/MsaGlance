using Telegram.Bot.Types;

namespace TelegramBot.Api.FSM.Attributes;

public class WithDocumentAttribute : UpdateValidatableAttribute
{
    public WithDocumentAttribute(string fromState) : base(fromState)
    {
    }

    protected override bool IsValidForUpdate(Update update)
        => update.Message?.Document != null;
}