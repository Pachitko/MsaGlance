using FluentValidation;

namespace Identity.Api.Models;
public record UserLoginDto(string Username, string Password)
{
    public class Validator : AbstractValidator<UserLoginDto>
    {
    }
}