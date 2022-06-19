using Telegram.Bot.Types;

namespace TelegramBot.Api.UpdateSpecifications.Abstractions;

public abstract class TextUpdateSpecification : IUpdateSpecification
{
    public virtual bool IsSatisfiedBy(Update update)
    {
        return update.Message?.Text != null;
    }
}