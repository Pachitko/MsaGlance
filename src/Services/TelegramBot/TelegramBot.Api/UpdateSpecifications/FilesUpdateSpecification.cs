
using TelegramBot.Api.UpdateSpecifications.Abstractions;

namespace TelegramBot.Api.UpdateSpecifications;

public class FilesUpdateSpecification : CommandUpdateSpecification
{
    protected override string CommandName => "/files";
}