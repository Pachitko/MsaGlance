using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TelegramBot.Api.Options;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using System;
using TelegramBot.Api.Extensions;
using TelegramBot.Api.Handlers;
using TelegramBot.Api.Handlers.Abstractions;
using TelegramBot.Api.Services.Abstractions;
using TelegramBot.Api.Models;
using Telegram.Bot;

namespace TelegramBot.Api.Services;

public class FsmHandlerExecutor : IFsmHandlerExecutor
{
    private readonly TelegramBotWrapper _botWrapper;
    private readonly ILogger<FsmHandlerExecutor> _logger;
    private readonly ITelegramUserStateManager _telegramUserStateManager;
    private readonly FsmSpecificationResolver _updateSpecificationResolver;
    private readonly IFsmHandlerFactory _updateHandlerFactory;
    private readonly FsmOptions _fsmOptions;

    public FsmHandlerExecutor(
        TelegramBotWrapper botWrapper,
        ILogger<FsmHandlerExecutor> logger,
        ITelegramUserStateManager telegramUserStateManager,
        FsmSpecificationResolver updateSpecificationResolver,
        IOptions<FsmOptions> fsmOptions,
        IFsmHandlerFactory updateHandlerFactory)
    {
        _botWrapper = botWrapper;
        _logger = logger;
        _telegramUserStateManager = telegramUserStateManager;
        _updateSpecificationResolver = updateSpecificationResolver;
        _updateHandlerFactory = updateHandlerFactory;
        _fsmOptions = fsmOptions.Value;
    }

    public async Task HandleAsync(Update update)
    {
        if (update.Message?.From is null)
            return;

        string? msgText = update.Message?.Text;
        _logger.LogInformation("Message text: {MessageText}", msgText);

        User user = update.Message!.From;
        long chatId = update.Message.Chat.Id;

        var currentState = await _telegramUserStateManager.GetStateAsync(update.Message.From, chatId);

        // todo ?????????????????? 
        // Only one specification must satisfy the update, so-called Endpoints in ASP.NET Core
        // ??????
        _updateSpecificationResolver
            .GetSatisfiedSpecifications(update)
            .ForEach(async spec =>
            {
                Type? handlerType = _fsmOptions.Get(currentState, spec);

                IFsmHandler handler = _updateHandlerFactory.GetFsmHandler(handlerType);
                TelegramBotClient botClient = await _botWrapper.GetClientAsync();
                UpdateContext updateContext = new(update, botClient, (currentState, spec));

                string nextState = await handler.HandleAsync(updateContext);
                await _telegramUserStateManager.SetStateAsync(user, chatId, nextState);
                currentState = nextState;
            });
    }
}