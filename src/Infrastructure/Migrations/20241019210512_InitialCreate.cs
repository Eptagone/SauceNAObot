// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SauceNAO.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Chats",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ChatId = table.Column<long>(type: "bigint", nullable: false),
                Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                Username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                LanguageCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Chats", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Sauces",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Similarity = table.Column<float>(type: "real", nullable: false),
                Links = table.Column<string[]>(type: "text[]", nullable: false),
                Title = table.Column<string>(type: "text", nullable: true),
                Author = table.Column<string>(type: "text", nullable: true),
                Characters = table.Column<string>(type: "text", nullable: true),
                Material = table.Column<string>(type: "text", nullable: true),
                Part = table.Column<string>(type: "text", nullable: true),
                Year = table.Column<string>(type: "text", nullable: true),
                EstimationTime = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Sauces", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SearchedMedias",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FileUniqueId = table.Column<string>(type: "text", nullable: false),
                FileId = table.Column<string>(type: "text", nullable: false),
                ThumbnailFileId = table.Column<string>(type: "text", nullable: true),
                MediaType = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SearchedMedias", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                FirstName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                LastName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                Username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                AlwaysUseOwnLanguage = table.Column<bool>(type: "boolean", nullable: false),
                PrivateChatStarted = table.Column<bool>(type: "boolean", nullable: false),
                SaveRecentSauces = table.Column<bool>(type: "boolean", nullable: false),
                LanguageCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AntiCheatRestrictions",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RestrictedBotId = table.Column<long>(type: "bigint", nullable: false),
                GroupId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AntiCheatRestrictions", x => x.Id);
                table.ForeignKey(
                    name: "FK_AntiCheatRestrictions_Chats_GroupId",
                    column: x => x.GroupId,
                    principalTable: "Chats",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SauceSauceMedia",
            columns: table => new
            {
                AssociatedMediasId = table.Column<int>(type: "integer", nullable: false),
                SaucesId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SauceSauceMedia", x => new { x.AssociatedMediasId, x.SaucesId });
                table.ForeignKey(
                    name: "FK_SauceSauceMedia_Sauces_SaucesId",
                    column: x => x.SaucesId,
                    principalTable: "Sauces",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_SauceSauceMedia_SearchedMedias_AssociatedMediasId",
                    column: x => x.AssociatedMediasId,
                    principalTable: "SearchedMedias",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ApiKeys",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Value = table.Column<string>(type: "text", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                OwnerId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ApiKeys", x => x.Id);
                table.ForeignKey(
                    name: "FK_ApiKeys_Users_OwnerId",
                    column: x => x.OwnerId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SearchRecords",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                SearchedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Similarity = table.Column<float>(type: "real", nullable: false),
                UserId = table.Column<int>(type: "integer", nullable: false),
                MediaId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SearchRecords", x => x.Id);
                table.ForeignKey(
                    name: "FK_SearchRecords_SearchedMedias_MediaId",
                    column: x => x.MediaId,
                    principalTable: "SearchedMedias",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_SearchRecords_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AntiCheatRestrictions_GroupId",
            table: "AntiCheatRestrictions",
            column: "GroupId");

        migrationBuilder.CreateIndex(
            name: "IX_ApiKeys_OwnerId",
            table: "ApiKeys",
            column: "OwnerId");

        migrationBuilder.CreateIndex(
            name: "IX_Chats_ChatId",
            table: "Chats",
            column: "ChatId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_SauceSauceMedia_SaucesId",
            table: "SauceSauceMedia",
            column: "SaucesId");

        migrationBuilder.CreateIndex(
            name: "IX_SearchRecords_MediaId",
            table: "SearchRecords",
            column: "MediaId");

        migrationBuilder.CreateIndex(
            name: "IX_SearchRecords_UserId",
            table: "SearchRecords",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_Users_UserId",
            table: "Users",
            column: "UserId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AntiCheatRestrictions");

        migrationBuilder.DropTable(
            name: "ApiKeys");

        migrationBuilder.DropTable(
            name: "SauceSauceMedia");

        migrationBuilder.DropTable(
            name: "SearchRecords");

        migrationBuilder.DropTable(
            name: "Chats");

        migrationBuilder.DropTable(
            name: "Sauces");

        migrationBuilder.DropTable(
            name: "SearchedMedias");

        migrationBuilder.DropTable(
            name: "Users");
    }
}
