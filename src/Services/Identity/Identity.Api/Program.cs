using Identity.Api;
using Serilog;
using Identity.Api.Domain.Entities;
using Identity.Api.Data;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Identity.Api.Models;
using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Http;
using Identity.Api.Options;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Net.Http.Headers;
using IdentityServer4.Services;
using FluentValidation;
using System.Threading.Tasks;
using Identity.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo
    .Console()
    .CreateLogger();

builder.Host.UseSerilog();

string dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var migrationsAssembly = Assembly.GetExecutingAssembly().GetName().Name;

builder.Services
    .AddDbContext<AuthDbContext>(o => o.UseNpgsql(dbConnectionString, o => o.MigrationsAssembly(migrationsAssembly)))
    .AddIdentity<AppUser, AppRole>(c =>
    {
        c.User.RequireUniqueEmail = true;

        c.Password.RequiredLength = 8;
        c.Password.RequireDigit = false;
        c.Password.RequireNonAlphanumeric = false;
        c.Password.RequireUppercase = false;
        c.Password.RequireLowercase = false;
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AuthDbContext>();

var idsrvBuilder = builder.Services
    .AddIdentityServer(c =>
    {
        c.IssuerUri = "http://idsrv";
        c.UserInteraction.LoginUrl = "/auth/login";
    })
    .AddAspNetIdentity<AppUser>()
    .AddOperationalStore(options => options.ConfigureDbContext =
        builder => builder.UseNpgsql(dbConnectionString, o => o.MigrationsAssembly(migrationsAssembly)))
    .AddConfigurationStore(options => options.ConfigureDbContext =
        builder => builder.UseNpgsql(dbConnectionString, o => o.MigrationsAssembly(migrationsAssembly)))
    .AddInMemoryApiResources(Configuration.ApiResources)
    .AddInMemoryClients(Configuration.Clients)
    .AddInMemoryIdentityResources(Configuration.IdentityResources)
    .AddInMemoryApiScopes(Configuration.IdentityApiScopes)
    .AddDeveloperSigningCredential();

builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

SigningCredentialsOptions signingCredentialsOptions = builder.Configuration
    .GetSection("SigningCredentials")
    .Get<SigningCredentialsOptions>();

switch (signingCredentialsOptions.Type)
{
    case "cert":
        idsrvBuilder.AddSigningCredential(new System.Security.Cryptography.X509Certificates.X509Certificate2(signingCredentialsOptions.CertName, signingCredentialsOptions.CertPassword));
        break;
    case "default":
        idsrvBuilder.AddDeveloperSigningCredential();
        break;
    default:
        idsrvBuilder.AddDeveloperSigningCredential();
        break;
}

builder.Services.AddFluentValidation(config =>
{
    config.ImplicitlyValidateRootCollectionElements = true;
    config.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});

// Pipeline
var app = builder.Build();
await app.Services.CreateScope().ServiceProvider.SeedDataAsync();

app.UseCors(c => c.WithOrigins("http://webapi").AllowAnyHeader().AllowAnyMethod());

app.UseIdentityServer();

app.MapGet("/", () =>
{
    DateTime now = DateTime.UtcNow;
    return $"Identity {now}";
});

app.MapGet("/users", async (UserManager<AppUser> userManager) =>
{
    return await userManager.Users.ToListAsync();
});

app.MapGet("/roles", async (RoleManager<AppRole> roleManager) =>
{
    return await roleManager.Roles.ToListAsync();
});

app.MapPost("/auth/register", Register);

app.MapGet("/auth/login", async () =>
    Results.Content(await File.ReadAllTextAsync("./wwwroot/login.html"),
        MediaTypeHeaderValue.Parse(System.Net.Mime.MediaTypeNames.Text.Html)));

app.MapPost("/auth/login", async (UserLoginDto loginDto, [FromQuery] string returnUrl,
    IIdentityServerInteractionService interaction, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) =>
{
    var context = await interaction.GetAuthorizationContextAsync(returnUrl);

    var user = await userManager.FindByNameAsync(loginDto.Username);
    if (user is null)
        return Results.NotFound("User not found");

    var signInResult = await signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
    if (signInResult.Succeeded)
    {
        if (context is not null)
        {
            return Results.Redirect(returnUrl);
        }

        if (string.IsNullOrEmpty(returnUrl))
        {
            return Results.LocalRedirect("~/");
        }
        else
        {
            // user might have clicked on a malicious link - should be logged
            throw new Exception("invalid return URL");
        }
    }

    return Results.BadRequest(signInResult);
});

await app.RunAsync();

static async Task<IResult> Register(UserRegistrationDto registrationDto, UserManager<AppUser> userManager,
    IValidator<UserRegistrationDto> validator, ITokenService tokenService)
{
    var validationResult = await validator.ValidateAsync(registrationDto);
    if (!validationResult.IsValid)
        return Results.ValidationProblem(validationResult.ToDictionary());

    AppUser newUser = new()
    {
        UserName = registrationDto.Username,
        Email = registrationDto.Email
    };

    var createUserResult = await userManager.CreateAsync(newUser, registrationDto.Password);

    if (createUserResult.Succeeded)
    {
        var addToRoleResult = await userManager.AddToRoleAsync(newUser, "User");
        if (addToRoleResult.Succeeded)
        {
            return Results.Ok("User has been created!");
        }
        else
        {
            return Results.BadRequest(addToRoleResult.Errors);
        }
    }
    else
    {
        return Results.BadRequest(createUserResult.Errors);
    }
}