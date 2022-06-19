using TelegramBot.Api.UpdateSpecifications.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Api.Models;

public record UpdateContext(Update Update, TelegramBotClient BotClient, IUpdateSpecification UsedSpecification, string CurrentState)
{
    public long? SafeChatId => Update.Message?.Chat.Id
                ?? Update.EditedMessage?.Chat.Id
                ?? Update.CallbackQuery?.Message?.Chat.Id
                ?? Update.InlineQuery?.From.Id;

    public long? SafeUserId => Update.Message?.From?.Id
                ?? Update.EditedMessage?.From?.Id
                ?? Update.CallbackQuery?.From?.Id
                ?? Update.InlineQuery?.From.Id;

    public long UserId =>
                (Update.Message?.From?.Id
                ?? Update.EditedMessage?.From?.Id
                ?? Update.CallbackQuery?.From?.Id
                ?? Update.InlineQuery?.From?.Id)!.Value;

    public int? MessageId => Update.Message?.MessageId;

    public int? CallbackMessageId => Update!.CallbackQuery!.Message?.MessageId;

    public int? SafeMessageId => Update.Message?.MessageId
                ?? Update.CallbackQuery?.Message?.MessageId
                ?? Update.EditedMessage?.MessageId;

    public string? SafeTextPayload => Update.Message?.Text
                ?? Update.CallbackQuery?.Data
                ?? Update.InlineQuery?.Query;
    public CallbackQuery CallbackQuery => Update!.CallbackQuery!;

    public long CallbackQueryChatId => Update!.CallbackQuery!.Message!.Chat.Id;

    public string? TypeValue => Update.Message?.Text
                ?? Update.CallbackQuery?.Data
                ?? Update.EditedMessage?.Text;

    public string? Username => Update.Message?.From?.Username
                ?? Update.CallbackQuery?.From.Username
                ?? Update.EditedMessage?.From?.Username;

    public string GetUserFullName()
    {
        var first = Update.Message?.From?.FirstName
                ?? Update.CallbackQuery?.From?.FirstName
                ?? Update.EditedMessage?.From?.FirstName;

        var last = Update.Message?.From?.LastName
                ?? Update.CallbackQuery?.From?.LastName
                ?? Update.EditedMessage?.From?.LastName;

        return first + " " + last;
    }
}