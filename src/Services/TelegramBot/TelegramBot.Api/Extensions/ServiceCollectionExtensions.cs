using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.Services;
using TelegramBot.Api.Commands;
using TelegramBot.Api.Options;
using System.Linq;
using System;

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

        Type botCommandHandlerType = typeof(IBotCommandHandler);
        var botCommandHandlerTypes = botCommandHandlerType
            .Assembly.GetTypes()
            .Where(t => botCommandHandlerType.IsAssignableFrom(t) && !t.IsInterface);

        foreach (var botCommandHandlerImplementation in botCommandHandlerTypes)
        {
            services.AddScoped(typeof(IBotCommandHandler), botCommandHandlerImplementation);
        }

        services.AddHttpClient();
        services.AddSingleton<TelegramBotWrapper>();

        services.AddScoped<TelegramUserStateManager>();
        services.AddScoped<ICommandExecutor, CommandExecutor>();
        services.AddScoped<ITelegramUserRepository, TelegramUserRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();

        services.AddHostedService<TelegramBotInitializationHostedService>();

        return services;
    }
}