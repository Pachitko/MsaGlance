using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Identity.Api;
using Serilog;
using System;
using Identity.Api.Domain.Entities;
using Identity.Api.Data;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo
    .Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services
    .AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("DB"))
    .AddIdentity<AppUser, AppRole>(c =>
    {
        c.User.RequireUniqueEmail = true;

        c.Password.RequiredLength = 8;
        c.Password.RequireDigit = false;
        c.Password.RequireNonAlphanumeric = false;
        c.Password.RequireUppercase = false;
        c.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services
    .AddIdentityServer()
    .AddInMemoryApiResources(Configuration.ApiResources)
    .AddInMemoryClients(Configuration.Clients)
    .AddInMemoryIdentityResources(Configuration.IdentityResources)
    .AddInMemoryApiScopes(Configuration.IdentityApiScopes)
    .AddDeveloperSigningCredential();

// Pipeline
var app = builder.Build();
await app.Services.CreateScope().ServiceProvider.SeedDataAsync();

app.UseIdentityServer();

app.MapGet("/", () =>
{
    DateTime now = DateTime.UtcNow;
    return $"Identity {now}";
});

app.Map("/users", async (UserManager<AppUser> userManager) =>
{
    return await userManager.Users.ToListAsync();
});

app.Map("/roles", async (RoleManager<AppRole> roleManager) =>
{
    return await roleManager.Roles.ToListAsync();
});

await app.RunAsync();
