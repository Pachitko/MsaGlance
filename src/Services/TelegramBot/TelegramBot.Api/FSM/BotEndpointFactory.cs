using TelegramBot.Api.FSM.Attributes;
using TelegramBot.Api.FSM.Handlers;
using TelegramBot.Api.FSM.Models;
using Telegram.Bot.Types;
using System.Reflection;

namespace TelegramBot.Api.FSM;

public record BotEndpointInfo(UpdateValidatableAttribute TransitionAttribute, MethodInfo Method);
public record BotEndpoints(List<BotEndpointInfo> Endpoints);

public static class BotEndpointFactory
{
    private static Type BaseHandler { get; } = typeof(BaseFsmHandler<BotFsmContext, Update>);
    public static List<Type> Handlers { get; } =
        AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(c => c.GetTypes())
        .Where(c => !c.IsAbstract && BaseHandler.IsAssignableFrom(c))
        .ToList();

    public static BotEndpoints GetHandlerEndpoints()
        => new(Handlers
            .SelectMany(c => c.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            .Select(m =>
            {
                var transitionAttr = m.GetCustomAttribute<UpdateValidatableAttribute>();
                return new BotEndpointInfo(transitionAttr!, m);
            })
            .Where(x => x.TransitionAttribute is not null)
            .ToList());
}
