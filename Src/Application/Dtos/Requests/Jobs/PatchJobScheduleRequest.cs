namespace Backend.Src.Application.Dtos.Requests.Jobs;

public record PatchJobScheduleRequest(
    DateTime OpensAt,
    DateTime ClosesAt
);
