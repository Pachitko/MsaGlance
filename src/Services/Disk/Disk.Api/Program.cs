using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => $"Disk {DateTime.UtcNow}");

app.MapGet("/secret", (HttpContext ctx) =>
{
    return "Disk secret";
})
    .RequireAuthorization();

app.Run();