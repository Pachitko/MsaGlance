using TelegramBot.Api.Domain.Entities;

namespace TelegramBot.Api.Data.Repositories;

public interface IUserTokenRepository : IRepository<UserToken, (long userId, string providerName, string name)>
{
}