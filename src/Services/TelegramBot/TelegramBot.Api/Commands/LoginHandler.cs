using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Api.Commands;

public class LoginHandler : IBotCommandHandler
{
    public async Task<UserState> HandleAsync(UserState currentState, Update update, TelegramBotClient botClient)
    {
        if (currentState == UserState.LoginWithUsernameAndPassword)
        {
            // Retrieve access_token from idsrv and save it in tele_bot db
            return UserState.Any;
        }
        else
        {
            long? userId = update.Message?.From?.Id;
            await botClient.SendTextMessageAsync(userId!, "Enter <i>username:password</i>", ParseMode.Html);
            return UserState.LoginWithUsernameAndPassword;
        }
    }
}