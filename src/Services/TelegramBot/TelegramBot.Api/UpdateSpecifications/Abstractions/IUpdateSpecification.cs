using Telegram.Bot.Types;

namespace TelegramBot.Api.UpdateSpecifications.Abstractions;

public interface IFsmSpecification
{
    bool IsSatisfiedBy(Update update);
}