using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Chit.Utilities;

public class ChitExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ChitExceptionHandler> _logger;

    public ChitExceptionHandler(RequestDelegate next, ILogger<ChitExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionMessageAsync(context, ex).ConfigureAwait(false);
        }
    }

    private static Task HandleExceptionMessageAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        int statusCode = (int)HttpStatusCode.InternalServerError;
        var errorMessages = new List<string>();
        foreach (var exceptionMessage in exception.Message.Split('\n'))
        {
            errorMessages.Add(exceptionMessage);
        }
        // add messages from inner exceptions
        var innerException = exception.InnerException;
        while (innerException != null)
        {
            foreach (var exceptionMessage in innerException.Message.Split('\n'))
            {
                errorMessages.Add(exceptionMessage);
            }
            innerException = innerException.InnerException;
        }

        var result = JsonConvert.SerializeObject(new ChitStandardResponse<dynamic>()
        {
            StatusCode = (HttpStatusCode)statusCode,
            Message = exception.Message,
            Errors = errorMessages
        });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsync(result);
    }
}

public static class ExceptionHandlerExtensions
{
    public static void UseChitExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ChitExceptionHandler>();
    }
}

