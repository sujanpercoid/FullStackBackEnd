using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FullStack.Api.Migrations
{
    public partial class tryyyyyy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProducts",
                columns: table => new
                {
                    UserProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProducts", x => x.UserProductId);
                    table.ForeignKey(
                        name: "FK_UserProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProducts_Users_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Users",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProducts_ContactId",
                table: "UserProducts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProducts_ProductId",
                table: "UserProducts",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProducts");
        }
    }
}
