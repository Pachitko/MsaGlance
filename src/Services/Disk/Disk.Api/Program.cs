using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
    {
        c.RequireHttpsMetadata = builder.Environment.IsProduction();
        c.Authority = "http://identity";
        // c.Audience = "DiskApi";
        c.TokenValidationParameters = new()
        {
            ValidateAudience = false,
        };
    });

Log.Logger = new LoggerConfiguration()
    .WriteTo
    .Console()
    .CreateLogger();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () =>
{
    DateTime now = DateTime.UtcNow;
    return $"Disk {now}";
});

app.Map("/secret", (HttpContext ctx) =>
{
    return "Disk secret";
})
    .RequireAuthorization();

app.Run();