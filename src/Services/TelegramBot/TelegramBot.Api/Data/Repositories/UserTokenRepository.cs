using Microsoft.Extensions.Configuration;
using TelegramBot.Api.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using Npgsql;
using System;

namespace TelegramBot.Api.Data.Repositories;

public class UserTokenRepository : IUserTokenRepository
{
    private readonly string _dbConnectionString;
    private readonly ILogger<UserTokenRepository> _logger;
    private const string TableName = "user_tokens";

    public UserTokenRepository(IConfiguration configuration, ILogger<UserTokenRepository> logger)
    {
        _dbConnectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }

    internal IDbConnection Connection => new NpgsqlConnection(_dbConnectionString);

    public async Task AddAsync(UserToken item)
    {
        using IDbConnection dbConnection = Connection;
        dbConnection.Open();
        await dbConnection.ExecuteAsync(
            @$"INSERT INTO {TableName} (user_id, login_provider, name, value)
            VALUES (@UserId, @LoginProvider, @Name, @Value)", item);
        _logger.LogDebug("User token created: {@newUserToken}", item);
    }

    public async Task<IEnumerable<UserToken>> GetAllAsync()
    {
        using IDbConnection dbConnection = Connection;
        dbConnection.Open();
        return await dbConnection.QueryAsync<UserToken>($"SELECT * FROM {TableName}");
    }

    public async Task<UserToken?> GetByIdAsync((long userId, string providerName, string name) id)
    {
        using IDbConnection dbConnection = Connection;
        dbConnection.Open();
        var result = await dbConnection.QueryFirstOrDefaultAsync<UserToken>(
            @$"SELECT * 
            FROM {TableName} 
            WHERE user_id=@userId and login_provider=@providerName and name=@name",
            new { id.userId, id.providerName, id.name });
        return result;
    }

    public async Task RemoveAsync((long userId, string providerName, string name) id)
    {
        using IDbConnection dbConnection = Connection;
        dbConnection.Open();
        await dbConnection.ExecuteAsync(
            @$"DELETE
            FROM {TableName}
            WHERE user_id=@userId and login_provider=@providerName and name=@name", id);
    }

    public async Task UpdateAsync(UserToken item)
    {
        using IDbConnection dbConnection = Connection;
        dbConnection.Open();
        await dbConnection.ExecuteAsync(
            @$"UPDATE {TableName}
            SET value=@Value
            WHERE user_id=@UserId and login_provider=@LoginProvider and name=@Name", item);
    }
}