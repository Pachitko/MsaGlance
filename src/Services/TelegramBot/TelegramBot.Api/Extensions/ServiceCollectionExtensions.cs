using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.Services;
using TelegramBot.Api.Commands;
using TelegramBot.Api.Options;

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

        services.AddSingleton<TelegramBotWrapper>();
        services.AddSingleton<IBotCommandHandler, EchoHandler>();
        services.AddSingleton<IBotCommandHandler, RegistrationHandler>();

        services.AddScoped<TelegramUserStateManager>();
        services.AddScoped<ICommandExecutor, CommandExecutor>();
        services.AddScoped<ITelegramUserRepository, TelegramUserRepository>();

        services.AddHostedService<TelegramBotInitializationHostedService>();

        return services;
    }
}