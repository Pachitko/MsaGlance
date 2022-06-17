namespace TelegramBot.Api.Commands;

public enum UserState : byte
{
    Any = 0,
    LoginWithUsernameAndPassword,
    RegisterWithUsernameAndPassword,
}