using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.Domain;
using TelegramBot.Api.Domain.Entities;
using TelegramBot.Api.Extensions;
using TelegramBot.Api.Handlers.Abstractions;
using TelegramBot.Api.Models;
using TelegramBot.Api.Options;
using TelegramBot.Api.UpdateSpecifications;
using TelegramBot.Api.UpdateSpecifications.Abstractions;

namespace TelegramBot.Api.Handlers;

public class LoginHandler : IUpdateHandler
{
    private const string LoginWithUsernameAndPassword = "LoginWithUsernameAndPassword";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserTokenRepository _userTokenRepository;

    public LoginHandler(IHttpClientFactory httpClientFactory, IUserTokenRepository userTokenRepository)
    {
        _httpClientFactory = httpClientFactory;
        _userTokenRepository = userTokenRepository;
    }

    public void AddTransitionsToOptions(UpdateHandlerOptions config)
    {
        config.From(GlobalStates.Any).With<LoginUpdateSpecification>().To<LoginHandler>();
        config.From(LoginWithUsernameAndPassword).With<NotCommandUpdateSpecification>().To<LoginHandler>();
    }

    public async Task<string> HandleAsync(UpdateContext context)
    {
        long? userId = context.Update.Message?.From?.Id;

        switch (context.Key)
        {
            case (GlobalStates.Any, LoginUpdateSpecification):
                await context.BotClient.SendTextMessageAsync(userId!, "Enter <i>username:password</i>", ParseMode.Html);
                return LoginWithUsernameAndPassword;
            case (LoginWithUsernameAndPassword, NotCommandUpdateSpecification):
                string messageText = context.Update.Message!.Text!;

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
                    await context.BotClient.SendTextMessageAsync(userId!, $"Login succeeded", ParseMode.MarkdownV2);
                }
                else
                {
                    await context.BotClient.SendTextMessageAsync(userId!, $"Login error", ParseMode.MarkdownV2);
                }
                return GlobalStates.Any;
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