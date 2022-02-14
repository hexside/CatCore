using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatCore.Client.Migrations
{
    public partial class FixTriggerByDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegexActionTriggers");

            migrationBuilder.DropColumn(
                name: "TriggerByDefault",
                table: "RegexActions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TriggerByDefault",
                table: "RegexActions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RegexActionTriggers",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegexActionId = table.Column<int>(type: "INTEGER", nullable: false),
                    RegexActionConditionId = table.Column<int>(type: "INTEGER", nullable: false),
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
                name: "IX_RegexActionTriggers_RegexActionId",
                table: "RegexActionTriggers",
                column: "RegexActionId");
        }
    }
}
