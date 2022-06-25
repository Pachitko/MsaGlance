using Telegram.Bot.Types;

namespace TelegramBot.Api.FSM.Attributes;

public class SlashCommandAttribute : UpdateValidatableAttribute
{
    public readonly string CommandName;
    public readonly string Desc;

    public SlashCommandAttribute(string fromState, string commandName, string desc)
        : base(fromState)
    {
        CommandName = commandName.StartsWith("/") ? commandName : $"/{commandName}";
        Desc = desc;
    }

    protected override bool IsValidForUpdate(Update update)
    {
        return update.Message?.Text is not null && update.Message.Text.StartsWith(CommandName);
    }
}