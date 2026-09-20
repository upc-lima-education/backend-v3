using Backend.Src.Api.Rest.Dtos.Recruitment;
using Backend.Src.Application.Dtos.Requests.Recruitment;

namespace Backend.Src.Api.Rest.Mappers.Recruitment;

public static class CreateJobApplicationRequestMapper
{
    public static CreateJobApplicationRequest ToApplicationRequest(CreateJobApplicationApiRequest request)
    {
        var stream = request.Cv.OpenReadStream();
        return new CreateJobApplicationRequest(
            request.JobId,
            stream,
            request.Cv.FileName,
            request.Cv.ContentType
        );
    }
}