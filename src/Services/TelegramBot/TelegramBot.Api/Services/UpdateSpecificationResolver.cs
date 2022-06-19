using TelegramBot.Api.UpdateSpecifications.Abstractions;
using System.Collections.Generic;
using Telegram.Bot.Types;
using System.Linq;

namespace TelegramBot.Api.Services;

public class UpdateSpecificationResolver
{
    private readonly IEnumerable<IUpdateSpecification> _updateSpecifications;

    public UpdateSpecificationResolver(IEnumerable<IUpdateSpecification> updateSpecifications)
    {
        _updateSpecifications = updateSpecifications;
    }

    public IEnumerable<IUpdateSpecification> GetSatisfiedSpecifications(Update update)
        => _updateSpecifications.Where(spec => spec.IsSatisfiedBy(update));
}