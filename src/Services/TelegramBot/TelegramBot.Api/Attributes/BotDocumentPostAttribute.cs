using Microsoft.AspNetCore.Mvc;
using TelegramBot.Api.Domain;

namespace TelegramBot.Api.Attribute;

public class BotDocumentPostAttribute : HttpPostAttribute
{
    public BotDocumentPostAttribute(string fromState)
        : base($"/{fromState}{BotDefaults.DocumentPath}")
    {

    }
}