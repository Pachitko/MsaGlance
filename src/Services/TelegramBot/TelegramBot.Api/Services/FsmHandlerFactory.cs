using TelegramBot.Api.Services.Abstractions;
using TelegramBot.Api.Handlers.Abstractions;
using System.Collections.Generic;
using TelegramBot.Api.Handlers;
using System.Linq;
using System;

namespace TelegramBot.Api.Services;

public class FsmHandlerFactory : IFsmHandlerFactory
{
    private readonly IEnumerable<IFsmHandler> _updateHandlers;

    public FsmHandlerFactory(IEnumerable<IFsmHandler> updateHandlers)
    {
        _updateHandlers = updateHandlers;
    }

    public IFsmHandler GetFsmHandler(Type? handlerType)
    {
        if (handlerType is null)
            return _updateHandlers.Single(h => h.GetType() == typeof(FallbackHandler));

        return _updateHandlers.Single(h => h.GetType() == handlerType);
    }
}