using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SauceNAO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                CREATE FUNCTION ""UpdatedAt_Function""() RETURNS TRIGGER LANGUAGE PLPGSQL AS $$
                BEGIN
                    NEW.""updated_at"" := now();
                    RETURN NEW;
                END;
                $$;"
            );

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    username = table.Column<string>(
                        type: "character varying(32)",
                        maxLength: 32,
                        nullable: true
                    ),
                    language_code = table.Column<string>(
                        type: "character varying(8)",
                        maxLength: 8,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_groups", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "media_files",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    file_unique_id = table.Column<string>(type: "text", nullable: false),
                    file_id = table.Column<string>(type: "text", nullable: false),
                    mime_type = table.Column<string>(type: "text", nullable: true),
                    thumbnail_file_id = table.Column<string>(type: "text", nullable: true),
                    thumbnail_mime_type = table.Column<string>(type: "text", nullable: true),
                    media_type = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "now()"
                    ),
                    updated_at = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    sauces = table.Column<string>(type: "jsonb", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media_files", x => x.id);
                }
            );
            migrationBuilder.Sql(
                @"CREATE TRIGGER ""UpdateMediaTimestamp""
                BEFORE INSERT OR UPDATE
                ON ""media_files""
                FOR EACH ROW
                EXECUTE FUNCTION ""UpdatedAt_Function""();"
            );

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    first_name = table.Column<string>(
                        type: "character varying(128)",
                        maxLength: 128,
                        nullable: false
                    ),
                    last_name = table.Column<string>(
                        type: "character varying(128)",
                        maxLength: 128,
                        nullable: true
                    ),
                    username = table.Column<string>(
                        type: "character varying(32)",
                        maxLength: 32,
                        nullable: true
                    ),
                    language_code = table.Column<string>(
                        type: "character varying(8)",
                        maxLength: 8,
                        nullable: true
                    ),
                    use_fixed_language = table.Column<bool>(type: "boolean", nullable: false),
                    has_started_dm = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "api_keys",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    name = table.Column<string>(
                        type: "character varying(32)",
                        maxLength: 32,
                        nullable: false
                    ),
                    value = table.Column<string>(
                        type: "character varying(40)",
                        maxLength: 40,
                        nullable: false
                    ),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    owner_id = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_api_keys", x => x.id);
                    table.ForeignKey(
                        name: "fk_api_keys_users_owner_id",
                        column: x => x.owner_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "search_records",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    similarity = table.Column<float>(type: "real", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    media_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "now()"
                    ),
                    updated_at = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_search_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_search_records_media_files_media_id",
                        column: x => x.media_id,
                        principalTable: "media_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_search_records_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );
            migrationBuilder.Sql(
                @"CREATE TRIGGER ""UpdateSearchTimestamp""
                BEFORE INSERT OR UPDATE
                ON ""search_records""
                FOR EACH ROW
                EXECUTE FUNCTION ""UpdatedAt_Function""();"
            );

            migrationBuilder.CreateIndex(
                name: "ix_api_keys_owner_id",
                table: "api_keys",
                column: "owner_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_groups_chat_id",
                table: "groups",
                column: "chat_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_media_files_file_id",
                table: "media_files",
                column: "file_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_media_files_file_unique_id",
                table: "media_files",
                column: "file_unique_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_search_records_media_id",
                table: "search_records",
                column: "media_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_search_records_user_id",
                table: "search_records",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_user_id",
                table: "users",
                column: "user_id",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER ""UpdateMediaTimestamp"" ON ""media_files"";");
            migrationBuilder.Sql(@"DROP TRIGGER ""UpdateSearchTimestamp"" ON ""search_records"";");
            migrationBuilder.Sql(@"DROP FUNCTION ""UpdatedAt_Function"";");

            migrationBuilder.DropTable(name: "api_keys");

            migrationBuilder.DropTable(name: "groups");

            migrationBuilder.DropTable(name: "search_records");

            migrationBuilder.DropTable(name: "media_files");

            migrationBuilder.DropTable(name: "users");
        }
    }
}
