using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marqelle.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductsSizeAndStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Products_ProductId",
                table: "Stocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks");

            migrationBuilder.RenameTable(
                name: "Stocks",
                newName: "SizeAndStocks");

            migrationBuilder.RenameIndex(
                name: "IX_Stocks_ProductId",
                table: "SizeAndStocks",
                newName: "IX_SizeAndStocks_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SizeAndStocks",
                table: "SizeAndStocks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SizeAndStocks_Products_ProductId",
                table: "SizeAndStocks",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SizeAndStocks_Products_ProductId",
                table: "SizeAndStocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SizeAndStocks",
                table: "SizeAndStocks");

            migrationBuilder.RenameTable(
                name: "SizeAndStocks",
                newName: "Stocks");

            migrationBuilder.RenameIndex(
                name: "IX_SizeAndStocks_ProductId",
                table: "Stocks",
                newName: "IX_Stocks_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Products_ProductId",
                table: "Stocks",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
