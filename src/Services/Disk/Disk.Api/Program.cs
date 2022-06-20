using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Disk.Api;
using Disk.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
        options.Authority = builder.Configuration["Integration:IdentityServerEndpoint"];
        options.ApiName = "DiskApi";
    });

builder.Services.AddAuthorization(c =>
{
    c.AddPolicy("disk.api.read", b => b
        .RequireClaim("sub")
        .RequireScope("disk.api.read")
        .Build());
    c.AddPolicy("disk.api.write", b => b
        .RequireClaim("sub")
        .RequireScope("disk.api.write")
        .Build());
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => $"Disk {DateTime.UtcNow}");

app.MapGet("/files", (HttpContext httpContext) =>
{
    string userDirectoryPath = GetUserDirectoryPathOrCreate(httpContext);
    IEnumerable<string> fileNames = Directory.GetFiles(userDirectoryPath).Select(f => Path.GetFileName(f));
    fileNames = fileNames.OrderBy(x => x);
    return Results.Ok(fileNames);
}).RequireAuthorization("disk.api.read");

app.MapGet("/files/{fileName}", (string fileName, HttpContext httpContext) =>
{
    fileName = Path.GetFileName(fileName);
    string filePath = Path.Combine(GetUserDirectoryPathOrCreate(httpContext), fileName);
    if (File.Exists(filePath))
    {
        return Results.File(filePath);
    }
    else
    {
        return Results.NotFound("File not found");
    }
}).RequireAuthorization("disk.api.read");

app.MapPost("/files", async (HttpContext httpContext, FileDto fileDto) =>
{
    if (fileDto.File is null)
        return Results.BadRequest("File is empty");

    if (fileDto.File!.Length > 1024 * 1024 * 1)
        return Results.BadRequest("Max file size is 1 MB");

    string fileName = Path.GetFileName(fileDto.File.FileName);

    string filePath = Path.Combine(GetUserDirectoryPathOrCreate(httpContext), fileName);
    await using var fs = new FileStream(filePath, FileMode.Create);
    await fileDto.File.CopyToAsync(fs);

    return Results.Ok();
}).RequireAuthorization("disk.api.write");

await app.RunAsync();

static string GetUserDirectoryPathOrCreate(HttpContext httpContext)
{
    string userDirectoryPath = Path.Combine(Constants.FilesPath, httpContext.User.Claims.First(x => x.Type == "sub").Value);
    if (!Directory.Exists(userDirectoryPath))
        Directory.CreateDirectory(userDirectoryPath);

    return userDirectoryPath;
}