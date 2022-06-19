using Telegram.Bot.Types;

namespace TelegramBot.Api.UpdateSpecifications.Abstractions;

public interface IUpdateSpecification
{
    bool IsSatisfiedBy(Update update);
}