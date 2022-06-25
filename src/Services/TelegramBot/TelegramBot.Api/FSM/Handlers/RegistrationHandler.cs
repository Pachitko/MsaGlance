using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.FSM.Attributes;
using TelegramBot.Api.FSM.Models;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Domain;
using Telegram.Bot.Types;
using Telegram.Bot;
using Newtonsoft.Json;
using System.Text;
using System.Net.Mime;

namespace TelegramBot.Api.FSM.Handlers;

public class RegistrationHandler : BaseFsmHandler<BotFsmContext, Update>
{
    private const string RegisterWithUsernameAndPassword = "RegisterWithUsernameAndPassword";

    private readonly IHttpClientFactory _httpClientFactory;

    public RegistrationHandler(IHttpClientFactory httpClientFactory,
    IContextAccessor<BotFsmContext, Update> updateContextAccessor) : base(updateContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
    }

    [SlashCommand(GlobalStates.Any, "register", "Register")]
    public async Task<string> Register()
    {
        await Context.BotClient.SendTextMessageAsync(Context.UserId, "Enter <i>username:email:password</i> to register", ParseMode.Html);
        return RegisterWithUsernameAndPassword;
    }

    [NoSlashCommand(RegisterWithUsernameAndPassword)]
    public async Task<string> ContinuteRegistration()
    {
        string messageText = Context.Input.Message!.Text!;

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
        await Context.BotClient.SendTextMessageAsync(Context.UserId, $"Registration succeeded", ParseMode.MarkdownV2);

        return GlobalStates.Any;
    }
}