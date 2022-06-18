using TelegramBot.Api.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TelegramBot.Api.Data.Repositories
{
    public interface IRepository<T, TKey> where T : class
    {
        Task AddAsync(T item);
        Task RemoveAsync(TKey id);
        Task UpdateAsync(T item);
        Task<T?> GetByIdAsync(TKey id);
        Task<IEnumerable<T>> GetAllAsync();
    }
}