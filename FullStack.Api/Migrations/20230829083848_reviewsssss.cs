using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FullStack.Api.Migrations
{
    public partial class reviewsssss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProducts_Users_ContactId",
                table: "UserProducts");

            migrationBuilder.DropIndex(
                name: "IX_UserProducts_ContactId",
                table: "UserProducts");

            migrationBuilder.AddColumn<int>(
                name: "UserContactId",
                table: "UserProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Reviews = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProducts_UserContactId",
                table: "UserProducts",
                column: "UserContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProducts_Users_UserContactId",
                table: "UserProducts",
                column: "UserContactId",
                principalTable: "Users",
                principalColumn: "ContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProducts_Users_UserContactId",
                table: "UserProducts");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_UserProducts_UserContactId",
                table: "UserProducts");

            migrationBuilder.DropColumn(
                name: "UserContactId",
                table: "UserProducts");

            migrationBuilder.CreateIndex(
                name: "IX_UserProducts_ContactId",
                table: "UserProducts",
                column: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProducts_Users_ContactId",
                table: "UserProducts",
                column: "ContactId",
                principalTable: "Users",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
