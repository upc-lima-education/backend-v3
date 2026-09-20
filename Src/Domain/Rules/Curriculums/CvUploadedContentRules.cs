namespace Backend.Src.Domain.Rules.Curriculums;

public static class CvUploadedContentRules
{
    public const long MaxFileSizeBytes = 2 * 1024 * 1024; // 2 MB
    public const int MinPages = 1;
    public const int MaxPages = 10;
    public const string PdfContentType = "application/pdf";
    public const string PdfExtension = ".pdf";
}
