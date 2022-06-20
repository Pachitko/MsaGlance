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

        services.AddByType<IFsmHandler>(ServiceLifetime.Scoped);
        services.AddByType<IFsmSpecification>(ServiceLifetime.Singleton);

        services.AddHttpClient();

        services.AddScoped<IFsmHandlerExecutor, FsmHandlerExecutor>();
        services.AddScoped<ITelegramUserStateManager, TelegramUserStateManager>();
        services.AddScoped<IFsmHandlerFactory, FsmHandlerFactory>();

        services.AddScoped<ITelegramUserRepository, TelegramUserRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();

        services.AddSingleton<TelegramBotWrapper>();
        services.AddSingleton<FsmSpecificationResolver>();

        services.AddHostedService<TelegramBotInitializationHostedService>();

        services.AddOptions<FsmOptions>().Configure<IServiceProvider>((config, serviceProvider) =>
        {
            serviceProvider.CreateScope()
                .ServiceProvider.GetServices<IFsmHandler>().ForEach(handler =>
            {
                handler.AddTransitionsToFsmOptions(config);
            });
        });

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