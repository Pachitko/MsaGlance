using TelegramBot.Api.Domain.Entities;
using System.Threading.Tasks;

namespace TelegramBot.Api.Data.Repositories;

public interface ITelegramUserRepository : IRepository<TelegramUser, long>
{
    Task SetStateAsync(long id, string newState);
    Task<string> GetStateAsync(long id);
}