using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Authentication.JwtBearer;

public class BotAuthenticationHandler : IAuthenticationSignInHandler
{
    private readonly IOptionsMonitor<BotAuthenticationOptions> _options;
    private readonly ILogger<BotAuthenticationHandler> _logger;

    public BotAuthenticationHandler(IOptionsMonitor<BotAuthenticationOptions> options, ILoggerFactory loggerFactory)
    {
        _options = options;
        _logger = loggerFactory.CreateLogger<BotAuthenticationHandler>();
    }

    public Task<AuthenticateResult> AuthenticateAsync()
    {
        throw new NotImplementedException();
    }

    public Task ChallengeAsync(AuthenticationProperties? properties)
    {
        throw new NotImplementedException();
    }

    public Task ForbidAsync(AuthenticationProperties? properties)
    {
        throw new NotImplementedException();
    }

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        throw new NotImplementedException();
    }

    public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties? properties)
    {
        throw new NotImplementedException();
    }

    public Task SignOutAsync(AuthenticationProperties? properties)
    {
        throw new NotImplementedException();
    }
}