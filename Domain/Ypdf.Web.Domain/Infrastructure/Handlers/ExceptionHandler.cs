using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Models.Api;
using Ypdf.Web.Domain.Models.Api.Exceptions;

namespace Ypdf.Web.Domain.Infrastructure.Handlers;

public static class ExceptionHandler
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    static ExceptionHandler()
    {
        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public static async Task HandleExceptionAsync(HttpContext context, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        string requestPath = context.Request.Path.Value ?? string.Empty;

        bool isApiError = requestPath.StartsWith(
            "/api/",
            StringComparison.InvariantCulture);

        if (isApiError)
        {
            await HandleApiExceptionAsync(context, logger)
                .ConfigureAwait(false);
        }
        else if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
        {
            HandleNotFoundError(context);
        }
        else
        {
            HandleUnknownError(context);
        }
    }

    public static async Task HandleApiExceptionAsync(HttpContext context, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        IExceptionHandlerPathFeature? exceptionFeature = context.Features
            .Get<IExceptionHandlerPathFeature>();

        Exception? exception = exceptionFeature?.Error;

        if (exception is not null)
            logger.LogError("Exception {@Exception} occurred", exception);

        ApiError apiError = exception switch
        {
            ApiException apiException => new(apiException.StatusCode, apiException.Message),
            _ => new(HttpStatusCode.InternalServerError, nameof(HttpStatusCode.InternalServerError))
        };

        context.Response.ContentType = "application/json";

        string response = JsonSerializer.Serialize(apiError, _jsonSerializerOptions);

        await context.Response
            .WriteAsync(response)
            .ConfigureAwait(false);
    }

    public static void HandleNotFoundError(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        context.Response.Redirect("/error/notfound");
    }

    public static void HandleUnknownError(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        context.Response.Redirect("/error");
    }
}
