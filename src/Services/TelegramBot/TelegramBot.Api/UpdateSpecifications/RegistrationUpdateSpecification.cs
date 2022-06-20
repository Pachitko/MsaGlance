using TelegramBot.Api.UpdateSpecifications.Abstractions;

namespace TelegramBot.Api.UpdateSpecifications;

public class RegistrationUpdateSpecification : CommandUpdateSpecification
{
    protected override string CommandName => "/register";
}