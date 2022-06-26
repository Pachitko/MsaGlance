using TelegramBot.Api.Middlewares.Abstractions;
using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.Exceptions;
using TelegramBot.Api.Models;
using System.Text;

namespace TelegramBot.Api.Middlewares;

public class BotRouteRewriteMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BotRouteRewriteMiddleware> _logger;
    private readonly IEnumerable<IBotRouteRewriteRuleBase> _botRouteRewriteRules;

    public BotRouteRewriteMiddleware(
        RequestDelegate next,
        ILogger<BotRouteRewriteMiddleware> logger,
        IEnumerable<IBotRouteRewriteRuleBase> botRouteRewriteRules)
    {
        _next = next;
        _logger = logger;
        _botRouteRewriteRules = botRouteRewriteRules;
    }

    public async Task Invoke(HttpContext context, IBotContextAccessor botContextAccessor, IFsmStateManager stateManager)
    {
        IBotRouteRewriteRuleBase? applicableBotRouteRewriteRule = null;
        try
        {
            applicableBotRouteRewriteRule = _botRouteRewriteRules
                .SingleOrDefault(x => x.IsApplicableTo(botContextAccessor!.Context));
        }
        catch (InvalidOperationException e)
        {
            throw new MatchedMultipleBotRouteRewriteRulesException(null, e);
        }

        string currentState = await stateManager.GetStateAsync();

        // state is the necessary part of the path
        StringBuilder pathBuiler = new($"/{currentState}");

        if (applicableBotRouteRewriteRule is null)
        {
            _logger.LogWarning("Bot route rewrite rule was not found");
            throw new BotRouteRewriteRuleNotFoundException(null);
        }
        else
        {
            BotRouteRewriteContext botRouteRewriteContext = new(currentState, context, botContextAccessor!.Context);
            applicableBotRouteRewriteRule.ConfigurePath(pathBuiler, botRouteRewriteContext);
        }

        _logger.LogInformation("Rewrite path to: \"{NewPath}\"", pathBuiler);
        context.Request.Path = PathString.FromUriComponent(pathBuiler.ToString());

        await _next(context);
    }
}