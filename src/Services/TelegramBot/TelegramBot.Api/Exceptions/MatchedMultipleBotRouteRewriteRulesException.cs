namespace TelegramBot.Api.Exceptions;

public class MatchedMultipleBotRouteRewriteRulesException : Exception
{
    public MatchedMultipleBotRouteRewriteRulesException(string? message, Exception? innerException) : base(message, innerException)
    {

    }
}