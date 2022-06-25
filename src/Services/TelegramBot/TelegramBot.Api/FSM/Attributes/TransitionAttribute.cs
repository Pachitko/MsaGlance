namespace TelegramBot.Api.FSM.Attributes;

// <TInput> Generic attributes are unsupported
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public abstract class TransitionAttribute : Attribute
{
    public string FromState { get; }

    public TransitionAttribute(string fromState)
    {
        FromState = fromState;
    }

    protected bool IsValidForState(string state)
        => FromState.Equals(state, StringComparison.OrdinalIgnoreCase);
}