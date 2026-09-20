using Backend.Src.Infrastructure.Persistence.MongoDb.Documents.Conversations;
using Backend.Src.Infrastructure.Persistence.MongoDb.Documents.Curriculums;
using MongoDB.Driver;

namespace Backend.Src.Infrastructure.Persistence.MongoDb;

/// <summary>
/// Handles the MongoDb Entities
/// </summary>
public class MongoDbContext(IMongoDatabase database)
{
    public IMongoCollection<ConversationDocument> Conversations => database.GetCollection<ConversationDocument>("Conversations");
    public IMongoCollection<CvDocument> CvDocuments => database.GetCollection<CvDocument>("CvDocuments");
}