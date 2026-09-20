namespace Backend.Src.Domain.Contracts.Common;

public abstract record IntegrationMessage(
    Guid Id,
    DateTime CreatedAt
);