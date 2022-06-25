using Telegram.Bot.Types;

namespace TelegramBot.Api.FSM.Attributes;

public abstract class UpdateValidatableAttribute : TransitionAttribute
{
    protected UpdateValidatableAttribute(string fromState) : base(fromState)
    {
    }

    public bool IsValidFor(string state, Update update)
    {
        return IsValidForState(state) && IsValidForUpdate(update);
    }

    protected abstract bool IsValidForUpdate(Update update);
}