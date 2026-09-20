using Backend.Src.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.EntityConfiguration;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("Users");

        entity.HasKey(u => u.Id);

        entity.Property(u => u.Id)
            .HasColumnType("UUID")
            .IsRequired();

        entity.Property(u => u.Email)
            .HasColumnType("TEXT")
            .IsRequired();

        entity.Property(u => u.Password)
            .HasColumnType("TEXT");

        entity.Property(u => u.IsEmailVerified)
            .HasColumnType("BOOLEAN")
            .HasDefaultValue(false);

        entity.Property(u => u.IsActive)
            .HasColumnType("BOOLEAN")
            .HasDefaultValue(true);

        entity.Property(u => u.CreditBalance)
            .HasColumnType("NUMERIC")
            .HasDefaultValue(3);

        entity.Property(u => u.CreatedAt)
            .HasColumnType("TIMESTAMPTZ")
            .HasDefaultValueSql("now()");

        entity.Property(u => u.UpdatedAt)
            .HasColumnType("TIMESTAMPTZ")
            .HasDefaultValueSql("now()");

        entity.HasIndex(u => u.Email).IsUnique();
        entity.HasIndex(u => u.CreatedAt);
    }
}
