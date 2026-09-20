using Backend.Src.Domain.Entities.Jobs;
using Backend.Src.Domain.Entities.Recruitment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.EntityConfiguration;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> entity)
    {
        entity.ToTable("JobApplications");
        
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasColumnType("UUID");
        entity.Property(e => e.JobId)
            .IsRequired()
            .HasColumnType("UUID");
        entity.Property(e => e.CandidateId)
            .IsRequired()
            .HasColumnType("UUID");
        entity.Property(e => e.CvStorageKey)
            .IsRequired()
            .HasColumnType("TEXT");
        entity.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("TEXT");
        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnType("TIMESTAMPTZ");
        entity.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasColumnType("TIMESTAMPTZ");

        entity.HasIndex(e => new { e.JobId, e.CandidateId })
            .IsUnique();

        entity.HasOne<Job>()
            .WithMany()
            .HasForeignKey(e => e.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
