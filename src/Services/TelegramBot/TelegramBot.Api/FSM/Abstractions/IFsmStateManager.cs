namespace TelegramBot.Api.FSM.Abstractions;

public interface IFsmStateManager
{
    Task<string> GetStateAsync();
    Task SetStateAsync(string newState);
}