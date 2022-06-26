using TelegramBot.Api.FSM.Models;
using TelegramBot.Api.Models;
using System.Text;

namespace TelegramBot.Api.Middlewares.Abstractions;

public interface IBotRouteRewriteRuleBase
{
    bool IsApplicableTo(BotContext botContext);
    void ConfigurePath(StringBuilder pathBuilder, BotRouteRewriteContext context);
}