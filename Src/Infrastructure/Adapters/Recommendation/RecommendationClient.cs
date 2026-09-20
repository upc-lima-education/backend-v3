using System.Net.Http.Json;
using System.Web;
using System.Text.Json.Serialization;
using Backend.Src.Application.Dtos.Requests.Recommendation;
using Backend.Src.Infrastructure.Options.Recommendation;
using Microsoft.Extensions.Options;

namespace Backend.Src.Infrastructure.Adapters.Recommendation;

public sealed record AlsRecommendationItem([property: JsonPropertyName("job_id")] string JobId, double Score);

public sealed record CbfRecommendationPage(
    [property: JsonPropertyName("items")] IReadOnlyList<AlsRecommendationItem> Items,
    [property: JsonPropertyName("total_items")] int TotalItems,
    [property: JsonPropertyName("page")] int Page,
    [property: JsonPropertyName("page_size")] int PageSize);

public interface IRecommendationClient
{
    Task<IReadOnlyList<AlsRecommendationItem>> GetAlsRecommendationsAsync(
        string candidateProfileId,
        int limit,
        CancellationToken cancellationToken);

    Task<CbfRecommendationPage> GetCbfRecommendationsAsync(
        SearchRecommendationsRequest request,
        CancellationToken cancellationToken);

    Task<bool> CheckHealthAsync(CancellationToken cancellationToken);
}

public sealed class RecommendationClient(HttpClient httpClient, IOptions<RecommendationOptions> options) : IRecommendationClient
{
    public async Task<IReadOnlyList<AlsRecommendationItem>> GetAlsRecommendationsAsync(
        string candidateProfileId,
        int limit,
        CancellationToken cancellationToken)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["candidateProfileId"] = candidateProfileId;
        query["limit"] = limit.ToString(System.Globalization.CultureInfo.InvariantCulture);
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"api/v1/recommendations/personalized?{query}");
        if (!string.IsNullOrWhiteSpace(options.Value.ApiKey))
            requestMessage.Headers.TryAddWithoutValidation("X-Recommendation-Key", options.Value.ApiKey);
        using var response = await httpClient.SendAsync(requestMessage, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<AlsRecommendationItem>>(cancellationToken: cancellationToken) ?? [];
    }

    public async Task<CbfRecommendationPage> GetCbfRecommendationsAsync(
        SearchRecommendationsRequest request,
        CancellationToken cancellationToken)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["query"] = request.Query.Trim();
        query["page"] = request.Page.ToString(System.Globalization.CultureInfo.InvariantCulture);
        query["page_size"] = request.PageSize.ToString(System.Globalization.CultureInfo.InvariantCulture);
        AddOptionalQuery(query, "ubigeo", request.Ubigeo);
        AddOptionalQuery(query, "educationLevel", request.EducationLevel);
        AddOptionalQuery(query, "experience", request.Experience);
        AddOptionalQuery(query, "workHours", request.WorkHours);
        AddOptionalQuery(query, "jobType", request.JobType);
        if (request.MinSalary.HasValue)
            query["minSalary"] = request.MinSalary.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"api/v1/recommendations/search?{query}");
        if (!string.IsNullOrWhiteSpace(options.Value.ApiKey))
            requestMessage.Headers.TryAddWithoutValidation("X-Recommendation-Key", options.Value.ApiKey);
        using var response = await httpClient.SendAsync(requestMessage, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CbfRecommendationPage>(cancellationToken: cancellationToken)
            ?? new CbfRecommendationPage([], 0, request.Page, request.PageSize);
    }

    public async Task<bool> CheckHealthAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.GetAsync("health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private static void AddOptionalQuery(System.Collections.Specialized.NameValueCollection query, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value)) query[key] = value.Trim();
    }
}
