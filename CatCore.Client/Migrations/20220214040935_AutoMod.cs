using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatCore.Client.Migrations
{
    public partial class AutoMod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "MessageFlagChannelId",
                table: "Guilds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateTable(
                name: "RegexActions",
                columns: table => new
                {
                    RegexActionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Valid = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActionName = table.Column<string>(type: "TEXT", nullable: true),
                    GuildId = table.Column<int>(type: "INTEGER", nullable: false),
                    RegexString = table.Column<string>(type: "TEXT", nullable: true),
                    TriggerByDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    CleanMessage = table.Column<bool>(type: "INTEGER", nullable: false),
                    RemoveWhitespace = table.Column<bool>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegexActions", x => x.RegexActionId);
                    table.ForeignKey(
                        name: "FK_RegexActions_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegexActionTriggers",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegexActionConditionId = table.Column<int>(type: "INTEGER", nullable: false),
                    RegexActionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegexActionTriggers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegexActionTriggers_RegexActions_RegexActionId",
                        column: x => x.RegexActionId,
                        principalTable: "RegexActions",
                        principalColumn: "RegexActionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegexActions_GuildId",
                table: "RegexActions",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_RegexActionTriggers_RegexActionId",
                table: "RegexActionTriggers",
                column: "RegexActionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegexActionTriggers");

            migrationBuilder.DropTable(
                name: "RegexActions");

            migrationBuilder.DropColumn(
                name: "MessageFlagChannelId",
                table: "Guilds");
        }
    }
}
