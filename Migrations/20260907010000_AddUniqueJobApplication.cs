using Backend.Src.Infrastructure.Persistence.PostgreSql;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260907010000_AddUniqueJobApplication")]
public partial class AddUniqueJobApplication : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_JobApplications_JobId_CandidateId",
            table: "JobApplications",
            columns: new[] { "JobId", "CandidateId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_JobApplications_JobId_CandidateId",
            table: "JobApplications");
    }
}
