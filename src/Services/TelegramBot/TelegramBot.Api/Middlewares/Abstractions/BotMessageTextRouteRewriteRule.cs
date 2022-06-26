using TelegramBot.Api.FSM.Models;
using TelegramBot.Api.Domain;
using TelegramBot.Api.Models;
using System.Text;

namespace TelegramBot.Api.Middlewares.Abstractions;

public class BotMessageTextRouteRewriteRule : IBotRouteRewriteRuleBase
{
    public void ConfigurePath(StringBuilder pathBuilder, BotRouteRewriteContext context)
    {
        context.HttpContext.Request.Method = HttpMethod.Get.Method;
        string[] messageTextParts = context.BotContext.Input!.Message!.Text!.Split(' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        string firstPart = messageTextParts[0];

        if (IsSlashCommand(firstPart))
        {
            pathBuilder.Append($"{firstPart}");
            if (messageTextParts.Length > 1)
            {
                // todo pass message text args to endpoint
                throw new NotImplementedException();
                for (int i = 1; i < messageTextParts.Length; i++)
                {
                    // newPath.Append($"{messageTextParts[i]} ");
                }
            }
        }
        else
        {
            pathBuilder.Append($"{BotDefaults.TextPath}/{firstPart}");
        }
    }

    public bool IsApplicableTo(BotContext botContext)
    {
        return botContext.Input?.Message?.Text is not null;
    }

    private static bool IsSlashCommand(string s) => s.StartsWith('/');
}