using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreeDimensionalWorld.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAttributionLinkNulls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttributionLink",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttributionLink",
                table: "Products");
        }
    }
}
