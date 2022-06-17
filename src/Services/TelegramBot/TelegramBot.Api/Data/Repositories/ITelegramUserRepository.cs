using TelegramBot.Api.Domain.Entities;
using TelegramBot.Api.Commands;
using System.Threading.Tasks;

namespace TelegramBot.Api.Data.Repositories;

public interface ITelegramUserRepository : IRepository<TelegramUser, long>
{
    Task SetStateAsync(long id, UserState newState);
    Task<UserState> GetStateAsync(long id);
}