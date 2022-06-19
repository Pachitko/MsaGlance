using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.Domain.Entities;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using System;
using TelegramBot.Api.Domain;
using TelegramBot.Api.Services.Abstractions;

namespace TelegramBot.Api.Services;

public class TelegramUserStateManager : ITelegramUserStateManager
{
    private readonly ITelegramUserRepository _userRepository;

    public TelegramUserStateManager(ITelegramUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new NullReferenceException(nameof(userRepository));
    }

    public  async Task<string> GetStateAsync(User user, long chatId)
    {
        await CreateUserIfDoesNotExist(user, chatId);
        return await _userRepository.GetStateAsync(user.Id);
    }

    public  async Task SetStateAsync(User user, long chatId, string nextState)
    {
        await CreateUserIfDoesNotExist(user, chatId);
        await _userRepository.SetStateAsync(user.Id, nextState);
    }
    
    private async Task CreateUserIfDoesNotExist(User user, long chatId)
    {
        // user is required to save the state
        if (await _userRepository.GetByIdAsync(user.Id) == null)
        {
            TelegramUser newTelegramUser = new()
            {
                Id = user.Id,
                IdentityId = null,
                ChatId = chatId,
                Username = user.Username!,
                State = GlobalStates.Any
            };

            await _userRepository.AddAsync(newTelegramUser);
        }
    }
}