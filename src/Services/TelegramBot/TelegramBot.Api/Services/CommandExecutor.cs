using Microsoft.Extensions.Logging;
using TelegramBot.Api.Exceptions;
using System.Collections.Generic;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Commands;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using System.Linq;
using System;

namespace TelegramBot.Api.Services;

public class CommandExecutor : ICommandExecutor
{
    private const BotCommandType DefaultCommand = BotCommandType.Text;
    private readonly IEnumerable<IBotCommandHandler> _commands;
    private readonly TelegramBotWrapper _botWrapper;
    private readonly ILogger<CommandExecutor> _logger;
    private readonly TelegramUserStateManager _telegramUserStateManager;

    public CommandExecutor(
        IEnumerable<IBotCommandHandler> commands,
        TelegramBotWrapper botWrapper,
        ILogger<CommandExecutor> logger,
        TelegramUserStateManager telegramUserStateManager)
    {
        _botWrapper = botWrapper ?? throw new NullReferenceException(nameof(botWrapper));
        _commands = commands ?? throw new NullReferenceException(nameof(commands));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramUserStateManager = telegramUserStateManager ?? throw new ArgumentNullException(nameof(telegramUserStateManager));
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

                    string msgText = update.Message!.Text!;
                    _logger.LogInformation("Message text: {messageText}", msgText);

                    BotCommandType commandType = GetCommandType(msgText);
                    _logger.LogInformation("Selected command type: {selectedCommand}", commandType);

                    var handlerType = await _telegramUserStateManager.GetCommandHandlerTypeAsync(update, commandType);

                    IBotCommandHandler handler = _commands.Single(c => c.GetType() == handlerType);
                    var currentState = await _telegramUserStateManager.GetStateAsync(userId);

                    UserState nextState = await handler.HandleAsync(currentState, update, await _botWrapper.GetClientAsync());
                    await _telegramUserStateManager.SetStateAsync(userId, nextState);
                }
            }
        }
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
                throw new BotCommandDoesNotExistException($"Command type '{commandName}' does not exist in enum {nameof(BotCommandType)}");
            }
        }

        return botCommandType;
    }
}