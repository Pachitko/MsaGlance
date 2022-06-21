using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using FluentValidation;

namespace Identity.Api.Models;
public record UserLoginDto(string Username, string Password)
{
    public static ValueTask<UserLoginDto?> BindAsync(HttpContext httpContext)
    {
        return ValueTask.FromResult<UserLoginDto?>(
            new UserLoginDto(
                httpContext.Request.Form["username"],
                httpContext.Request.Form["password"]
            )
        );
    }
    public class Validator : AbstractValidator<UserLoginDto>
    {
    }
}