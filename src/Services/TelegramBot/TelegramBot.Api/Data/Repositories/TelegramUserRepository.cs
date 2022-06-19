using Microsoft.Extensions.Configuration;
using TelegramBot.Api.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using Npgsql;
using System;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Api.Data.Repositories;

public class TelegramUserRepository : ITelegramUserRepository
{
    private readonly string _dbConnectionString;
    private readonly ILogger<TelegramUserRepository> _logger;

    public TelegramUserRepository(IConfiguration configuration, ILogger<TelegramUserRepository> logger)
    {
        _dbConnectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }

    internal IDbConnection Connection => new NpgsqlConnection(_dbConnectionString);

    public async Task AddAsync(TelegramUser item)
    {
        using IDbConnection dbConnection = Connection;
        dbConnection.Open();
        await dbConnection.ExecuteAsync(
            @$"INSERT INTO users (id, identity_id, chat_id, username, state)
            VALUES (@Id, @IdentityId, @ChatId, @Username, @State)", item);
        _logger.LogDebug("User created: {@NewUser}", item);
    }

    public async Task SetStateAsync(long id, string newState)
    {
        using IDbConnection dbConnection = Connection;
        dbConnection.Open();
        await dbConnection.ExecuteAsync("UPDATE users SET state = @newState WHERE id = @id", new { newState, id });
    }

    public async Task<string> GetStateAsync(long id)
    {
        using IDbConnection dbConnection = Connection;
        dbConnection.Open();
        return await dbConnection.QueryFirstAsync<string>("SELECT state FROM users WHERE id = @id", id);
    }

    public async Task<IEnumerable<TelegramUser>> GetAllAsync()
    {
        using IDbConnection dbConnection = Connection;
        dbConnection.Open();
        return await dbConnection.QueryAsync<TelegramUser>("SELECT * FROM users");
    }

    public async Task<TelegramUser?> GetByIdAsync(long id)
    {
        using IDbConnection dbConnection = Connection;
        dbConnection.Open();
        return await dbConnection.QueryFirstOrDefaultAsync<TelegramUser>("SELECT * FROM users WHERE id = @id LIMIT 1", new { id });
    }

    public async Task RemoveAsync(long id)
    {
        using IDbConnection dbConnection = Connection;
        dbConnection.Open();
        await dbConnection.ExecuteAsync("DELETE FROM users WHERE id=@id", new { id });
    }

    public Task UpdateAsync(TelegramUser item)
    {
        throw new NotImplementedException();
    }
}