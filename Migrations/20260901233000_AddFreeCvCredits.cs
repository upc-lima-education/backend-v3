using System;
using Backend.Src.Infrastructure.Persistence.PostgreSql;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260901233000_AddFreeCvCredits")]
    public partial class AddFreeCvCredits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CreditBalance",
                table: "Users",
                type: "NUMERIC",
                nullable: false,
                defaultValue: 3m,
                oldClrType: typeof(decimal),
                oldType: "NUMERIC",
                oldDefaultValue: 2m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CreditBalance",
                table: "Users",
                type: "NUMERIC",
                nullable: false,
                defaultValue: 2m,
                oldClrType: typeof(decimal),
                oldType: "NUMERIC",
                oldDefaultValue: 3m);
        }
    }
}
