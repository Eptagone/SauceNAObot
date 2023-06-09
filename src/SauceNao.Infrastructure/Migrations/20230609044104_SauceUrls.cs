using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SauceNAO.Infrastructure.Migrations
{
    public partial class SauceUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AntiCheat_Group_ChatKey",
                schema: "tg",
                table: "AntiCheat");

            migrationBuilder.DropForeignKey(
                name: "FK_AntiCheat_User_AddedByUserId",
                schema: "tg",
                table: "AntiCheat");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSauce_Sauce_SauceId",
                schema: "tg",
                table: "UserSauce");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSauce_User_UserId",
                schema: "tg",
                table: "UserSauce");

            migrationBuilder.DropColumn(
                name: "Urls",
                schema: "tg",
                table: "Sauce");

            migrationBuilder.CreateTable(
                name: "SauceUrl",
                schema: "tg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SauceKey = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SauceUrl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SauceUrl_Sauce_SauceKey",
                        column: x => x.SauceKey,
                        principalSchema: "tg",
                        principalTable: "Sauce",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SauceUrl_SauceKey",
                schema: "tg",
                table: "SauceUrl",
                column: "SauceKey");

            migrationBuilder.AddForeignKey(
                name: "FK_AntiCheat_Group_ChatKey",
                schema: "tg",
                table: "AntiCheat",
                column: "ChatKey",
                principalSchema: "tg",
                principalTable: "Group",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AntiCheat_User_AddedByUserId",
                schema: "tg",
                table: "AntiCheat",
                column: "AddedByUserId",
                principalSchema: "tg",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSauce_Sauce_SauceId",
                schema: "tg",
                table: "UserSauce",
                column: "SauceId",
                principalSchema: "tg",
                principalTable: "Sauce",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSauce_User_UserId",
                schema: "tg",
                table: "UserSauce",
                column: "UserId",
                principalSchema: "tg",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AntiCheat_Group_ChatKey",
                schema: "tg",
                table: "AntiCheat");

            migrationBuilder.DropForeignKey(
                name: "FK_AntiCheat_User_AddedByUserId",
                schema: "tg",
                table: "AntiCheat");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSauce_Sauce_SauceId",
                schema: "tg",
                table: "UserSauce");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSauce_User_UserId",
                schema: "tg",
                table: "UserSauce");

            migrationBuilder.DropTable(
                name: "SauceUrl",
                schema: "tg");

            migrationBuilder.AddColumn<string>(
                name: "Urls",
                schema: "tg",
                table: "Sauce",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_AntiCheat_Group_ChatKey",
                schema: "tg",
                table: "AntiCheat",
                column: "ChatKey",
                principalSchema: "tg",
                principalTable: "Group",
                principalColumn: "Key");

            migrationBuilder.AddForeignKey(
                name: "FK_AntiCheat_User_AddedByUserId",
                schema: "tg",
                table: "AntiCheat",
                column: "AddedByUserId",
                principalSchema: "tg",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSauce_Sauce_SauceId",
                schema: "tg",
                table: "UserSauce",
                column: "SauceId",
                principalSchema: "tg",
                principalTable: "Sauce",
                principalColumn: "Key");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSauce_User_UserId",
                schema: "tg",
                table: "UserSauce",
                column: "UserId",
                principalSchema: "tg",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
