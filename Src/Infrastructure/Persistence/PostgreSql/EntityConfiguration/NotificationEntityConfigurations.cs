using Backend.Src.Domain.Entities.Auth;
using Backend.Src.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.EntityConfiguration;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> entity)
    {
        entity.ToTable("Notifications");
        
        entity.HasKey(n => n.Id);
        entity.Property(n => n.Id)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasColumnType("UUID");
        entity.Property(n => n.UserId)
            .IsRequired()
            .HasColumnType("UUID");
        entity.Property(n => n.Message)
            .IsRequired()
            .HasColumnType("TEXT");
        entity.Property(n => n.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("TEXT");
        entity.Property(n => n.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("TEXT");
        entity.Property(n => n.CreatedAt)
            .IsRequired()
            .HasColumnType("TIMESTAMPTZ");
        entity.Property(n => n.SentAt)
            .HasColumnType("TIMESTAMPTZ");

        entity.HasOne<User>()
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(n => n.UserId);
        entity.HasIndex(n => n.CreatedAt);
    }
}