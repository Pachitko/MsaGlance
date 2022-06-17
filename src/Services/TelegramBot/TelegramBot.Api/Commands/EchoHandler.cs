using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Api.Commands;

public class EchoHandler : IBotCommandHandler
{
    public async Task<UserState> HandleAsync(UserState currentState, Update update, TelegramBotClient botClient)
    {
        string? messageText = update.Message?.Text;
        long? userId = update.Message?.From?.Id;

        await botClient.SendTextMessageAsync(userId!, messageText!, ParseMode.MarkdownV2);

        return UserState.Any;
    }
}