using TelegramBot.Api.Handlers.Abstractions;
using System;

namespace TelegramBot.Api.Services.Abstractions;

public interface IUpdateHandlerFactory
{
    IUpdateHandler GetUpdateHandler(Type? handlerType);
}