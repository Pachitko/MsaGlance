using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Identity.Api;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddIdentityServer()
    .AddInMemoryApiResources(Configuration.GetApiResources())
    .AddInMemoryClients(Configuration.GetClients())
    .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
    .AddDeveloperSigningCredential();

Log.Logger = new LoggerConfiguration()
    .WriteTo
    .Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Pipeline
var app = builder.Build();

app.UseIdentityServer();

app.MapGet("/", () =>
{
    DateTime now = DateTime.UtcNow;
    return $"Identity {now}";
});

await app.RunAsync();
