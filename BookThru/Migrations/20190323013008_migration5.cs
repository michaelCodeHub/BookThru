using Microsoft.EntityFrameworkCore.Migrations;

namespace BookThru.Migrations
{
    public partial class migration5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "BookBid",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BookBid_BookId",
                table: "BookBid",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookBid_Book_BookId",
                table: "BookBid",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookBid_Book_BookId",
                table: "BookBid");

            migrationBuilder.DropIndex(
                name: "IX_BookBid_BookId",
                table: "BookBid");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "BookBid");
        }
    }
}
