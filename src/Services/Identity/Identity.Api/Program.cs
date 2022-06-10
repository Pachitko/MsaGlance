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
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Identity.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Identity.Api.Options;
using Microsoft.Extensions.Configuration;
using System.Linq;

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

app.MapPost("/login", Login);

await app.RunAsync();

static async Task<IResult> Login([FromBody] UserLoginDto userLogin, UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager, IValidator<UserLoginDto> validator)
{
    ValidationResult validationResult = validator.Validate(userLogin);

    if (validationResult.IsValid)
    {
        var user = await userManager.FindByNameAsync(userLogin.Username);
        if (user is null)
        {
            return Results.NotFound("User not found");
        }
        // var signInResult = await signInManager.PasswordSignInAsync(userLogin.Username, userLogin.Password, false, false);
        return Results.Ok(user);
    }
    else
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
}