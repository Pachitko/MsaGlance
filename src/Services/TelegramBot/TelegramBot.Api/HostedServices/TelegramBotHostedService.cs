using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Api;

public class TelegramBotInitializationHostedService : BackgroundService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<TelegramBotInitializationHostedService> _logger;
    private readonly TelegramBotWrapper _botWrapper;

    public TelegramBotInitializationHostedService(TelegramBotWrapper botWrapper, IHostApplicationLifetime lifetime,
        ILogger<TelegramBotInitializationHostedService> logger)
    {
        _lifetime = lifetime;
        _logger = logger;
        _botWrapper = botWrapper;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (!await WaitForAppStartup(_lifetime, cancellationToken))
            return;

        try
        {
            await _botWrapper.InitializeBotAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("Exception has occured: {exception}", e);
        }
    }

    static async Task<bool> WaitForAppStartup(IHostApplicationLifetime lifetime, CancellationToken stoppingToken)
    {
        var startedSource = new TaskCompletionSource();
        await using var appStartedTask = lifetime.ApplicationStarted.Register(() => startedSource.TrySetResult());

        var cancelledSource = new TaskCompletionSource();
        await using var appStoppedTask = stoppingToken.Register(() => cancelledSource.TrySetResult());

        Task completedTask = await Task.WhenAny(startedSource.Task, cancelledSource.Task).ConfigureAwait(false);

        return completedTask == startedSource.Task;
    }
}