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
using Microsoft.AspNetCore.Antiforgery;

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

builder.Services.AddAntiforgery(options =>
    {
        options.Cookie.Name = "X-XSRF-COOKIE-TOKEN";
        options.HeaderName = "X-XSRF-REQUEST-TOKEN";
        options.FormFieldName = "X-XSRF-REQUEST-TOKEN";
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

var app = builder.Build();
await app.Services.CreateScope().ServiceProvider.SeedDataAsync();

app.Use(async (httpContext, next) =>
{
    if (httpContext.Request.Path.HasValue && httpContext.Request.Path.Value == "/auth/login" && httpContext.Request.Method == "POST")
    {
        IAntiforgery antiforgery = httpContext.RequestServices.GetRequiredService<IAntiforgery>();
        await antiforgery.ValidateRequestAsync(httpContext);
    }
    await next();
});

app.UseCors(c => c.WithOrigins("http://webapi").AllowAnyHeader().AllowAnyMethod());

app.UseIdentityServer();

app.MapGet("/", () => $"Identity {DateTime.UtcNow}");

app.MapGet("/users", async (UserManager<AppUser> userManager) =>
{
    return await userManager.Users.ToListAsync();
});

app.MapGet("/roles", async (RoleManager<AppRole> roleManager) =>
{
    return await roleManager.Roles.ToListAsync();
});

app.MapGet("/auth/login", async (HttpContext httpContext, IAntiforgery antiforgery) =>
{
    var tokens = antiforgery.GetAndStoreTokens(httpContext);
    if (tokens.RequestToken is not null)
        httpContext.Response.Cookies.Append("X-XSRF-REQUEST-TOKEN", tokens.RequestToken, new CookieOptions
        {
            HttpOnly = false
        });

    return Results.Content(await File.ReadAllTextAsync("./wwwroot/login.html"),
        MediaTypeHeaderValue.Parse(System.Net.Mime.MediaTypeNames.Text.Html));
});

app.MapPost("/auth/login", Login);

app.MapPost("/auth/register", Register);

await app.RunAsync();

static async Task<IResult> Login(UserLoginDto loginDto, [FromQuery] string returnUrl,
    IIdentityServerInteractionService interaction, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
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
        else if (string.IsNullOrEmpty(returnUrl))
        {
            return Results.LocalRedirect("~/");
        }
        else
        {
            return Results.BadRequest("Invalid return URL");
        }
    }

    return Results.BadRequest(signInResult);
}

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