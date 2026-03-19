using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marqelle.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EmailUpdation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdatingEmail",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatingEmail",
                table: "Users");
        }
    }
}
