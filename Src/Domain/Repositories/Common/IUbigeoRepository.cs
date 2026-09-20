using Backend.Src.Domain.Models;

namespace Backend.Src.Domain.Repositories.Common;

public interface IUbigeoRepository
{
    Task<Ubigeo?> GetByCodeAsync(string code);
}