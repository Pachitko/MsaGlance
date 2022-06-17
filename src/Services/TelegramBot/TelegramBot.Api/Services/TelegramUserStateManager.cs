using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.Domain.Entities;
using System.Collections.Generic;
using TelegramBot.Api.Commands;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using System;

namespace TelegramBot.Api.Services;

public class TelegramUserStateManager
{
    private readonly ITelegramUserRepository _userRepository;

    public TelegramUserStateManager(ITelegramUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new NullReferenceException(nameof(userRepository));
    }

    // currentState + command = newState (handler)
    private static readonly Dictionary<(UserState, BotCommandType), Type> _transitions = new()
    {
        {(UserState.Any, BotCommandType.Text), typeof(EchoHandler)},

        {(UserState.Any, BotCommandType.Login), typeof(LoginHandler)},
        {(UserState.LoginWithUsernameAndPassword, BotCommandType.Text), typeof(LoginHandler)},

        {(UserState.Any, BotCommandType.Register), typeof(RegistrationHandler)},
        {(UserState.RegisterWithUsernameAndPassword, BotCommandType.Text), typeof(RegistrationHandler)},
    };

    public async Task<Type> GetCommandHandlerTypeAsync(Update update, BotCommandType commandType)
    {
        if (update.Message?.From == null)
            throw new NullReferenceException();

        long userId = update.Message.From.Id;

        // user is required to save the state
        if (await _userRepository.GetByIdAsync(userId) == null)
        {
            TelegramUser newTelegramUser = new()
            {
                Id = userId,
                IdentityId = null,
                ChatId = update.Message.Chat.Id,
                Username = update.Message.From.Username!,
                State = UserState.Any
            };

            await _userRepository.AddAsync(newTelegramUser);
        }

        UserState currentState = await GetStateAsync(userId);
        if (!_transitions.TryGetValue((currentState, commandType), out Type? handlerType))
        {
            handlerType = _transitions[(UserState.Any, commandType)];
            await SetStateAsync(userId, UserState.Any);
        }

        return handlerType;
    }

    public async Task<UserState> GetStateAsync(long userId)
    {
        return await _userRepository.GetStateAsync(userId);
    }

    public async Task SetStateAsync(long userId, UserState nextState)
    {
        await _userRepository.SetStateAsync(userId, nextState);
    }
}