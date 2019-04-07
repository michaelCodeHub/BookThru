using Microsoft.EntityFrameworkCore.Migrations;

namespace BookThru.Migrations
{
    public partial class migration10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromName",
                table: "Contact",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToName",
                table: "Contact",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromName",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "ToName",
                table: "Contact");
        }
    }
}
