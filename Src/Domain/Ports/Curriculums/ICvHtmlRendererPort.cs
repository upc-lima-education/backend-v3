using Backend.Src.Domain.Entities.Curriculums;

namespace Backend.Src.Domain.Ports.Curriculums;

public interface ICvHtmlRendererPort
{
    string Render(CvStructuredContent cv);
}