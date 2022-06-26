using Microsoft.AspNetCore.Mvc;
using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.FSM.Models;

namespace TelegramBot.Api.Controllers;

[ApiController]
public class BotControllerBase : ControllerBase
{
    private BotContext _botContext = default!;
    protected BotContext BotContext
    {
        get
        {
            return _botContext ??= HttpContext.RequestServices.GetRequiredService<IBotContextAccessor>().Context;
        }
    }
}