using System.Net.Http.Headers;
using Backend.Src.Domain.Ports.Profiles;
using Backend.Src.Infrastructure.Contracts.Profiles.Decolecta;
using Backend.Src.Infrastructure.Options.Profiles;
using Microsoft.Extensions.Options;

namespace Backend.Src.Infrastructure.Adapters.Profiles.Decolecta;

public class DecolectaSunatRucValidatorAdapter(
    HttpClient http,
    IOptions<DecolectaOptions> options
) : IRucValidationPort
{
    private readonly DecolectaOptions _options = options.Value;
    public async Task<bool> ValidateRucAsync(string ruc)
    {
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        var response = await http.GetAsync($"{_options.BaseUrl}{ruc}");
        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<DecolectaSunatResponse>();
        if (result is null) return false;

        return result.Ruc == ruc && string.Equals(result.State, "ACTIVO", StringComparison.OrdinalIgnoreCase);
    }
}