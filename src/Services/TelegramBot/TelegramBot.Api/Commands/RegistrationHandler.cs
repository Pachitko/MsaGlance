using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using System.Net.Mime;
using Newtonsoft.Json;
using System.Net.Http;
using Telegram.Bot;
using System.Text;

namespace TelegramBot.Api.Commands;

public class RegistrationHandler : IBotCommandHandler
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RegistrationHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<UserState> HandleAsync(UserState currentState, Update update, TelegramBotClient botClient)
    {
        string? messageText = update.Message?.Text;
        long? userId = update.Message?.From?.Id;

        switch (currentState)
        {
            case UserState.Any:
                await botClient.SendTextMessageAsync(userId!, "Enter <i>username:email:password</i> to register", ParseMode.Html);
                return UserState.RegisterWithUsernameAndPassword;
            case UserState.RegisterWithUsernameAndPassword:
                if (messageText != null)
                {
                    string[] msgTextParts = messageText.Split(':');
                    var registrationModel = new
                    {
                        username = msgTextParts[0].Trim(),
                        email = msgTextParts[1].Trim(),
                        password = msgTextParts[2].Trim(),
                        passwordConfirmation = msgTextParts[2].Trim()
                    };

                    // todo: validate registrationModel

                    StringContent content = new(JsonConvert.SerializeObject(registrationModel), Encoding.UTF8, MediaTypeNames.Application.Json);

                    var authClient = _httpClientFactory.CreateClient();
                    authClient.BaseAddress = new System.Uri("https://idsrv");
                    await authClient.PostAsync("/auth/register", content);
                    await botClient.SendTextMessageAsync(userId!, $"Registration succeeded", ParseMode.MarkdownV2);
                }
                return UserState.Any;
        }

        return UserState.Any;
    }
}