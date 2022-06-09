using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Host.UseSerilog();
Log.Logger = new LoggerConfiguration()
    .WriteTo
    .Console()
    .CreateLogger();

var app = builder.Build();

app.MapGet("/", () =>
{
    DateTime now = DateTime.UtcNow;
    return $"Web {now}";
});

app.Map("/files", GetFilesAsync);

app.Run();

static async Task<IResult> GetFilesAsync(IHttpClientFactory httpClientFactory, ILogger<Program> logger)
{
    var authClient = httpClientFactory.CreateClient();
    DiscoveryDocumentRequest discoveryDocumentRequest = new()
    {
        Address = "http://identity",
        Policy = new DiscoveryPolicy()
        {
            RequireHttps = false
        }
    };
    var discoveryDocument = await authClient.GetDiscoveryDocumentAsync(discoveryDocumentRequest);

    var tokenResponse = await authClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
    {
        Address = discoveryDocument.TokenEndpoint,
        ClientId = "client_id",
        ClientSecret = "client_secret",
        Scope = "DiskApi"
    });

    var diskClient = httpClientFactory.CreateClient();
    diskClient.SetBearerToken(tokenResponse.AccessToken);
    var response = await diskClient.GetAsync("http://disk/secret");

    if (response.IsSuccessStatusCode)
    {
        return Results.Ok(await response.Content.ReadAsStringAsync());
    }
    else
    {
        return Results.StatusCode((int)response.StatusCode);
    }
}