using Microsoft.AspNetCore.Mvc;
using TelegramBot.Api.Domain;

namespace TelegramBot.Api.Attribute;

public class BotTextGetAttribute : HttpGetAttribute
{
    public BotTextGetAttribute(string fromState, string? remainingPart = null)
        : base($"/{fromState}{BotDefaults.TextPath}/{remainingPart}")
    {

    }
}