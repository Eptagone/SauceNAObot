using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SauceNAO.Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.EnsureSchema(
			name: "tg");

		migrationBuilder.CreateTable(
			name: "Group",
			schema: "tg",
			columns: table => new
			{
				Key = table.Column<int>(type: "int", nullable: false)
					.Annotation("SqlServer:Identity", "1, 1"),
				LanguageCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
				Id = table.Column<long>(type: "bigint", nullable: false),
				Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
				Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Username = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InviteLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
				LinkedChatId = table.Column<long>(type: "bigint", nullable: true)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_Group", x => x.Key);
			});

		migrationBuilder.CreateTable(
			name: "Sauce",
			schema: "tg",
			columns: table => new
			{
				Key = table.Column<int>(type: "int", nullable: false)
					.Annotation("SqlServer:Identity", "1, 1"),
				FileUniqueId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
				Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
				FileId = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Info = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Urls = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Similarity = table.Column<float>(type: "real", nullable: false),
				Date = table.Column<DateTime>(type: "datetime2", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_Sauce", x => x.Key);
			});

		migrationBuilder.CreateTable(
			name: "User",
			schema: "tg",
			columns: table => new
			{
				Id = table.Column<long>(type: "bigint", nullable: false),
				LangForce = table.Column<bool>(type: "bit", nullable: false),
				Start = table.Column<bool>(type: "bit", nullable: false),
				ApiKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
				FirstName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
				LastName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
				Username = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
				LanguageCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_User", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "AntiCheat",
			schema: "tg",
			columns: table => new
			{
				Key = table.Column<int>(type: "int", nullable: false)
					.Annotation("SqlServer:Identity", "1, 1"),
				ChatKey = table.Column<int>(type: "int", nullable: false),
				BotId = table.Column<long>(type: "bigint", nullable: false),
				AddedByUserId = table.Column<long>(type: "bigint", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_AntiCheat", x => x.Key);
				table.ForeignKey(
					name: "FK_AntiCheat_Group_ChatKey",
					column: x => x.ChatKey,
					principalSchema: "tg",
					principalTable: "Group",
					principalColumn: "Key");
				table.ForeignKey(
					name: "FK_AntiCheat_User_AddedByUserId",
					column: x => x.AddedByUserId,
					principalSchema: "tg",
					principalTable: "User",
					principalColumn: "Id");
			});

		migrationBuilder.CreateTable(
			name: "UserSauce",
			schema: "tg",
			columns: table => new
			{
				Key = table.Column<int>(type: "int", nullable: false)
					.Annotation("SqlServer:Identity", "1, 1"),
				SauceId = table.Column<int>(type: "int", nullable: false),
				UserId = table.Column<long>(type: "bigint", nullable: false),
				Date = table.Column<DateTime>(type: "datetime2", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_UserSauce", x => x.Key);
				table.ForeignKey(
					name: "FK_UserSauce_Sauce_SauceId",
					column: x => x.SauceId,
					principalSchema: "tg",
					principalTable: "Sauce",
					principalColumn: "Key");
				table.ForeignKey(
					name: "FK_UserSauce_User_UserId",
					column: x => x.UserId,
					principalSchema: "tg",
					principalTable: "User",
					principalColumn: "Id");
			});

		migrationBuilder.CreateIndex(
			name: "IX_AntiCheat_AddedByUserId",
			schema: "tg",
			table: "AntiCheat",
			column: "AddedByUserId");

		migrationBuilder.CreateIndex(
			name: "IX_AntiCheat_ChatKey",
			schema: "tg",
			table: "AntiCheat",
			column: "ChatKey");

		migrationBuilder.CreateIndex(
			name: "UQ_FileUniqueId",
			schema: "tg",
			table: "Sauce",
			column: "FileUniqueId",
			unique: true);

		migrationBuilder.CreateIndex(
			name: "IX_UserSauce_SauceId",
			schema: "tg",
			table: "UserSauce",
			column: "SauceId");

		migrationBuilder.CreateIndex(
			name: "IX_UserSauce_UserId",
			schema: "tg",
			table: "UserSauce",
			column: "UserId");
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(
			name: "AntiCheat",
			schema: "tg");

		migrationBuilder.DropTable(
			name: "UserSauce",
			schema: "tg");

		migrationBuilder.DropTable(
			name: "Group",
			schema: "tg");

		migrationBuilder.DropTable(
			name: "Sauce",
			schema: "tg");

		migrationBuilder.DropTable(
			name: "User",
			schema: "tg");
	}
}
