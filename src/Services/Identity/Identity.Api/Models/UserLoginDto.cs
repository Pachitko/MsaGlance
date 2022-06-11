using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Identity.Api.Models;
public record UserLoginDto(string Username, string Password)
{
    public static ValueTask<UserLoginDto?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
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