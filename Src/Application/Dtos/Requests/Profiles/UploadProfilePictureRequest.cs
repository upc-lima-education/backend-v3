using Backend.Src.Application.Dtos.Requests.Common;

namespace Backend.Src.Application.Dtos.Requests.Profiles;

public record UploadProfilePictureRequest(UploadFileRequest File);