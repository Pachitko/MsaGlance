using TelegramBot.Api.FSM.Models;

namespace TelegramBot.Api.Models;

public record BotRouteRewriteContext(string FromState, HttpContext HttpContext, BotContext BotContext);