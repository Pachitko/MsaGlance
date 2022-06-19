using TelegramBot.Api.Handlers.Abstractions;
using TelegramBot.Api.UpdateSpecifications;
using TelegramBot.Api.Extensions;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Options;
using TelegramBot.Api.Domain;
using TelegramBot.Api.Models;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramBot.Api.Handlers;

public class EchoHandler : IUpdateHandler
{
    public void AddTransitionsToOptions(UpdateHandlerOptions config)
    {
        config.From(GlobalStates.Any).With<NotCommandUpdateSpecification>().To<EchoHandler>();
    }

    public async Task<string> HandleAsync(UpdateContext context)
    {
        string? messageText = context.Update.Message?.Text;
        long? userId = context.Update.Message?.From?.Id;

        await context.BotClient.SendTextMessageAsync(userId!, messageText!, ParseMode.MarkdownV2);

        return GlobalStates.Any;
    }
}