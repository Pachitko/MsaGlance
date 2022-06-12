using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo
    .Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(options =>
    {
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
        options.Authority = builder.Configuration["ApiResourceBaseUrls:AuthServer"];
        options.ApiName = "DiskApi";
    });

builder.Services.AddAuthorization(c =>
{
    c.AddPolicy("disk.api.read", b => b
        .RequireScope("disk.api.read")
        .Build());
    c.AddPolicy("disk.api.write", b => b
        .RequireScope("disk.api.write")
        .Build());
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => $"Disk {DateTime.UtcNow}");

app.MapGet("/secret", (HttpRequest s) =>
{
    return Results.Content("Disk secret");
})
    .RequireAuthorization("disk.api.read");

app.Run();