
using TelegramBot.Api.UpdateSpecifications.Abstractions;

namespace TelegramBot.Api.UpdateSpecifications;

public class LoginUpdateSpecification : CommandUpdateSpecification
{
    protected override string CommandName => "/login";
}