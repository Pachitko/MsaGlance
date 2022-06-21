using Microsoft.AspNetCore.Identity;
using FluentValidation.Validators;
using Identity.Domain.Entities;
using FluentValidation;

namespace Identity.Api.Models;
public record UserRegistrationDto(string Username, string Email, string Password, string PasswordConfirmation)
{
    public class Validator : AbstractValidator<UserRegistrationDto>
    {
        public Validator(UserManager<AppUser> userManager)
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MustAsync(async (x, _) => await userManager.FindByNameAsync(x) is null)
                    .WithMessage("Username already exists");

            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress(EmailValidationMode.AspNetCoreCompatible)
                    .WithMessage("Invalid email")
                .MustAsync(async (email, _) => await userManager.FindByEmailAsync(email) == null)
                    .WithMessage("Email already exists");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(x => x.PasswordConfirmation)
                .NotEmpty()
                .Equal(x => x.Password)
                    .WithMessage("Password confirmation missmatch");
        }
    }
}