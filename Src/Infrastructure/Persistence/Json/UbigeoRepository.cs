using System.Text.Json;
using Backend.Src.Domain.Models;
using Backend.Src.Domain.Repositories.Common;

namespace Backend.Src.Infrastructure.Persistence.Json;

public class UbigeoRepository : IUbigeoRepository
{
    private readonly Dictionary<string, Ubigeo> _ubigeos;

    public UbigeoRepository()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Json", "Ubigeo.json");
        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<List<UbigeoJson>>(json) ?? [];

        _ubigeos = data.ToDictionary(
            x => x.Ubigeo,
            x => new Ubigeo(
                x.Ubigeo,
                x.Departamento,
                x.Provincia,
                x.Distrito
            )
        );
    }

    public Task<Ubigeo?> GetByCodeAsync(string code)
    {
        _ubigeos.TryGetValue(code, out var ubigeo);
        return Task.FromResult(ubigeo);
    }

    private sealed record UbigeoJson(
        string Departamento,
        string Provincia,
        string Distrito,
        string Ubigeo
    );
}