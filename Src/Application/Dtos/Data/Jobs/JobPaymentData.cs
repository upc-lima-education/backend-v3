using Backend.Src.Domain.ValueObjects.Jobs;

namespace Backend.Src.Application.Dtos.Data.Jobs;

public record JobPaymentData(
    decimal? MinSalary,
    decimal? MaxSalary,
    Currency Currency,
    SalaryPeriod SalaryPeriod,
    CompensationType CompensationType
);