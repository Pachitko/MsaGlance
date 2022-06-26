using TelegramBot.Api.FSM.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace TelegramBot.Api.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class BotStateActionFilterAttribute : System.Attribute, IAsyncActionFilter
{
    private readonly IFsmStateManager _stateManager;
    private readonly ILogger<BotStateActionFilterAttribute> _logger;

    public BotStateActionFilterAttribute(IFsmStateManager botStateManager, ILoggerFactory loggerFactory)
    {
        _stateManager = botStateManager;
        _logger = loggerFactory.CreateLogger<BotStateActionFilterAttribute>();
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var botContextInput = context.HttpContext.RequestServices.GetRequiredService<IBotContextAccessor>().Context.Input;
        if (botContextInput is null)
        {
            throw new NullReferenceException(nameof(botContextInput));
        }

        var ctx = await next();

        if (ctx.Result is ObjectResult result)
        {
            if (result.Value is Task<string> newStateTask)
            {
                await _stateManager.SetStateAsync(await newStateTask);
            }

            if (result.Value is string newState)
            {
                await _stateManager.SetStateAsync(newState);
            }
        }
        else
        {
            _logger.LogWarning($"Result type must be \"{nameof(ObjectResult)}\" but was {{Type}}", ctx.Result?.GetType());
        }
    }
}