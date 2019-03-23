using Microsoft.EntityFrameworkCore.Migrations;

namespace BookThru.Migrations
{
    public partial class migration6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentBidder",
                table: "Book",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentBidder",
                table: "Book");
        }
    }
}
