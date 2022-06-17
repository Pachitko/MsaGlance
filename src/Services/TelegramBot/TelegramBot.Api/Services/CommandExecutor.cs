using TelegramBot.Api.Data.Repositories;
using System.Collections.Generic;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Commands;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using System.Linq;
using Serilog;
using System;
using TelegramBot.Api.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Api.Services;

public class CommandExecutor : ICommandExecutor
{
    private const BotCommandType DefaultCommand = BotCommandType.Echo;
    private readonly IEnumerable<IBotCommand> _commands;
    private readonly TelegramBotWrapper _botWrapper;
    private readonly ITelegramUserRepository _userRepository;
    private readonly ILogger<CommandExecutor> _logger;

    // (currentState, command) -> newState
    private static readonly Dictionary<(string, BotCommandType), string> _transitions = new()
    {
        {("", BotCommandType.Echo), ""}
    };

    public CommandExecutor(
        IEnumerable<IBotCommand> commands,
        TelegramBotWrapper botWrapper,
        ITelegramUserRepository userRepository,
        ILogger<CommandExecutor> logger)
    {
        _botWrapper = botWrapper ?? throw new NullReferenceException(nameof(botWrapper));
        _commands = commands ?? throw new NullReferenceException(nameof(commands));
        _userRepository = userRepository ?? throw new NullReferenceException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ExecuteAsync(Update update)
    {
        if (update.Type == UpdateType.Message)
        {
            if (update.Message!.Type == MessageType.Text)
            {
                if (update.Message.From != null)
                {
                    long userId = update.Message.From.Id;

                    if (await _userRepository.GetByIdAsync(userId) == null)
                    {
                        TelegramUser newTelegramUser = new()
                        {
                            Id = userId,
                            IdentityId = null,
                            ChatId = update.Message.Chat.Id,
                            Username = update.Message.From.Username!,
                            State = ""
                        };

                        await _userRepository.AddAsync(newTelegramUser);
                        _logger.LogDebug("User created: {@newUser}", newTelegramUser);
                    }

                    string msgText = update.Message!.Text!;
                    _logger.LogInformation("Message text: {messageText}", msgText);

                    BotCommandType selectedCommandType = GetCommandType(msgText);
                    _logger.LogInformation("Selected command type: {selectedCommand}", selectedCommandType);

                    IBotCommand command = _commands.Single(c => c.CommandType == selectedCommandType);

                    await SetUserStateAsync(userId, selectedCommandType);
                    await command.ExecuteAsync(update, await _botWrapper.GetClientAsync());
                }
            }
        }
    }

    private async Task SetUserStateAsync(long userId, BotCommandType commandType)
    {
        string currentState = await _userRepository.GetStateAsync(userId);
        await _userRepository.SetStateAsync(userId, _transitions[(currentState, commandType)]);
    }

    private static BotCommandType GetCommandType(string rawCommand)
    {
        BotCommandType botCommandType = DefaultCommand;
        rawCommand = rawCommand.TrimStart();

        if (rawCommand.StartsWith('/'))
        {
            int firstSpaceIndex = rawCommand.IndexOf(' ');
            // remove the '/' and all text after the command name
            string commandName = rawCommand[1..(firstSpaceIndex == -1 ? rawCommand.Length : firstSpaceIndex)];

            if (Enum.TryParse<BotCommandType>(commandName, ignoreCase: true, out var parsedCommandType))
            {
                botCommandType = parsedCommandType;
            }
            else
            {
                throw new InvalidOperationException($"Command type '{commandName}' does not exist in enum {nameof(BotCommandType)}");
            }
        }

        return botCommandType;
    }
}