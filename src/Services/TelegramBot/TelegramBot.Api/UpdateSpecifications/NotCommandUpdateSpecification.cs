using TelegramBot.Api.UpdateSpecifications.Abstractions;
using Telegram.Bot.Types;

namespace TelegramBot.Api.UpdateSpecifications;

public class NotCommandUpdateSpecification : TextUpdateSpecification
{
    public override bool IsSatisfiedBy(Update update)
    {
        return base.IsSatisfiedBy(update) && !update.Message!.Text!.TrimStart().StartsWith('/');
    }
}