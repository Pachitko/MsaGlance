using TelegramBot.Api.UpdateSpecifications.Abstractions;
using System.Collections.Generic;
using Telegram.Bot.Types;
using System.Linq;

namespace TelegramBot.Api.Services;

public class FsmSpecificationResolver
{
    private readonly IEnumerable<IFsmSpecification> _fsmSpecifications;

    public FsmSpecificationResolver(IEnumerable<IFsmSpecification> fsmSpecifications)
    {
        _fsmSpecifications = fsmSpecifications;
    }

    public IEnumerable<IFsmSpecification> GetSatisfiedSpecifications(Update update)
        => _fsmSpecifications.Where(spec => spec.IsSatisfiedBy(update));
}