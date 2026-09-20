namespace Backend.Src.Application.Dtos.Responses.Jobs;

public record SyncScrapedJobsResponse(int JobsSynced, int JobsSkipped);