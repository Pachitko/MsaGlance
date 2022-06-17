using System;

namespace TelegramBot.Api.Exceptions;

public class BotCommandDoesNotExistException : Exception
{
    public BotCommandDoesNotExistException(string? message) : base(message)
    {
    }
}