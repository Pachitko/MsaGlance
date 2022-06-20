using TelegramBot.Api.UpdateSpecifications.Abstractions;
using Telegram.Bot.Types;

namespace TelegramBot.Api.UpdateSpecifications;

public class DocumentUpdateSpecification : IFsmSpecification
{
    public virtual bool IsSatisfiedBy(Update update)
    {
        return update.Message?.Document != null;
    }
}