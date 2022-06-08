using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
    {
        c.RequireHttpsMetadata = builder.Environment.IsProduction();
        c.Authority = "http://identity:5000";
        c.Audience = "DiskAPI";
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

app.Map("/files", GetFilesAsync)
    .RequireAuthorization();

app.Run();

static async Task<IResult> GetFilesAsync(IHttpClientFactory httpClientFactory, ILogger<Program> logger)
{
    var authClient = httpClientFactory.CreateClient();
    DiscoveryDocumentRequest discoveryDocumentRequest = new()
    {
        Address = "http://identity:5000",
        Policy = new DiscoveryPolicy()
        {
            RequireHttps = false
        }
    };
    var discoveryDocument = await authClient.GetDiscoveryDocumentAsync(discoveryDocumentRequest);

    return Results.Ok(new
    {
        authorizeEndpoint = discoveryDocument.AuthorizeEndpoint
    });
}