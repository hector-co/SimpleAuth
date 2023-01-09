using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleAuth.Infrastructure.Migrations
{
    public partial class Role_AssignByDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DefaultRole",
                schema: "auth",
                table: "Roles",
                newName: "AssignByDefault");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignByDefault",
                schema: "auth",
                table: "Roles",
                newName: "DefaultRole");
        }
    }
}
