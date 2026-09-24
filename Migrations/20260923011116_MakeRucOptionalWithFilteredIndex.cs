using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class MakeRucOptionalWithFilteredIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"CompanyProfiles\" ALTER COLUMN \"Ruc\" DROP NOT NULL;");

            migrationBuilder.DropIndex(
                name: "IX_CompanyProfiles_Ruc",
                table: "CompanyProfiles");

            migrationBuilder.Sql("UPDATE \"CompanyProfiles\" SET \"Ruc\" = NULL WHERE \"Ruc\" = '' OR TRIM(\"Ruc\") = '';");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfiles_Ruc",
                table: "CompanyProfiles",
                column: "Ruc",
                unique: true,
                filter: "\"Ruc\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyProfiles_Ruc",
                table: "CompanyProfiles");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfiles_Ruc",
                table: "CompanyProfiles",
                column: "Ruc",
                unique: true);
        }
    }
}
