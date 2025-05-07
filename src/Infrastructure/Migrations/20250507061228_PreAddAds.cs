using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SauceNAO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PreAddAds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SponsorMessage",
                table: "ApiKeys",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SponsorMessage",
                table: "ApiKeys");
        }
    }
}
