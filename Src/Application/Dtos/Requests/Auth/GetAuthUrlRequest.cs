using Backend.Src.Application.Dtos.Enums.Profiles;

namespace Backend.Src.Application.Dtos.Requests.Auth;

public record GetAuthUrlRequest(
    ProfileType ProfileType
);