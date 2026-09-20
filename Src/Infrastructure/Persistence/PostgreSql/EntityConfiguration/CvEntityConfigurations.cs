using Backend.Src.Domain.Entities.Auth;
using Backend.Src.Domain.Entities.Curriculums;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Rules.Curriculums;
using Backend.Src.Domain.ValueObjects.Curriculums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.EntityConfiguration;

public class CvConfiguration : IEntityTypeConfiguration<Cv>
{
    public void Configure(EntityTypeBuilder<Cv> entity)
    {
        entity.ToTable("Cvs");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnType("UUID");

        entity.Property(e => e.CandidateId)
            .HasColumnType("UUID");

        entity.HasOne<CandidateProfile>()
            .WithMany()
            .HasForeignKey(e => e.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.Property(e => e.Title)
            .HasColumnType("TEXT")
            .HasMaxLength(CvRules.MaxTitleLength);

        entity.Property(e => e.CreatedAt)
            .HasColumnType("TIMESTAMPTZ");

        entity.Property(e => e.UpdatedAt)
            .HasColumnType("TIMESTAMPTZ");

        entity.Property(e => e.IsCurrent)
            .HasColumnType("BOOLEAN");

        entity.Property(e => e.ProcessingStatus)
            .HasConversion<string>()
            .HasColumnType("TEXT")
            .HasMaxLength(32)
            .HasDefaultValue(CvProcessingStatus.Ready);

        entity.Property(e => e.ProcessingError)
            .HasColumnType("TEXT")
            .HasMaxLength(500);

        entity.HasIndex(e => e.CandidateId);

        entity.HasIndex(e => new { e.CandidateId, e.IsCurrent })
            .HasFilter("\"IsCurrent\" = true")
            .IsUnique();
    }
}
