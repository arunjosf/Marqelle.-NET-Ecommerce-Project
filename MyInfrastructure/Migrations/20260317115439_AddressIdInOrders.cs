using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marqelle.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddressIdInOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                table: "Orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Orders");
        }
    }
}
