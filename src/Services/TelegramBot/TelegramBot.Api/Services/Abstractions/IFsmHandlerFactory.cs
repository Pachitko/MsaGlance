using TelegramBot.Api.Handlers.Abstractions;
using System;

namespace TelegramBot.Api.Services.Abstractions;

public interface IFsmHandlerFactory
{
    IFsmHandler GetFsmHandler(Type? handlerType);
}