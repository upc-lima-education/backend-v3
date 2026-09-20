using Backend.Src.Domain.Entities.Auth;
using Backend.Src.Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.EntityConfiguration;

public sealed class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
{
    public void Configure(EntityTypeBuilder<CompanyProfile> entity)
    {
        entity.ToTable("CompanyProfiles");

        entity.HasKey(o => o.ProfileId);

        entity.Property(o => o.ProfileId)
            .HasColumnType("UUID")
            .IsRequired();

        entity.Property(o => o.CompanyName)
            .HasColumnType("TEXT")
            .IsRequired();

        entity.Property(o => o.Sector)
            .HasColumnType("TEXT");

        entity.Property(o => o.Ruc)
            .HasColumnType("TEXT");

        entity.Property(o => o.Website)
            .HasColumnType("TEXT");

        entity.Property(o => o.CompanySize)
            .HasColumnType("TEXT");

        entity.Property(o => o.IsVerified)
            .HasColumnType("boolean")
            .HasDefaultValue(false);

        entity.Property(o => o.VerifiedAt)
            .HasColumnType("TIMESTAMPTZ");

        entity.Property(o => o.VerifiedByUserId)
            .HasColumnType("UUID");

        entity.Property(o => o.CreatedAt)
            .HasColumnType("TIMESTAMPTZ")
            .HasDefaultValueSql("now()");

        entity.Property(o => o.UpdatedAt)
            .HasColumnType("TIMESTAMPTZ")
            .HasDefaultValueSql("now()");

        entity.HasOne<User>()
            .WithMany()
            .HasForeignKey(o => o.VerifiedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasIndex(o => o.Ruc)
            .IsUnique();
    }
}
