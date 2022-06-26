using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.Domain.Entities;
using TelegramBot.Api.Attribute;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Filters;
using TelegramBot.Api.Domain;
using IdentityModel.Client;
using System.Net.Mime;
using Newtonsoft.Json;
using Telegram.Bot;
using System.Text;

namespace TelegramBot.Api.Controllers;

[ApiController]
[TypeFilter(typeof(BotStateActionFilterAttribute))]
public class AuthController : BotControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserTokenRepository _userTokenRepository;
    public AuthController(
        IHttpClientFactory httpClientFactory,
        IUserTokenRepository userTokenRepository)
    {
        _httpClientFactory = httpClientFactory;
        _userTokenRepository = userTokenRepository;
    }

    [BotCommandGet(BotDefaults.AnyState, "login")]
    public async Task<string> Login()
    {
        await BotContext.BotClient.SendTextMessageAsync(BotContext.UserId, "Enter <i>username:password</i>", ParseMode.Html);
        return nameof(AuthController.LoginWithUsernameAndPassword);
    }

    [BotTextGet(nameof(AuthController.LoginWithUsernameAndPassword), "{messageText}")]
    public async Task<string> LoginWithUsernameAndPassword(string messageText)
    {
        long? userId = BotContext.Input?.Message?.From?.Id;

        string[] msgTextParts = messageText.Split(':');
        string username = msgTextParts[0].Trim();
        string password = msgTextParts[1].Trim();

        var authClient = _httpClientFactory.CreateClient();
        DiscoveryDocumentRequest discoveryDocumentRequest = new()
        {
            Address = "https://idsrv",
        };
        var discoveryDocument = await authClient.GetDiscoveryDocumentAsync(discoveryDocumentRequest);

        PasswordTokenRequest passwordTokenRequest = new()
        {
            Address = discoveryDocument.TokenEndpoint,
            UserName = username,
            Password = password,
            Scope = $"disk.api.read disk.api.write {IdentityModel.OidcConstants.StandardScopes.OfflineAccess} {IdentityModel.OidcConstants.StandardScopes.OpenId}",
            ClientId = "Passworded"
        };

        TokenResponse response = await authClient.RequestPasswordTokenAsync(passwordTokenRequest);
        UserToken accessToken = new()
        {
            UserId = userId!.Value,
            LoginProvider = "idsrv",
            Name = IdentityModel.OidcConstants.TokenTypes.AccessToken,
            Value = response.AccessToken
        };

        UserToken refreshToken = new()
        {
            UserId = userId!.Value,
            LoginProvider = "idsrv",
            Name = IdentityModel.OidcConstants.TokenTypes.RefreshToken,
            Value = response.RefreshToken
        };

        if (response.HttpResponse.IsSuccessStatusCode)
        {
            await AddOrUpdateTokenAsync(accessToken);
            await AddOrUpdateTokenAsync(refreshToken);
            await BotContext.BotClient.SendTextMessageAsync(userId!, $"Login succeeded", ParseMode.MarkdownV2);
        }
        else
        {
            await BotContext.BotClient.SendTextMessageAsync(userId!, $"Login error", ParseMode.MarkdownV2);
        }
        return BotDefaults.AnyState;
    }

    private async Task AddOrUpdateTokenAsync(UserToken token)
    {
        if (!string.IsNullOrEmpty(token.Value))
        {
            if (await _userTokenRepository.GetByIdAsync((token.UserId, token.LoginProvider, token.Name)) == null)
            {
                await _userTokenRepository.AddAsync(token);
            }
            else
            {
                await _userTokenRepository.UpdateAsync(token);
            }
        }
    }

    [BotCommandGet(BotDefaults.AnyState, "register")]
    public async Task<string> Register()
    {
        await BotContext.BotClient.SendTextMessageAsync(BotContext.UserId, "Enter <i>username:email:password</i> to register", ParseMode.Html);
        return nameof(AuthController.RegisterWithUsernameAndPassword);
    }

    [BotTextGet(nameof(AuthController.RegisterWithUsernameAndPassword), "{messageText}")]
    public async Task<string> RegisterWithUsernameAndPassword(string messageText)
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
        await BotContext.BotClient.SendTextMessageAsync(BotContext.UserId, $"Registration succeeded", ParseMode.MarkdownV2);

        return BotDefaults.AnyState;
    }
}