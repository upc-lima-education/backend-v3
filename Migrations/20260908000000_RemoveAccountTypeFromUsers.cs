using Backend.Src.Infrastructure.Persistence.PostgreSql;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260908000000_RemoveAccountTypeFromUsers")]
public partial class RemoveAccountTypeFromUsers : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn("AccountType", "Users");

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<string>("AccountType", "Users", type: "text", nullable: true);
}
