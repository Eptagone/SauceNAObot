using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SauceNao.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppChat",
                columns: table => new
                {
                    ChatId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    InviteLink = table.Column<string>(type: "TEXT", nullable: true),
                    Lang = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppChat", x => x.ChatId);
                });

            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Lang = table.Column<string>(type: "TEXT", maxLength: 8, nullable: true),
                    LangForce = table.Column<bool>(type: "INTEGER", nullable: false),
                    Start = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SuccessfulSauce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileUniqueId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    FileId = table.Column<string>(type: "TEXT", nullable: false),
                    Info = table.Column<string>(type: "TEXT", nullable: false),
                    Urls = table.Column<string>(type: "TEXT", nullable: false),
                    Similarity = table.Column<float>(type: "REAL", nullable: false, defaultValueSql: "55"),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuccessfulSauce", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemporalFile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileUniqueId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemporalFile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AntiCheat",
                columns: table => new
                {
                    AntiCheattd = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<int>(type: "INTEGER", nullable: false),
                    AddedByUserId = table.Column<long>(type: "INTEGER", nullable: false),
                    Id = table.Column<long>(type: "INTEGER", nullable: true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AntiCheat", x => x.AntiCheattd);
                    table.ForeignKey(
                        name: "FK_AntiCheat_AppChat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "AppChat",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AntiCheat_AppUser_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "AppUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSauce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SauceId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSauce", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSauce_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSauce_SuccessfulSauce_SauceId",
                        column: x => x.SauceId,
                        principalTable: "SuccessfulSauce",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AntiCheat_AddedByUserId",
                table: "AntiCheat",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AntiCheat_ChatId",
                table: "AntiCheat",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "UQ_FileUniqueId",
                table: "SuccessfulSauce",
                column: "FileUniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSauce_SauceId",
                table: "UserSauce",
                column: "SauceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSauce_UserId",
                table: "UserSauce",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AntiCheat");

            migrationBuilder.DropTable(
                name: "TemporalFile");

            migrationBuilder.DropTable(
                name: "UserSauce");

            migrationBuilder.DropTable(
                name: "AppChat");

            migrationBuilder.DropTable(
                name: "AppUser");

            migrationBuilder.DropTable(
                name: "SuccessfulSauce");
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
