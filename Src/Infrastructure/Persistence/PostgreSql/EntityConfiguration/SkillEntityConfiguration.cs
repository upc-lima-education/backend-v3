using Backend.Src.Domain.Entities.Skills;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.EntityConfiguration;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> entity)
    {
        entity.ToTable("Skills");
        
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasColumnType("UUID");

        entity.Property(e => e.Name)
            .IsRequired()
            .HasColumnType("TEXT");
        entity.HasIndex(e => e.Name)
            .IsUnique();
    }
}