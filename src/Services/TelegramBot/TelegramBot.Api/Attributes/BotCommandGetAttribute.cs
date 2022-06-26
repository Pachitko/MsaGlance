using Microsoft.AspNetCore.Mvc;

namespace TelegramBot.Api.Attribute;

public class BotCommandGetAttribute : HttpGetAttribute
{
    public BotCommandGetAttribute(string fromState, string commandName, string? remainingPart = null)
        : base($"/{fromState}/{commandName}/{remainingPart}")
    {

    }
}