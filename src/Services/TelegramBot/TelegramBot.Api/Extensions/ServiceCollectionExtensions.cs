using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.FSM.Models;
using TelegramBot.Api.Services;
using TelegramBot.Api.Options;
using TelegramBot.Api.FSM;
using Telegram.Bot.Types;

namespace TelegramBot.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegramBot(this IServiceCollection services)
    {
        services.AddOptions<TelegramBotOptions>().Configure<IConfiguration>((o, c) =>
            {
                o.BotToken = c.GetValue<string>("BOT_TOKEN");
                o.WebHookEndpoint = c.GetValue<string>("WEB_HOOK_ENDPOINT");
            });

        var endpoints = BotEndpointFactory.GetHandlerEndpoints();
        services.AddSingleton(endpoints);
        foreach (var handlerType in BotEndpointFactory.Handlers)
        {
            services.AddScoped(handlerType);
        }

        services.AddScoped<BotStateManager>();
        services.AddScoped<IFsmHandlerExecutor<BotFsmContext, Update>, BotHandlerExecutor>();
        services.AddScoped<IContextAccessor<BotFsmContext, Update>, BotContextAccessor>();

        services.AddScoped<ITelegramUserRepository, TelegramUserRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();

        services.AddSingleton<TelegramBotWrapper>();

        services.AddHostedService<TelegramBotInitializationHostedService>();

        return services;
    }

    private static IServiceCollection AddByType<T>(this IServiceCollection services, ServiceLifetime serviceLifetime)
    {
        Type targetType = typeof(T);

        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(c => c.GetTypes())
            .Where(t => targetType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ForEach(implementation =>
            {
                services.Add(new ServiceDescriptor(targetType, implementation, serviceLifetime));
            });

        return services;
    }
}