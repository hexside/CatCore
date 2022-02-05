using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatCore.Client.Migrations
{
    public partial class Messages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuppressed",
                table: "UserMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuppressed",
                table: "UserMessages");
        }
    }
}
