namespace Backend.Src.Domain.Ports.Profiles;

public interface IRucValidationPort
{
    public Task<bool> ValidateRucAsync(string ruc);
}