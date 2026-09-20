using Backend.Src.Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.EntityConfiguration;

public sealed class CandidateProfileConfiguration : IEntityTypeConfiguration<CandidateProfile>
{
    public void Configure(EntityTypeBuilder<CandidateProfile> entity)
    {
        entity.ToTable("CandidateProfiles");

        entity.HasKey(e => e.ProfileId);

        entity.Property(e => e.ProfileId)
            .HasColumnType("UUID")
            .IsRequired();

        entity.Property(e => e.FirstName)
            .HasColumnType("TEXT")
            .IsRequired();

        entity.Property(e => e.LastName)
            .HasColumnType("TEXT")
            .IsRequired();

        entity.Property(e => e.Dni)
            .HasColumnType("TEXT");

        entity.Property(e => e.CreatedAt)
            .HasColumnType("TIMESTAMPTZ")
            .HasDefaultValueSql("now()");

        entity.Property(e => e.UpdatedAt)
            .HasColumnType("TIMESTAMPTZ")
            .HasDefaultValueSql("now()");

        entity.HasMany(e => e.Languages)
            .WithOne()
            .HasForeignKey(e => e.CandidateProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.WorkExperiences)
            .WithOne()
            .HasForeignKey(e => e.CandidateProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.Educations)
            .WithOne()
            .HasForeignKey(e => e.CandidateProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.Navigation(e => e.Languages)
            .HasField("_languages");

        entity.Navigation(e => e.WorkExperiences)
            .HasField("_workExperiences");

        entity.Navigation(e => e.Educations)
            .HasField("_educations");
    }
}

public sealed class EducationConfiguration : IEntityTypeConfiguration<Education>
{
    public void Configure(EntityTypeBuilder<Education> entity)
    {
        entity.ToTable("Educations");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnType("UUID")
            .ValueGeneratedNever()
            .IsRequired();

        entity.Property(e => e.CandidateProfileId)
            .HasColumnType("UUID")
            .IsRequired();

        entity.Property(e => e.Institution)
            .HasColumnType("TEXT")
            .IsRequired();

        entity.Property(e => e.Degree)
            .HasColumnType("TEXT")
            .IsRequired();

        entity.Property(e => e.FieldOfStudy)
            .HasColumnType("TEXT");

        entity.Property(e => e.StartDate)
            .HasColumnType("DATE")
            .IsRequired();

        entity.Property(e => e.EndDate)
            .HasColumnType("DATE");

        entity.HasIndex(e => e.CandidateProfileId);
    }
}

public sealed class WorkExperienceConfiguration : IEntityTypeConfiguration<WorkExperience>
{
    public void Configure(EntityTypeBuilder<WorkExperience> entity)
    {
        entity.ToTable("WorkExperiences");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnType("UUID")
            .ValueGeneratedNever()
            .IsRequired();

        entity.Property(e => e.CandidateProfileId)
            .HasColumnType("UUID")
            .IsRequired();

        entity.Property(e => e.Company)
            .HasColumnType("TEXT")
            .IsRequired();

        entity.Property(e => e.Position)
            .HasColumnType("TEXT")
            .IsRequired();

        entity.Property(e => e.Description)
            .HasColumnType("TEXT");

        entity.Property(e => e.StartDate)
            .HasColumnType("DATE")
            .IsRequired();

        entity.Property(e => e.EndDate)
            .HasColumnType("DATE");

        entity.HasIndex(e => e.CandidateProfileId);
    }
}

public sealed class LanguageKnownConfiguration : IEntityTypeConfiguration<LanguageKnown>
{
    public void Configure(EntityTypeBuilder<LanguageKnown> entity)
    {
        entity.ToTable("Languages");

        entity.HasKey(x => new
        {
            x.CandidateProfileId,
            x.LanguageCode,
            x.LanguageLevel
        });

        entity.Property(x => x.CandidateProfileId)
            .HasColumnType("UUID")
            .ValueGeneratedNever()
            .IsRequired();

        entity.Property(x => x.LanguageCode)
            .HasConversion<string>()
            .HasMaxLength(2)
            .IsRequired();

        entity.Property(x => x.LanguageLevel)
            .HasConversion<string>()
            .HasMaxLength(2)
            .IsRequired();

    }
}