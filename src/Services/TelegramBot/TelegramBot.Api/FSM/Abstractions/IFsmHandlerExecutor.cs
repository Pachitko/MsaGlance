using TelegramBot.Api.FSM.Models;

namespace TelegramBot.Api.FSM.Abstractions;

public interface IFsmHandlerExecutor<TContext, TInput>
    where TContext : BaseFsmContext<TInput>
{
    Task HandleAsync(TInput update);
}