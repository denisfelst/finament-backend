using System.Net;
using System.Text.Json;
using Finament.Application.Exceptions;

namespace Finament.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(
        HttpContext context,
        Exception exception
    )
    {
        var statusCode = exception switch
        {
            ValidationException      => HttpStatusCode.BadRequest,   // 400
            NotFoundException        => HttpStatusCode.NotFound,      // 404
            BusinessRuleException    => HttpStatusCode.Conflict,      // 409
            _                        => HttpStatusCode.InternalServerError
        };

        var response = new
        {
            message = exception.Message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response);

        return context.Response.WriteAsync(json);
    }
}