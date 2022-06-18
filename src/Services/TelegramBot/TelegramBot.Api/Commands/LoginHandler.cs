using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Net.Http;
using System.Collections.Generic;
using IdentityModel.Client;
using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.Domain.Entities;

namespace TelegramBot.Api.Commands;

public class LoginHandler : IBotCommandHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserTokenRepository _userTokenRepository;

    public LoginHandler(IHttpClientFactory httpClientFactory, IUserTokenRepository userTokenRepository)
    {
        _httpClientFactory = httpClientFactory;
        _userTokenRepository = userTokenRepository;
    }
    public async Task<UserState> HandleAsync(UserState currentState, Update update, TelegramBotClient botClient)
    {
        long? userId = update.Message?.From?.Id;

        switch (currentState)
        {
            case UserState.Any:
                await botClient.SendTextMessageAsync(userId!, "Enter <i>username:password</i>", ParseMode.Html);
                return UserState.LoginWithUsernameAndPassword;
            case UserState.LoginWithUsernameAndPassword:
                string messageText = update.Message!.Text!;

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
                    LoginProvider = "idsrv_passworded",
                    Name = "access_token",
                    Value = response.AccessToken
                };

                UserToken refreshToken = new()
                {
                    UserId = userId!.Value,
                    LoginProvider = "idsrv_passworded",
                    Name = "refresh_token",
                    Value = response.RefreshToken
                };

                if (response.HttpResponse.IsSuccessStatusCode)
                {
                    await AddOrUpdateTokenAsync(accessToken);
                    await AddOrUpdateTokenAsync(refreshToken);
                    await botClient.SendTextMessageAsync(userId!, $"Login succeeded", ParseMode.MarkdownV2);
                }
                else
                {
                    await botClient.SendTextMessageAsync(userId!, $"Login error", ParseMode.MarkdownV2);
                }
                return UserState.Any;
        }

        return UserState.Any;
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