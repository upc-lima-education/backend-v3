using Backend.Src.Domain.Entities.Auth;
using Backend.Src.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.EntityConfiguration;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> entity)
    {
        entity.ToTable("Payments");

        entity.HasKey(e => e.OrderId);

        entity.Property(e => e.OrderId)
            .HasColumnType("TEXT");

        entity.Property(e => e.UserId)
            .HasColumnType("UUID");

        entity.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.Property(e => e.CreditPlan)
            .HasConversion<string>()
            .HasColumnType("TEXT");

        entity.Property(e => e.Status)
            .HasConversion<string>()
            .HasColumnType("TEXT");

        entity.Property(e => e.Platform)
            .HasConversion<string>()
            .HasColumnType("TEXT");

        entity.Property(e => e.TransactionId)
            .HasColumnType("TEXT");

        entity.Property(e => e.CreatedAt)
            .HasColumnType("TIMESTAMPTZ");

        entity.Property(e => e.CompletedAt)
            .HasColumnType("TIMESTAMPTZ");

        entity.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_Payments_UserId");
    }
}
