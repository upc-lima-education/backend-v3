using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.Src.Api.Rest.Filters.Jobs;

public class ScraperApiKeyFilter(IConfiguration configuration) : IActionFilter
{
    private const string Header = "X-Scraper-Key";

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var configured = configuration["Scraper:ApiKey"];
        if (string.IsNullOrWhiteSpace(configured))
        {
            context.Result = new StatusCodeResult(503);
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(Header, out var provided)
            || provided != configured)
        {
            context.Result = new UnauthorizedObjectResult(new { detail = "No autorizado. Token de integración inválido." });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
