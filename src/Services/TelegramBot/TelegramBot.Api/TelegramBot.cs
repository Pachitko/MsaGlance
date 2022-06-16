using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using TelegramBot.Api.Options;
using Telegram.Bot;
using Serilog;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Api;

public class TelegramBotWrapper
{
    private TelegramBotClient? _botClient;
    private readonly TelegramBotOptions _options;
    public ILogger<TelegramBotWrapper> _logger;

    public TelegramBotWrapper(IOptions<TelegramBotOptions> options, ILogger<TelegramBotWrapper> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<TelegramBotClient> GetClientAsync()
    {
        if (_botClient == null)
            await InitializeBotAsync();

        return _botClient!;
    }

    public async Task InitializeBotAsync()
    {
        if (_botClient != null)
        {
            Log.Information("The bot has been already initialized");
            return;
        }

        _logger.LogDebug("Initializing the bot client with options: {@options}", _options);
        _botClient = new TelegramBotClient(_options.BotToken);
        await _botClient.SetWebhookAsync(_options.WebHookEndpoint);
    }
}