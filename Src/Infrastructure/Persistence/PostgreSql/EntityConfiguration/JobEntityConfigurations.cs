using Backend.Src.Domain.Entities.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.EntityConfiguration;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> entity)
    {
        entity.ToTable("Jobs");

        //Ids
        entity.HasKey(j => j.Id);
        entity.Property(e => e.Id)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasColumnType("UUID");
        entity.Property(e => e.CompanyId)
            .HasColumnType("UUID");
        //Details
        entity.Property(e => e.Title)
            .IsRequired()
            .HasColumnType("TEXT");
        entity.Property(e => e.Description)
            .IsRequired()
            .HasColumnType("TEXT");
        entity.Property(e => e.JobType)
            .IsRequired()
            .HasColumnType("TEXT")
            .HasConversion<string>();
        entity.Property(e => e.WorkHours)
            .IsRequired()
            .HasColumnType("TEXT")
            .HasConversion<string>();
        //Requirements
        entity.HasMany(j => j.Skills)
            .WithMany(s => s.Jobs)
            .UsingEntity(j => j.ToTable("JobSkills"));
        entity.Property(e => e.Experience)
            .IsRequired()
            .HasColumnType("TEXT")
            .HasConversion<string>();
        entity.Property(e => e.EducationLevel)
            .IsRequired()
            .HasColumnType("TEXT")
            .HasConversion<string>();
        //Location
        entity.Property(e => e.Ubigeo)
            .HasColumnType("TEXT");
        entity.Property(e => e.Address)
            .HasColumnType("TEXT");
        //Payment
        entity.Property(e => e.MinSalary)
            .HasColumnType("NUMERIC")
            .HasPrecision(10, 2);
        entity.Property(e => e.MaxSalary)
            .HasColumnType("NUMERIC")
            .HasPrecision(10, 2);
        entity.Property(e => e.Currency)
            .HasColumnType("TEXT")
            .HasConversion<string>();
        entity.Property(e => e.SalaryPeriod)
            .HasColumnType("TEXT")
            .HasConversion<string>();
        entity.Property(e => e.CompensationType)
            .HasColumnType("TEXT")
            .HasConversion<string>();
        //Traceability
        entity.Property(e => e.OpensAt)
            .IsRequired()
            .HasColumnType("TIMESTAMPTZ");
        entity.Property(e => e.ClosesAt)
            .IsRequired()
            .HasColumnType("TIMESTAMPTZ");
        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnType("TIMESTAMPTZ");
        entity.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasColumnType("TIMESTAMPTZ");
        entity.Property(e => e.OriginPage)
            .IsRequired()
            .HasColumnType("TEXT")
            .HasConversion<string>();
        entity.Property(e => e.Views)
            .IsRequired()
            .HasColumnType("INTEGER");
        //External
        entity.Property(e => e.ExternalCompanyName)
            .HasColumnType("TEXT");
        entity.Property(e => e.ExternalCompanyImage)
            .HasColumnType("TEXT");
        entity.Property(e => e.SourceUrl)
            .HasColumnType("TEXT");
        entity.Property(e => e.ApplyUrl)
            .HasColumnType("TEXT");

        entity.HasOne(e => e.Company)
            .WithMany()
            .HasForeignKey(e => e.CompanyId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
