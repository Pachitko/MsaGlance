using TelegramBot.Api.FSM.Models;

namespace TelegramBot.Api.FSM.Abstractions;

public interface IContextAccessor<TContext, TInput>
    where TContext : BaseFsmContext<TInput>
{
    TContext Context { get; set; }
}