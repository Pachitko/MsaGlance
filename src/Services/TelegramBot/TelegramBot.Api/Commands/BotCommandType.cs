namespace TelegramBot.Api.Commands;

public enum BotCommandType : byte
{
    Text = 0,
    Echo,
    Login,
    Register,
}