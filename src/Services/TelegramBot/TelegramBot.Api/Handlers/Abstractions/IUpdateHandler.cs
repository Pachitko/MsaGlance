using TelegramBot.Api.Options;
using System.Threading.Tasks;
using TelegramBot.Api.Models;

namespace TelegramBot.Api.Handlers.Abstractions;

public interface IFsmHandler
{
    void AddTransitionsToFsmOptions(FsmOptions options);
    Task<string> HandleAsync(UpdateContext context);
}