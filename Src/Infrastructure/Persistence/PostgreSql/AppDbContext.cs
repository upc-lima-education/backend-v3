using Backend.Src.Domain.Entities.Auth;
using Backend.Src.Domain.Entities.Curriculums;
using Backend.Src.Domain.Entities.Jobs;
using Backend.Src.Domain.Entities.Notifications;
using Backend.Src.Domain.Entities.Payments;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Entities.Recruitment;
using Backend.Src.Domain.Entities.Recommendation;
using Backend.Src.Domain.Entities.Skills;
using Microsoft.EntityFrameworkCore;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql;

/// <summary>
/// Handles the Postgres Entities
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // Auth
    public DbSet<User> Users => Set<User>();

    // Profiles
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<LanguageKnown> LanguageKnowns => Set<LanguageKnown>();

    // Jobs
    public DbSet<Job> Jobs => Set<Job>();

    //Skills
    public DbSet<Skill> Skills => Set<Skill>();

    // Message
    //Handled in MongoDb

    // Curriculums
    public DbSet<Cv> Cvs => Set<Cv>();
    // Cv documents handled in MongoDB
    // Only references and metadata are handled in Postgres

    // Payments
    public DbSet<Payment> Payments => Set<Payment>();

    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();

    // Recruitment
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();

    // Recommendation
    public DbSet<JobInteraction> JobInteractions => Set<JobInteraction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<JobInteraction>(entity =>
        {
            entity.ToTable("JobInteractions");
            entity.HasKey(interaction => interaction.Id);
            entity.HasIndex(interaction => interaction.CandidateProfileId);
            entity.HasIndex(interaction => interaction.JobId);
            entity.HasIndex(interaction => new { interaction.CandidateProfileId, interaction.JobId });
            entity.Property(interaction => interaction.Type).HasConversion<string>();
            entity.Property(interaction => interaction.CreatedAt).IsRequired();
        });
    }
}
