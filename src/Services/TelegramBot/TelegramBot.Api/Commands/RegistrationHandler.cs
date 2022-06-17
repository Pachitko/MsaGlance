using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Api.Commands;

public class RegistrationHandler : IBotCommandHandler
{
    public async Task<UserState> HandleAsync(UserState currentState, Update update, TelegramBotClient botClient)
    {
        string? messageText = update.Message?.Text;
        long? userId = update.Message?.From?.Id;

        switch (currentState)
        {
            case UserState.Any:
                await botClient.SendTextMessageAsync(userId!, "Enter <i>username:password</i> to register", ParseMode.Html);
                return UserState.RegisterWithUsernameAndPassword;
            case UserState.RegisterWithUsernameAndPassword:
                if (messageText != null)
                {
                    string[] msgTextParts = messageText.Split(':');
                    string username = msgTextParts[0].Trim();
                    string password = msgTextParts[1].Trim();
                    await botClient.SendTextMessageAsync(userId!, $"({username}|{password})", ParseMode.MarkdownV2);
                }
                return UserState.Any;
        }

        return UserState.Any;
    }
}