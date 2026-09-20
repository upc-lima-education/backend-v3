using Backend.Src.Domain.Entities.Curriculums;

namespace Backend.Src.Domain.Repositories.Curriculums;

public interface ICvStructuredContentRepository
{
    Task CreateAsync(CvStructuredContent cvStructuredContent);
    Task UpdateAsync(CvStructuredContent cvStructuredContent);
    Task<CvStructuredContent?> GetByIdAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
}