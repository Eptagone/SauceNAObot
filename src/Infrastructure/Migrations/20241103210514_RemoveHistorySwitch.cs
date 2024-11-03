using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SauceNAO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHistorySwitch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaveRecentSauces",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SaveRecentSauces",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
