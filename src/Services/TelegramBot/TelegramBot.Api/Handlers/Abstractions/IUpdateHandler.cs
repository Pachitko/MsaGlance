using TelegramBot.Api.Options;
using System.Threading.Tasks;
using TelegramBot.Api.Models;

namespace TelegramBot.Api.Handlers.Abstractions;

public interface IUpdateHandler
{
    void AddTransitionsToOptions(UpdateHandlerOptions config);
    Task<string> HandleAsync(UpdateContext context);
}