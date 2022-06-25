using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.FSM.Models;

namespace TelegramBot.Api.FSM.Handlers;

public abstract class BaseFsmHandler<TContext, TInput>
    where TContext : BaseFsmContext<TInput>
{
    public TContext Context { get; set; }
    public BaseFsmHandler(IContextAccessor<TContext, TInput> updateContextAccessor)
    {
        Context = updateContextAccessor.Context;
    }
}