using TelegramBot.Api.Services.Abstractions;
using TelegramBot.Api.Handlers.Abstractions;
using System.Collections.Generic;
using TelegramBot.Api.Handlers;
using System.Linq;
using System;

namespace TelegramBot.Api.Services;

public class UpdateHandlerFactory : IUpdateHandlerFactory
{
    private readonly IEnumerable<IUpdateHandler> _updateHandlers;

    public UpdateHandlerFactory(IEnumerable<IUpdateHandler> updateHandlers)
    {
        _updateHandlers = updateHandlers;
    }

    public IUpdateHandler GetUpdateHandler(Type? handlerType)
    {
        if (handlerType is null)
            return _updateHandlers.Single(h => h.GetType() == typeof(FallbackHandler));

        return _updateHandlers.Single(h => h.GetType() == handlerType);
    }
}