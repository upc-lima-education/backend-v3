using System.Text.Json;
using System.Text.Json.Nodes;
using Backend.Src.Domain.Contracts.Curriculums;
using Backend.Src.Domain.Ports.Curriculums;
using Backend.Src.Infrastructure.Contracts.Curriculums.OpenRouter;
using Backend.Src.Infrastructure.Options.Curriculums;
using Backend.Src.Infrastructure.Resources.Prompts.Curriculums;
using Microsoft.Extensions.Options;

namespace Backend.Src.Infrastructure.Adapters.Curriculums.OpenRouter;

public class OpenRouterCvAiAssistantAdapter(
    HttpClient http,
    IOptions<OpenRouterOptions> options
) : ICvAiAssistantPort
{
    private readonly OpenRouterOptions orOptions = options.Value;
    // Las dos operaciones tienen contratos de salida distintos. Reutilizar el
    // esquema de generación para una mejora hacía que OpenRouter no pudiera
    // devolver campos como Summary, Certification, Project o Award.
    private static readonly JsonNode GenerationSchema = OpenRouterCvSchemaFactory.Create<AiAssistedCvGenerationResponse>();
    private static readonly JsonNode ImprovementSchema = OpenRouterCvSchemaFactory.Create<AiAssistedCvImprovementResponse>();

    public async Task<AiAssistedCvGenerationResponse> GenerateAsync(AiAssistedCvGenerationRequest request, CancellationToken cancellationToken = default)
    {
        List<OpenRouterMessage> messages = [
            new OpenRouterMessage("system", CvDetailsPrompt.BasePrompt),
            new OpenRouterMessage("system", CvDetailsPrompt.GenerationPrompt),
            new OpenRouterMessage("user", CvProfileSnapshotPrompt.ToPrompt(request.UserData))
        ];
        if (request.JobData is not null)
            messages.Add(new OpenRouterMessage("user", CvJobPostingPrompt.ToPrompt(request.JobData)));
        var payload = await GetOpenRouterPayload(messages, GenerationSchema, cancellationToken);
        var response = Parse<AiAssistedCvGenerationResponse>(payload);
        return response;
    }

    public async Task<AiAssistedCvImprovementResponse> ImproveAsync(AiAssistedCvImprovementRequest request, CancellationToken cancellationToken = default)
    {
        List<OpenRouterMessage> messages = [
            new OpenRouterMessage("system", CvDetailsPrompt.BasePrompt),
            new OpenRouterMessage("system", CvDetailsPrompt.ImprovementPrompt),
            new OpenRouterMessage("user", CvProfileSnapshotPrompt.ToPrompt(request.UserData)),
            new OpenRouterMessage("user", CvDocumentPrompt.ToPrompt(request.CvData))
        ];
        if (request.JobData is not null)
            messages.Add(new OpenRouterMessage("user", CvJobPostingPrompt.ToPrompt(request.JobData)));
        var payload = await GetOpenRouterPayload(messages, ImprovementSchema, cancellationToken);
        var response = Parse<AiAssistedCvImprovementResponse>(payload);
        return response;
    }

    private static T Parse<T>(OpenRouterChatResponse response)
    {
        var content = response.Choices?.FirstOrDefault()?.Message?.Content;
        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException("OpenRouter returned an empty response.");
        var json = ExtractJson(content);
        return JsonSerializer.Deserialize<T>(json, OpenRouterCvSchemaFactory.JsonOptions)
            ?? throw new InvalidOperationException($"Unable to deserialize response to {typeof(T).Name}.");
    }

    private static string ExtractJson(string content)
    {
        var first = content.IndexOf('{');
        var last = content.LastIndexOf('}');

        if (first == -1 || last == -1 || last < first)
            throw new InvalidOperationException("AI response does not contain a JSON object.");

        return content[first..(last + 1)];
    }

    private async Task<OpenRouterChatResponse> GetOpenRouterPayload(
        List<OpenRouterMessage> messages,
        JsonNode schema,
        CancellationToken cancellationToken
    )
    {
        var body = new OpenRouterChatRequest(
            orOptions.Model,
            messages,
            new OpenRouterResponseFormat(
                "json_schema", //type
                new OpenRouterJsonSchema(
                    "cv_document",
                    true, //strict
                    schema
                )
            ),
            orOptions.Temperature,
            orOptions.MaxTokens
        );

        using var openRouterResponse = await http.PostAsJsonAsync(
            "chat/completions",
            body,
            OpenRouterCvSchemaFactory.JsonOptions,
            cancellationToken
        );
        openRouterResponse.EnsureSuccessStatusCode();

        var rawResponse = await openRouterResponse.Content.ReadAsStringAsync(cancellationToken);
        var payload = await openRouterResponse.Content.ReadFromJsonAsync<OpenRouterChatResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException();
        return payload;
    }
}
