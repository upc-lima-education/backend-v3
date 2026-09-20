using Backend.Src.Api.Rest.Dtos.Curriculums;
using Backend.Src.Application.Dtos.Data.Curriculums;
using Backend.Src.Application.Dtos.Requests.Common;
using Backend.Src.Application.Dtos.Requests.Curriculums;

namespace Backend.Src.Api.Rest.Mappers.Curriculums;

public static class CreateCvUploadedContentRequestMapper
{
    public static CreateCvUploadedContentRequest ToApplicationRequest(CreateCvUploadedContentApiRequest request)
    {
        var stream = request.Cv.OpenReadStream();
        var Cv = new CvUploadedContentData(
            new UploadFileRequest(
                stream,
                request.Cv.FileName,
                request.Cv.ContentType
            )
        );

        return new CreateCvUploadedContentRequest(
            request.Title,
            request.IsCurrent,
            Cv
        );
    }
}