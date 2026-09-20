using Backend.Src.Domain.Exceptions.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Middleware;

/// <summary>
/// Handles the exceptions <br/>
/// Fluent Validation Exceptions: HTTP 400 <br/>
/// Custom Exceptions: Defined in each exception <br/>
/// </summary>
public sealed class ExceptionsMiddleware(
    RequestDelegate next,
    ILogger<ExceptionsMiddleware> logger
)
{
    private const string ContentType = "application/problem+json";
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            var problem = new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title  = "Validation failed"
            };

            context.Response.StatusCode  = StatusCodes.Status400BadRequest;
            context.Response.ContentType = ContentType;
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (HttpRequestException)
        {
            var isRecommendation = context.Request.Path.StartsWithSegments("/api/v1/recommendation", StringComparison.OrdinalIgnoreCase);
            var title = isRecommendation ? "Recommendation service unavailable" : "External service unavailable";
            var detail = isRecommendation
                ? "The recommendation service is temporarily unavailable."
                : "An external service required for this request is temporarily unavailable.";

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status503ServiceUnavailable,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = ContentType;
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (DomainException ex)
        {
            var problem = new ProblemDetails()
            {
                Status = ex.StatusCode,
                Title = ex.Title,
                Detail = ex.Message,
                Instance = context.Request.Path
            };
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = ContentType;
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled error for {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier
            );

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unexpected server error",
                Detail = "No se pudo completar la solicitud. Inténtalo nuevamente.",
                Instance = context.Request.Path
            };
            problem.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = ContentType;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
