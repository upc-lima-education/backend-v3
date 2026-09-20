using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddJobInteractions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobInteractions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobInteractions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobInteractions_CandidateProfileId",
                table: "JobInteractions",
                column: "CandidateProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_JobInteractions_CandidateProfileId_JobId",
                table: "JobInteractions",
                columns: new[] { "CandidateProfileId", "JobId" });

            migrationBuilder.CreateIndex(
                name: "IX_JobInteractions_JobId",
                table: "JobInteractions",
                column: "JobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobInteractions");
        }
    }
}
