using TelegramBot.Api.UpdateSpecifications.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Api.Handlers.Abstractions;
using Microsoft.Extensions.Configuration;
using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.Services;
using TelegramBot.Api.Options;
using System.Linq;
using System;
using TelegramBot.Api.Services.Abstractions;

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

        services.AddByType<IUpdateHandler>(ServiceLifetime.Scoped);
        services.AddByType<IUpdateSpecification>(ServiceLifetime.Singleton);

        services.AddHttpClient();

        services.AddScoped<IHandlerExecutor, HandlerExecutor>();
        services.AddScoped<ITelegramUserStateManager, TelegramUserStateManager>();
        services.AddScoped<IUpdateHandlerFactory, UpdateHandlerFactory>();

        services.AddScoped<ITelegramUserRepository, TelegramUserRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();

        services.AddSingleton<TelegramBotWrapper>();
        services.AddSingleton<UpdateSpecificationResolver>();

        services.AddHostedService<TelegramBotInitializationHostedService>();

        services.AddOptions<UpdateHandlerOptions>().Configure<IServiceProvider>((config, serviceProvider) =>
        {
            serviceProvider.CreateScope()
                .ServiceProvider.GetServices<IUpdateHandler>().ForEach(handler =>
            {
                handler.AddTransitionsToOptions(config);
            });
        });

        return services;
    }

    private static IServiceCollection AddByType<T>(this IServiceCollection services, ServiceLifetime serviceLifetime)
    {
        Type targetType = typeof(T);
        var types = targetType
            .Assembly.GetTypes()
            .Where(t => targetType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        types.ForEach(implementation =>
            {
                services.Add(new ServiceDescriptor(targetType, implementation, serviceLifetime));
            });

        return services;
    }
}