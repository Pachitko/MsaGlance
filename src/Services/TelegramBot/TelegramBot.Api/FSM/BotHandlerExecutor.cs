using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.FSM.Models;
using TelegramBot.Api.Services;
using Telegram.Bot.Types;

namespace TelegramBot.Api.FSM;

public class BotHandlerExecutor : IFsmHandlerExecutor<BotFsmContext, Update>
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BotHandlerExecutor> _logger;
    private readonly TelegramBotWrapper _botWrapper;
    private readonly IContextAccessor<BotFsmContext, Update> _updateContextAccessor;
    private readonly BotEndpoints _endpoints;
    private readonly BotStateManager _userStateManager;

    public BotHandlerExecutor(
        IServiceProvider services,
        ILogger<BotHandlerExecutor> logger,
        TelegramBotWrapper botWrapper,
        IContextAccessor<BotFsmContext, Update> updateContextAccessor,
        BotEndpoints endpoints,
        BotStateManager telegramUserStateManager)
    {
        _services = services;
        _logger = logger;
        _botWrapper = botWrapper;
        _updateContextAccessor = updateContextAccessor;
        _endpoints = endpoints;
        _userStateManager = telegramUserStateManager;
    }

    public async Task HandleAsync(Update update)
    {
        _updateContextAccessor.Context = new BotFsmContext()
        {
            Input = update,
            BotClient = await _botWrapper.GetClientAsync()
        };

        string? msgText = update.Message?.Text;
        _logger.LogInformation("Message text: {MessageText}", msgText);

        var currentState = await _userStateManager.GetStateAsync();

        BotEndpointInfo? endpointInfo = null;
        try
        {
            endpointInfo = _endpoints
                .Endpoints
                .SingleOrDefault(e => e.TransitionAttribute.IsValidFor(currentState, update));
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError(e, "Update matched multiple endpoints");
            throw;
        }

        if (endpointInfo is not null)
        {
            var handlerType = endpointInfo.Method.DeclaringType!;
            var handlerInstance = _services.GetRequiredService(handlerType);
            object nextState = endpointInfo.Method.Invoke(handlerInstance, null)!;

            if (nextState is Task<string> task)
            {
                await _userStateManager.SetStateAsync(await task);
            }
            else if (nextState is ValueTask<string> valueTask)
            {
                await _userStateManager.SetStateAsync(await valueTask);
            }
            else
            {
                await _userStateManager.SetStateAsync((string)nextState);
            }
            _logger.LogInformation("Executed endpoint:");
        }
        else
        {
            _logger.LogError("Endpoint was not found");
        }
    }
}