using Telegram.Bot.Types;

namespace TelegramBot.Api.UpdateSpecifications.Abstractions;

public abstract class TextUpdateSpecification : IFsmSpecification
{
    public virtual bool IsSatisfiedBy(Update update)
    {
        return update.Message?.Text != null;
    }
}