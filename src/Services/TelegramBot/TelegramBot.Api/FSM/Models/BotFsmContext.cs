using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Api.FSM.Models;

public class BotContext : BaseFsmContext<Update>
{
    public TelegramBotClient BotClient { get; set; } = default!;

    public long? SafeChatId => Input?.Message?.Chat.Id
        ?? Input?.EditedMessage?.Chat.Id
        ?? Input?.CallbackQuery?.Message?.Chat.Id
        ?? Input?.InlineQuery?.From.Id;

    public long? SafeUserId => Input?.Message?.From?.Id
        ?? Input?.EditedMessage?.From?.Id
        ?? Input?.CallbackQuery?.From?.Id
        ?? Input?.InlineQuery?.From.Id;

    public long UserId =>
        (Input?.Message?.From?.Id
        ?? Input?.EditedMessage?.From?.Id
        ?? Input?.CallbackQuery?.From?.Id
        ?? Input?.InlineQuery?.From?.Id)!.Value;

    public int? MessageId => Input?.Message?.MessageId;

    public int? CallbackMessageId => Input?.CallbackQuery!.Message?.MessageId;

    public int? SafeMessageId => Input?.Message?.MessageId
        ?? Input?.CallbackQuery?.Message?.MessageId
        ?? Input?.EditedMessage?.MessageId;

    public string? SafeTextPayload => Input?.Message?.Text
        ?? Input?.CallbackQuery?.Data
        ?? Input?.InlineQuery?.Query;
    public CallbackQuery CallbackQuery => Input!.CallbackQuery!;

    public long CallbackQueryChatId => Input!.CallbackQuery!.Message!.Chat.Id;

    public string? TypeValue => Input?.Message?.Text
        ?? Input?.CallbackQuery?.Data
        ?? Input?.EditedMessage?.Text;

    public string? Username => Input?.Message?.From?.Username
        ?? Input?.CallbackQuery?.From.Username
        ?? Input?.EditedMessage?.From?.Username;

    public string GetUserFullName()
    {
        var first = Input?.Message?.From?.FirstName
            ?? Input?.CallbackQuery?.From?.FirstName
            ?? Input?.EditedMessage?.From?.FirstName;

        var last = Input?.Message?.From?.LastName
            ?? Input?.CallbackQuery?.From?.LastName
            ?? Input?.EditedMessage?.From?.LastName;

        return first + " " + last;
    }
}