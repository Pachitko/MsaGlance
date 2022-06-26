namespace TelegramBot.Api.Exceptions;

public class BotRouteRewriteRuleNotFoundException : Exception
{
    public BotRouteRewriteRuleNotFoundException(string? message) : base(message)
    {
    }
}