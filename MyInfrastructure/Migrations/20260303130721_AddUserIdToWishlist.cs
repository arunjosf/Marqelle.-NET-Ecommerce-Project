using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marqelle.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToWishlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Wishlists",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
