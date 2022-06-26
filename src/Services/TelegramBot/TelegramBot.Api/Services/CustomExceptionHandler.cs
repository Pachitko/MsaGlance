using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace TelegramBot.Api.Services;
public static class CustomExceptionHandler
{
    public static void HandleException(IApplicationBuilder app)
    {
        app.Run(async ctx =>
        {
            // ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            // ctx.Response.ContentType = "application/json";

            var exceptionHandler = ctx.Features.Get<ExceptionHandlerFeature>();
            ctx.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("Exception")
                .LogError(exceptionHandler?.Error, "Exception on path: \"{Path}\"", ctx.Request.Path);

            await Results.Ok().ExecuteAsync(ctx);
        });
    }
}