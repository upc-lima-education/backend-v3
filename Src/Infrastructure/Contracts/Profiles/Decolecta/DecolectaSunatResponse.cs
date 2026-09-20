using System.Text.Json.Serialization;

namespace Backend.Src.Infrastructure.Contracts.Profiles.Decolecta;

public record DecolectaSunatResponse(
    [property: JsonPropertyName("numero_documento")]
    string Ruc,

    [property: JsonPropertyName("estado")]
    string State
);