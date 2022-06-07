using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo
    .Console()
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Disk started");

app.MapGet("/", () => "Disk");

app.Map("/files", GetFilesAsync);

app.Run();

static async Task<IResult> GetFilesAsync()
{
    await Task.Delay(100);
    return Results.Ok("Files:");
}