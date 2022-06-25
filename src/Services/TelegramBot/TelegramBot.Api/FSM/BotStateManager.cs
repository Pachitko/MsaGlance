using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.Domain.Entities;
using TelegramBot.Api.FSM.Models;
using TelegramBot.Api.Domain;
using Telegram.Bot.Types;

namespace TelegramBot.Api.FSM;

public class BotStateManager : IFsmStateManager
{
    private readonly ITelegramUserRepository _userRepository;
    private readonly IContextAccessor<BotFsmContext, Update> _updateContextAccessor;

    public BotStateManager(ITelegramUserRepository userRepository, IContextAccessor<BotFsmContext, Update> updateContextAccessor)
    {
        _userRepository = userRepository ?? throw new NullReferenceException(nameof(userRepository));
        _updateContextAccessor = updateContextAccessor;
    }

    public async Task<string> GetStateAsync()
    {
        var key = GetKey();
        await CreateUserIfDoesNotExist(key);
        return await _userRepository.GetStateAsync(key.user.Id);
    }

    public async Task SetStateAsync(string nextState)
    {
        var key = GetKey();
        await CreateUserIfDoesNotExist(key);
        await _userRepository.SetStateAsync(key.user.Id, nextState);
    }

    private async Task CreateUserIfDoesNotExist((User user, long chatId) key)
    {
        // user is required to save the state
        if (await _userRepository.GetByIdAsync(key.user.Id) == null)
        {
            TelegramUser newTelegramUser = new()
            {
                Id = key.user.Id,
                IdentityId = null,
                ChatId = key.chatId,
                Username = key.user.Username!,
                State = GlobalStates.Any
            };

            await _userRepository.AddAsync(newTelegramUser);
        }
    }

    private (User user, long chatId) GetKey()
    {
        User user = _updateContextAccessor.Context.Input.Message!.From!;
        long chatId = _updateContextAccessor.Context.SafeChatId!.Value;
        return (user!, chatId);
    }
}