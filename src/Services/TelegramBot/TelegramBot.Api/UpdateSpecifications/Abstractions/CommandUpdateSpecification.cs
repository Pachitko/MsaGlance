using Telegram.Bot.Types;

namespace TelegramBot.Api.UpdateSpecifications.Abstractions;

public abstract class CommandUpdateSpecification : TextUpdateSpecification
{
    protected abstract string CommandName { get; }
    public override bool IsSatisfiedBy(Update update)
    {
        return base.IsSatisfiedBy(update) && IsCommand(update.Message!.Text!, CommandName);
    }

    private static bool IsCommand(string messageText, string expectedCommandName)
    {
        messageText = messageText.TrimStart();
        return messageText.StartsWith($"/{expectedCommandName}", System.StringComparison.OrdinalIgnoreCase);
    }
}