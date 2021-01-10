using Microsoft.EntityFrameworkCore.Migrations;

namespace SauceNAO.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Key = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id = table.Column<long>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    InviteLink = table.Column<string>(nullable: true),
                    IsChannel = table.Column<bool>(nullable: false),
                    AntiCheats = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsBotException = table.Column<string>(nullable: true),
                    ErrorCode = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Details = table.Column<string>(nullable: true),
                    JsonMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Searches",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Okey = table.Column<bool>(nullable: false),
                    File = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    OriginalFile = table.Column<string>(nullable: true),
                    Data = table.Column<string>(nullable: true),
                    TmpUrl = table.Column<string>(nullable: true),
                    SearchDate = table.Column<uint>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Characters = table.Column<string>(nullable: true),
                    Material = table.Column<string>(nullable: true),
                    Part = table.Column<string>(nullable: true),
                    Year = table.Column<string>(nullable: true),
                    EstTime = table.Column<string>(nullable: true),
                    By = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Searches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Lang = table.Column<string>(nullable: true),
                    APIKEY = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Whitelists",
                columns: table => new
                {
                    Key = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    ChatKey = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Whitelists", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Whitelists_Chats_ChatKey",
                        column: x => x.ChatKey,
                        principalTable: "Chats",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Key = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<uint>(nullable: false),
                    SauceFile = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.Key);
                    table.ForeignKey(
                        name: "FK_History_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_History_Id",
                table: "History",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Whitelists_ChatKey",
                table: "Whitelists",
                column: "ChatKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Searches");

            migrationBuilder.DropTable(
                name: "Whitelists");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Chats");
        }
    }
}
