using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Api.Commands;

public class EchoCommand : IBotCommand
{
    public BotCommandType CommandType => BotCommandType.Echo;

    public async Task ExecuteAsync(Update update, TelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(update.Message.From.Id, update.Message.Text, ParseMode.MarkdownV2);
    }
}