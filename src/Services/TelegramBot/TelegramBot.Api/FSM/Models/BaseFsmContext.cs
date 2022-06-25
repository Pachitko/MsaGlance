namespace TelegramBot.Api.FSM.Models;

public abstract class BaseFsmContext<TInput>
{
    public TInput Input { get; set; } = default!;
}