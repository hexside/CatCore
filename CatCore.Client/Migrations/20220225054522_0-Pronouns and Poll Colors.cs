using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatCore.Client.Migrations
{
    public partial class _0PronounsandPollColors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AnyPronouns",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NoPronouns",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnyPronouns",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NoPronouns",
                table: "Users");
        }
    }
}
