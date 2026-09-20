using Backend.Src.Domain.Models;
using Backend.Src.Domain.Repositories.Common;

namespace Backend.Src.Application.Resolvers.Common;

public class UbigeoResolver(IUbigeoRepository ubigeoRepository)
{
    public async Task<Ubigeo?> ResolveAsync(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        return await ubigeoRepository.GetByCodeAsync(code);
    }
}