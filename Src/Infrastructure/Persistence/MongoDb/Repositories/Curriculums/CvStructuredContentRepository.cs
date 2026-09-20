using Backend.Src.Domain.Entities.Curriculums;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Infrastructure.Persistence.MongoDb.Mappers.Curriculums;
using MongoDB.Driver;

namespace Backend.Src.Infrastructure.Persistence.MongoDb.Repositories.Curriculums;

public class CvStructuredContentRepository(MongoDbContext context) : ICvStructuredContentRepository
{
    public async Task CreateAsync(CvStructuredContent cvStructuredContent)
    {
        var document = CvDocumentMapper.ToDocument(cvStructuredContent);
        await context.CvDocuments.InsertOneAsync(document);
    }

    public async Task UpdateAsync(CvStructuredContent cvStructuredContent)
    {
        var document = CvDocumentMapper.ToDocument(cvStructuredContent);
        await context.CvDocuments.ReplaceOneAsync(
            cv => cv.Id == cvStructuredContent.Id,
            document
        );
    }

    public async Task<CvStructuredContent?> GetByIdAsync(Guid id)
    {
        var document = await context.CvDocuments
            .Find(cv => cv.Id == id)
            .FirstOrDefaultAsync();
        if (document is null) return null;

        return CvDocumentMapper.ToDomainEntity(document);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await context.CvDocuments.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }
}