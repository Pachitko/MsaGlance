using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.Domain.Entities;
using TelegramBot.Api.FSM.Attributes;
using TelegramBot.Api.FSM.Models;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Domain;
using IdentityModel.Client;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Api.FSM.Handlers;

public class AuthHandler : BaseFsmHandler<BotFsmContext, Update>
{
    private const string WaitingForUsernameAndPassword = "WaitingForUsernameAndPassword";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserTokenRepository _userTokenRepository;
    public AuthHandler(IHttpClientFactory httpClientFactory,
        IUserTokenRepository userTokenRepository,
        IContextAccessor<BotFsmContext, Update> updateContextAccessor)
        : base(updateContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _userTokenRepository = userTokenRepository;
    }

    [SlashCommand(GlobalStates.Any, "login", "Login")]
    public async Task<string> Login()
    {
        await Context.BotClient.SendTextMessageAsync(Context.UserId, "Enter <i>username:password</i>", ParseMode.Html);
        return WaitingForUsernameAndPassword;
    }

    [NoSlashCommand(WaitingForUsernameAndPassword)]
    public async Task<string> ContinueLogin()
    {
        string messageText = Context.Input.Message!.Text!;
        long? userId = Context.Input.Message?.From?.Id;

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
            await Context.BotClient.SendTextMessageAsync(userId!, $"Login succeeded", ParseMode.MarkdownV2);
        }
        else
        {
            await Context.BotClient.SendTextMessageAsync(userId!, $"Login error", ParseMode.MarkdownV2);
        }
        return GlobalStates.Any;
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
}