namespace Backend.Src.Application.Dtos.Requests.Auth;

/// <summary>Editable contact data. Email remains immutable.</summary>
public record UpdateContactRequest(string? PhoneNumber);
