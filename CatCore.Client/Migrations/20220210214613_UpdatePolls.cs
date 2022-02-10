using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatCore.Client.Migrations
{
    public partial class UpdatePolls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "Polls",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Polls",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.GuildId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Polls_GuildId",
                table: "Polls",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Polls_Guilds_GuildId",
                table: "Polls",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Polls_Guilds_GuildId",
                table: "Polls");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropIndex(
                name: "IX_Polls_GuildId",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Polls");
        }
    }
}
