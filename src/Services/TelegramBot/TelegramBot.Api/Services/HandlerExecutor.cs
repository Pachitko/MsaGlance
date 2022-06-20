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

public class HandlerExecutor : IHandlerExecutor
{
    private readonly TelegramBotWrapper _botWrapper;
    private readonly ILogger<HandlerExecutor> _logger;
    private readonly ITelegramUserStateManager _telegramUserStateManager;
    private readonly UpdateSpecificationResolver _updateSpecificationResolver;
    private readonly IUpdateHandlerFactory _updateHandlerFactory;
    private readonly UpdateHandlerOptions _updateHandlerOptions;

    public HandlerExecutor(
        TelegramBotWrapper botWrapper,
        ILogger<HandlerExecutor> logger,
        ITelegramUserStateManager telegramUserStateManager,
        UpdateSpecificationResolver updateSpecificationResolver,
        IOptions<UpdateHandlerOptions> updateHandlerOptions,
        IUpdateHandlerFactory updateHandlerFactory)
    {
        _botWrapper = botWrapper;
        _logger = logger;
        _telegramUserStateManager = telegramUserStateManager;
        _updateSpecificationResolver = updateSpecificationResolver;
        _updateHandlerFactory = updateHandlerFactory;
        _updateHandlerOptions = updateHandlerOptions.Value;
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
                Type? handlerType = _updateHandlerOptions.Get(currentState, spec);

                IUpdateHandler handler = _updateHandlerFactory.GetUpdateHandler(handlerType);
                TelegramBotClient botClient = await _botWrapper.GetClientAsync();
                UpdateContext updateContext = new(update, botClient, (currentState, spec));

                string nextState = await handler.HandleAsync(updateContext);
                await _telegramUserStateManager.SetStateAsync(user, chatId, nextState);
                currentState = nextState;
            });
    }
}