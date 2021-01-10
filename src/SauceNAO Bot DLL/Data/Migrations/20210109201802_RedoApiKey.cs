using Microsoft.EntityFrameworkCore.Migrations;

namespace SauceNAO.Data.Migrations
{
    public partial class RedoApiKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APIKEY",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "APIKEY",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }
    }
}
