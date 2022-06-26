using TelegramBot.Api.FSM.Models;
using TelegramBot.Api.Models;
using TelegramBot.Api.Domain;
using Newtonsoft.Json;
using System.Text;

namespace TelegramBot.Api.Middlewares.Abstractions;

public class BotMessageDocumentRouteRewriteRule : IBotRouteRewriteRuleBase
{
    public bool IsApplicableTo(BotContext botContext)
    {
        return botContext.Input?.Message?.Document is not null;
    }
    public void ConfigurePath(StringBuilder pathBuilder, BotRouteRewriteContext context)
    {
        context.HttpContext.Request.Method = HttpMethod.Post.Method;

        // Replace the body with 'Update' by the 'Document' from the 'Update' for convenient binding
        byte[] documentBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(context.BotContext.Input!.Message!.Document));
        MemoryStream documentStream = new(documentBytes);
        context.HttpContext.Request.Body = documentStream;

        pathBuilder.Append($"{BotDefaults.DocumentPath}");
    }
}