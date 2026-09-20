using Backend.Src.Domain.Entities.Auth;
using Backend.Src.Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.EntityConfiguration;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> entity)
    {
        entity.ToTable("Profiles");
        
        entity.HasKey(p => p.Id);

        entity.Property(p => p.Id)
            .HasColumnType("UUID")
            .IsRequired();

        entity.Property(p => p.UserId)
            .HasColumnType("UUID")
            .IsRequired();

        entity.Property(p => p.Description)
            .HasColumnType("TEXT");

        entity.Property(p => p.Ubigeo)
            .HasColumnType("TEXT");

        entity.Property(p => p.ProfilePicture)
            .HasColumnType("TEXT");

        entity.Property(p => p.PhoneNumber)
            .HasColumnType("TEXT");

        entity.Property(p => p.CreatedAt)
            .HasColumnType("TIMESTAMPTZ")
            .HasDefaultValueSql("now()");

        entity.Property(p => p.UpdatedAt)
            .HasColumnType("TIMESTAMPTZ")
            .HasDefaultValueSql("now()");

        entity.HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(p => p.CandidateProfile)
            .WithOne(e => e.Profile)
            .HasForeignKey<CandidateProfile>(e => e.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(p => p.CompanyProfile)
            .WithOne(o => o.Profile)
            .HasForeignKey<CompanyProfile>(o => o.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(p => p.Skills)
            .WithMany(s => s.Profiles)
            .UsingEntity(j => j.ToTable("ProfileSkills"));

        entity.Navigation(p => p.Skills)
            .HasField("_skills");

        entity.HasIndex(p => p.UserId).IsUnique();
        entity.HasIndex(p => p.CreatedAt);
    }
}