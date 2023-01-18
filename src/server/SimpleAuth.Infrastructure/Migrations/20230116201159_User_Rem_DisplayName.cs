using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleAuth.Infrastructure.Migrations
{
    public partial class User_Rem_DisplayName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "auth",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "auth",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
